using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Character : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [SerializeField] float walkDuration;
    float jumpDuration;
    float pathIterationWait;
    [SerializeField] LayerMask sampleMask;
    [SerializeField] AnimationCurve walkCurve;
    [SerializeField] AnimationCurve jumpUpCurve;
    [SerializeField] AnimationCurve jumpDownCurve;
    float jumpAnimationHeight;
    [SerializeField] float attackRange;
    [SerializeField] float stepHeight;
    SpriteManager spriteManager;
    CharacterDirection currentDirection;
    Node currentOccupiedNode;
    Node currentTargetNode;
    List<Node> highlightedNodes = new List<Node>();

    bool walking;
    bool choosingDirection;

    #endregion

    #region Public
    public Node CurrentOccupiedNode {  get { return currentOccupiedNode; } }
    public List<Node> HighlightedNodes { get { return highlightedNodes; } }
    public bool Walking { get { return walking; } }
    public float StepHeight { get { return stepHeight; } }
    public bool ChoosingDirection {  get { return choosingDirection; } }
    #endregion

    #endregion

    #region Monobehaviour
    private void Awake()
    {
        spriteManager = GetComponent<SpriteManager>();    
    }

    private void Start()
    {
        UpdateDirection(Vector3.forward);
        Debug.Log("Current direction: " + currentDirection);

        //HighlightNodesInRange();
    }

    private void OnEnable()
    {
        EventsManager.NodeClicked += MoveTo;
        EventsManager.AllNodesRegistered += GetCurrentOccupiedNode;
        EventsManager.AllNodesRegistered += HighlightNodesInRange;

        EventsManager.TurnChanged += GetCurrentOccupiedNode;
        EventsManager.TurnChanged += HighlightNodesInRange;

        EventsManager.DirectionChosen += EndTurnDirectionChange;
    }

    private void OnDisable()
    {
        EventsManager.NodeClicked -= MoveTo;
        EventsManager.AllNodesRegistered -= GetCurrentOccupiedNode;
        EventsManager.AllNodesRegistered -= HighlightNodesInRange;

        EventsManager.TurnChanged -= GetCurrentOccupiedNode;
        EventsManager.TurnChanged -= HighlightNodesInRange;

        EventsManager.DirectionChosen -= EndTurnDirectionChange;
    }
    #endregion

    #region Methods
    private void MoveTo(Node[] path)
    {
        if(TileGrid.Instance.SelectedCharacter != this)
            return;

        //Debug.Log("Clicked node named: " + path[path.Length-1].name);
        if (walking)
        {
            Debug.LogWarning("Character still walking!");
            return;
        }

        if (path == null)
        {
            Debug.LogWarning("Path[] is null!");
            return;
        }
        UpdateDirection((path[1].transform.position - transform.position).normalized);
        currentTargetNode = path[path.Length - 1];
        StartCoroutine(PathWalkCoroutine(path));
    }

    IEnumerator PathWalkCoroutine(Node[] path)
    {
        walking = true;

        HighlightTargetNode();

        for (int i = 1; i < path.Length; i++) 
        {
            //if (i != 0)
            //    UpdateDirection((path[i].transform.position - transform.position).normalized);
            //else
            //    UpdateDirection((path[i + 1].transform.position - path[i].transform.position).normalized);

            UpdateDirection((path[i].transform.position - transform.position).normalized);

            yield return new WaitForEndOfFrame();

            if (path[i].transform.position.y != transform.position.y)
            {
                //transform.DOMove(transform.position + new Vector3(0.85f, jumpAnimationHeight, 0f), jumpDuration).SetEase(jumpCurve);
                //transform.DOMove(path[i].transform.position + new Vector3(0f, jumpAnimationHeight, 0f), jumpDuration).SetEase(jumpCurve);
                //transform.DOMove(path[i].transform.position, jumpDuration/2).SetEase(jumpCurve).SetDelay(jumpDuration / 2);

                //jumpAnimationHeight = path[i].transform.position.y + ((path[i].transform.position.y - transform.position.y) / 5);

                //transform.DOMove(transform.position + new Vector3((path[i].transform.position.x - transform.position.x)/1.5f, jumpAnimationHeight, (path[i].transform.position.z - transform.position.z) / 1.5f), jumpDuration).SetEase(jumpCurve);
                //transform.DOMove(path[i].transform.position + new Vector3(0f, jumpAnimationHeight, 0f), jumpDuration).SetEase(jumpCurve);
                //transform.DOMove(path[i].transform.position, jumpDuration / 2).SetEase(jumpCurve).SetDelay(jumpDuration / 2);

                int jumpSteps = (int)Mathf.Abs(path[i].transform.position.y - transform.position.y);

                if (stepHeight == jumpSteps)
                    jumpSteps--;

                jumpDuration = jumpSteps >= stepHeight - 2  && jumpSteps > 2 ? ((walkDuration / 2f) * stepHeight) / (stepHeight - jumpSteps) : walkDuration;

                bool jumpDir = transform.position.y < path[i].transform.position.y; 
                //true = jumping up
                //false = jumping down

                Vector3 origin = transform.position;
                Vector3 target = path[i].transform.position;

                float timer = 0;
                while (timer <= jumpDuration)
                {
                    timer += Time.deltaTime;
                    float percent = Mathf.Clamp01(timer / jumpDuration);
                    float curvePercentX = walkCurve.Evaluate(percent);
                    float curvePercentY = jumpDir ? jumpUpCurve.Evaluate(percent) : jumpDownCurve.Evaluate(percent);
                    float curvePercentZ = walkCurve.Evaluate(percent);
                    transform.position = new Vector3(Mathf.LerpUnclamped(origin.x, target.x, curvePercentX), Mathf.LerpUnclamped(origin.y, target.y, curvePercentY), Mathf.LerpUnclamped(origin.z, target.z, curvePercentZ));
                    yield return null;
                }

                pathIterationWait = 0f;
            }
            else
            {
                transform.DOMove(path[i].transform.position, walkDuration).SetEase(walkCurve);
                pathIterationWait = walkDuration;
            }

            yield return new WaitForSeconds(pathIterationWait);

            transform.position = path[i].transform.position;

            UpdateDirection((path[i].transform.position - transform.position).normalized);
        }

        yield return null;
        transform.position = path[path.Length - 1].transform.position;

        ClearHighlightTargetNode();
        GetCurrentOccupiedNode();
        //HighlightNodesInRange();

        //yield return new WaitForSeconds(1f);
        walking = false;

        choosingDirection = true;
        //TileGrid.Instance.PassTurn();
    }

    private void UpdateDirection(Vector3 dir)
    {
        Quaternion lookRot = Quaternion.LookRotation(dir);
        //transform.rotation = lookRot;
        //transform.rotation = Quaternion.Euler(0f, lookRot.eulerAngles.y, lookRot.eulerAngles.z);
        Debug.Log("Direction: " + transform.rotation.eulerAngles);

        Quaternion resultRot = transform.rotation * lookRot;

        if ((int)resultRot.eulerAngles.y == 0)
        {
            spriteManager.UpdateSpriteOrientation(CharacterDirection.FRONT);
            currentDirection = CharacterDirection.FRONT;
            return;
        }

        int yRot = 540 / (int)resultRot.eulerAngles.y;

        switch (yRot)
        {
            case 2:
                spriteManager.UpdateSpriteOrientation(CharacterDirection.LEFT);
                currentDirection = CharacterDirection.LEFT;
                break;
            case 3:
                spriteManager.UpdateSpriteOrientation(CharacterDirection.BACK);
                currentDirection = CharacterDirection.BACK;
                break;
            case 6:
                spriteManager.UpdateSpriteOrientation(CharacterDirection.RIGHT);
                currentDirection = CharacterDirection.RIGHT;
                break;
            default:
                break;
        }
    }

    private void GetCurrentOccupiedNode()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, float.MaxValue, sampleMask))
        {
            currentOccupiedNode = hit.collider.GetComponent<Node>();
            Debug.Log("current occupied node: " + currentOccupiedNode.name);
        }

        //Debug.DrawRay(transform.position + Vector3.up, Vector3.down * 5f, Color.red, 10f);
    }

    private void HighlightNodesInRange()
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        highlightedNodes = TileGrid.Instance.GetAreaUtility(currentOccupiedNode, attackRange, stepHeight).ToList();

        Debug.Log("areaResult count: " + highlightedNodes.Count);

        foreach (Node node in highlightedNodes)
        {
            if (node != currentOccupiedNode)
                node.HighlightNode(Color.cyan);
            else
                highlightedNodes.Remove(node);
        }
    }

    public void ClearHighlightedNodes()
    {
        for (int i = 0; i < highlightedNodes.Count; i++)
        {
            highlightedNodes[i].LoseHighlight();
        }

        highlightedNodes.Clear();
    }

    private void HighlightTargetNode()
    {
        currentTargetNode.HighlightNode(Color.red);
    }

    private void ClearHighlightTargetNode()
    {
        currentTargetNode.LoseHighlight();
    }

    private void EndTurnDirectionChange(CharacterDirection direction)
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        switch (direction)
        {
            case CharacterDirection.FRONT:
                UpdateDirection(Vector3.forward);
                break;
            case CharacterDirection.RIGHT:
                UpdateDirection(Vector3.right);
                break;
            case CharacterDirection.LEFT:
                UpdateDirection(Vector3.left);
                break;
            case CharacterDirection.BACK:
                UpdateDirection(Vector3.back);
                break;
        }

        choosingDirection = false;

        TileGrid.Instance.PassTurn();
    }
    #endregion
}

public enum CharacterDirection
{
    FRONT,
    RIGHT,
    LEFT,
    BACK
}

public enum TurnState
{
    d
}
