using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;

public class Character : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [SerializeField] float walkDuration;
    [SerializeField] float jumpDuration;
    float pathIterationWait;
    [SerializeField] LayerMask sampleMask;
    [SerializeField] AnimationCurve walkCurve;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] float jumpAnimationHeight;
    [SerializeField] float attackRange;
    [SerializeField] float stepHeight;
    SpriteManager spriteManager;
    CharacterDirection currentDirection;
    Node currentOccupiedNode;

    List<Node> highlightedNodes = new List<Node>();

    bool walking;
    #endregion

    #region Public
    public Node CurrentOccupiedNode {  get { return currentOccupiedNode; } }
    public List<Node> HighlightedNodes { get { return highlightedNodes; } }
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
    }

    private void OnDisable()
    {
        EventsManager.NodeClicked -= MoveTo;
        EventsManager.AllNodesRegistered -= GetCurrentOccupiedNode;
        EventsManager.AllNodesRegistered -= HighlightNodesInRange;
    }
    #endregion

    #region Methods
    private void MoveTo(Node[] path)
    {
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

        ClearHighlightedNodes();
        StartCoroutine(PathWalkCoroutine(path));
    }

    IEnumerator PathWalkCoroutine(Node[] path)
    {
        walking = true;

        for (int i = 0; i < path.Length; i++) 
        {
            if (i != 0)
                UpdateDirection((path[i].transform.position - transform.position).normalized);
            else
                UpdateDirection((path[i + 1].transform.position - path[i].transform.position).normalized);

            if (path[i].transform.position.y != transform.position.y)
            {
                //transform.DOMove(path[i].transform.position, jumpDuration - jumpDuration / 3).SetEase(jumpCurve);

                transform.DOMove(transform.position + new Vector3(0.85f, jumpAnimationHeight, 0f), jumpDuration).SetEase(jumpCurve);
                transform.DOMove(path[i].transform.position + new Vector3(0f, jumpAnimationHeight, 0f), jumpDuration).SetEase(jumpCurve);
                transform.DOMove(path[i].transform.position, jumpDuration/2).SetEase(jumpCurve).SetDelay(jumpDuration / 2);

                pathIterationWait = jumpDuration;

                Debug.Log("Jump --> " + (path[i].transform.position.y - transform.position.y));
            }
            else
            {
                transform.DOMove(path[i].transform.position, walkDuration).SetEase(walkCurve);
                pathIterationWait = walkDuration;
            }

            //GetCurrentOccupiedNode();

            yield return new WaitForSeconds(pathIterationWait);
        }
        yield return null;
        GetCurrentOccupiedNode();
        transform.position = path[path.Length - 1].transform.position;

        walking = false;
        yield return new WaitForSeconds(0.5f);
        HighlightNodesInRange();
    }

    private void UpdateDirection(Vector3 dir)
    {
        transform.rotation = Quaternion.LookRotation(dir);

        if ((int)transform.rotation.eulerAngles.y == 0)
        {
            spriteManager.UpdateSpriteOrientation(CharacterDirection.FRONT);
            currentDirection = CharacterDirection.FRONT;
            return;
        }

        int yRot = 540 / (int)transform.rotation.eulerAngles.y;

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
            currentOccupiedNode = hit.collider.GetComponent<Node>();

        //Debug.DrawRay(transform.position + Vector3.up, Vector3.down * 5f, Color.red, 10f);
    }

    private void HighlightNodesInRange()
    {
        Node[] getAreaResult = TileGrid.Instance.GetAreaUtility(currentOccupiedNode, attackRange, stepHeight);

        Debug.Log("areaResult count: " + getAreaResult.Length);

        foreach (Node node in getAreaResult)
        {
            highlightedNodes.Add(node);
        }

        for (int i = 0; i < highlightedNodes.Count; i++)
        {
            if (highlightedNodes[i] != currentOccupiedNode)
                highlightedNodes[i].HighlightNode(Color.cyan);
        }
    }

    private void ClearHighlightedNodes()
    {
        for (int i = 0; i < highlightedNodes.Count; i++)
        {
            highlightedNodes[i].LoseHighlight();
        }

        highlightedNodes.Clear();
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
