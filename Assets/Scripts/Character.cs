using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class Character : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [Header("Animations")]
    [SerializeField] AnimationCurve walkCurve;
    [SerializeField] AnimationCurve jumpUpCurve;
    [SerializeField] AnimationCurve jumpDownCurve;
    [SerializeField] float walkAnimationSpeed;
    SpriteManager spriteManager;
    float jumpDuration;
    float pathIterationWait;

    [Header("Pathfinding")]
    [SerializeField] LayerMask sampleMask;
    List<Node> highlightedNodes = new List<Node>();
    Node currentOccupiedNode;
    Node currentTargetNode;

    [Header("Gameplay")]
    [SerializeField] CharacterData characterData;
    [SerializeField] CharacterDirection startDirection;
    [SerializeField] int team;
    CharacterDirection currentDirection;
    bool isWalking;
    bool isChoosingDirection;
    #endregion

    #region Public
    public CharacterData CharacterData { get { return characterData; } }
    public Node CurrentOccupiedNode { get { return currentOccupiedNode; } }
    public List<Node> HighlightedNodes { get { return highlightedNodes; } }
    public bool IsWalking { get { return isWalking; } }
    public float Jump { get { return characterData.Jump; } }
    public bool IsChoosingDirection {  get { return isChoosingDirection; } }
    #endregion

    #endregion

    #region Monobehaviour
    private void Awake()
    {
        spriteManager = GetComponent<SpriteManager>();    
    }

    private void Start()
    {
        switch (startDirection)
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

        Debug.Log("Current direction: " + currentDirection);
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
        if (isWalking)
        {
            Debug.LogWarning("Character still walking!");
            return;
        }

        if (path == null)
        {
            Debug.LogWarning("Path[] is null!");
            return;
        }

        currentOccupiedNode.SetOccupationId(-1);
        UpdateDirection((path[1].transform.position - transform.position).normalized);
        currentTargetNode = path[path.Length - 1];
        StartCoroutine(PathWalkCoroutine(path));
    }

    IEnumerator PathWalkCoroutine(Node[] path)
    {
        isWalking = true;
        TileGrid.Instance.turnState = TurnState.MOVING;
        currentOccupiedNode.SetOccupationId(-1);
        HighlightTargetNode();

        for (int i = 1; i < path.Length; i++) 
        {
            UpdateDirection((path[i].transform.position - transform.position).normalized);

            yield return new WaitForEndOfFrame();

            if (path[i].transform.position.y != transform.position.y)
            {
                int jumpSteps = (int)Mathf.Abs(path[i].transform.position.y - transform.position.y);

                if (characterData.Jump == jumpSteps)
                    jumpSteps--;

                jumpDuration = jumpSteps >= characterData.Jump - 2  && jumpSteps > 2 ? ((walkAnimationSpeed / 2f) * characterData.Jump) / (characterData.Jump - jumpSteps) : walkAnimationSpeed;

                bool jumpDir = transform.position.y < path[i].transform.position.y;

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
                transform.DOMove(path[i].transform.position, walkAnimationSpeed).SetEase(walkCurve);
                pathIterationWait = walkAnimationSpeed;
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

        TileGrid.Instance.turnState = TurnState.CHOOSING;
        //yield return new WaitForSeconds(1f);
        isWalking = false;

        //choosingDirection = true;

        TileGrid.Instance.PassTurn();
    }

    private void UpdateDirection(Vector3 dir)
    {
        //Debug.Log("Direction: " + transform.rotation.eulerAngles);
        Quaternion lookRot = Quaternion.LookRotation(dir);
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
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, float.MaxValue, sampleMask))
        {
            currentOccupiedNode = hit.collider.GetComponent<Node>();
            Debug.Log("current occupied node: " + currentOccupiedNode.name);
            currentOccupiedNode.SetOccupationId(team);
        }

        //Debug.DrawRay(transform.position + Vector3.up, Vector3.down * 5f, Color.red, 10f);
    }

    private void HighlightNodesInRange()
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        highlightedNodes = TileGrid.Instance.GetAreaUtility(currentOccupiedNode, characterData.Move, characterData.Jump).ToList();

        Debug.Log("areaResult count: " + highlightedNodes.Count);

        foreach (Node node in highlightedNodes)
        {
            if (node != currentOccupiedNode)
                node.HighlightNode(TileGrid.Instance.UnselectedTileColor);
            else
                highlightedNodes.Remove(node);
        }

        TileGrid.Instance.currentHighlightedNode = currentOccupiedNode;
        currentOccupiedNode.PointNode();
        Debug.Log("CurrentHighLightedNode: " + TileGrid.Instance.currentHighlightedNode.name);
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
        currentTargetNode.HighlightNode(TileGrid.Instance.SelectedTileColor);
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

        isChoosingDirection = false;

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
    WAITING,
    CHOOSING,
    MOVING
}

public enum Team
{
    ALLY,
    ENEMY,
    NEUTRAL
}
