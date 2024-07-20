using CustomSorting;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeB : MonoBehaviour
{
    //Graph (Pure)
    private int value;
    private List<Edge> edges;

    [SerializeField] private int occupationId = -1;

    [SerializeField, Min(1)] float cost = 1;
    [SerializeField] LayerMask sampleMask;
    [SerializeField] float blockScale;

    [Header("Misc")]
    [SerializeField] SpriteRenderer highlightedSprite;
    [SerializeField] SpriteRenderer pointerHighlightedSprite;
    [SerializeField] SpriteRenderer pointerSprite;
    [SerializeField] AnimationCurve pointerUpCurve;
    [SerializeField] AnimationCurve pointerDownCurve;
    [SerializeField] float pointerAnimSpeed;
    [SerializeField] float pointerUpValue;
    Coroutine pointerCoroutine;
    Vector3 pointerPosition;

    public int Value { get { return value; } }
    public List<Edge> Edges { get { return edges; } }
    public int OccupationId { get { return occupationId; } }

    ////Struct Edges
    //[SerializeField] List<EdgeInfo> edgesInfo = new List<EdgeInfo>();

    ////Dijkstra
    //[HideInInspector] public float DistanceFromSource = float.MaxValue;
    //[HideInInspector] public Node PreviousNode = null;

    ////A*
    //[HideInInspector] public float HeuristicValue = float.MaxValue;

    #region Monobehaviour
    private void Awake()
    {
        edges = new List<Edge>();
        //relatedTile = GetComponent<Tile>();
    }

    private void Start()
    {
        RegisterToTileGrid();
        pointerPosition = pointerSprite.gameObject.transform.position;

        ////Populate the edges list with the ones got in inspector
        //if (edgesInfo.Count != 0)
        //{
        //    foreach (EdgeInfo edgeInfo in edgesInfo)
        //    {
        //        AddEdge(edgeInfo.end, edgeInfo.weight);
        //    }
        //}
    }
    #endregion

    public NodeB(int value)
    {
        occupationId = -1;
        this.value = value;
    }

    #region Methods

    public void SetValue(int id)
    {
        this.value = id;
    }

    public void SortEdges()
    {
        edges.MergeSort();
    }

    public float GetManhattanDistance(NodeB tile)
    {
        return Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.z - tile.transform.position.z);
    }

    private void RegisterToTileGrid()
    {
        BindToNeighbor(Vector3.forward);
        BindToNeighbor(Vector3.right);
        BindToNeighbor(Vector3.back);
        BindToNeighbor(Vector3.left);

        TileGrid.Instance.RegisterTile(this);
    }

    private void BindToNeighbor(Vector3 direction)
    {
        if (Physics.Raycast(transform.position + direction * blockScale + Vector3.up * float.MaxValue / 2, Vector3.down, out RaycastHit hit, float.MaxValue, sampleMask))
        {
            if (hit.transform.gameObject.TryGetComponent(out NodeB tile))
            { 
                edges.Add(new Edge(this, tile, cost, CalculateStepHeight(tile)));
            }
        }
    }

    public float CalculateStepHeight(NodeB tileB) //pivot must be on top of the cube mesh and character at its foot
    {
        return Mathf.Abs(tileB.transform.position.y - transform.position.y);
    }

    public void SetOccupationId(int id)
    {
        Debug.LogError("ID SET");
        occupationId = id;
    }

    public void HighlightNode(Color color)
    {
        highlightedSprite.color = color;
        highlightedSprite.gameObject.SetActive(true);
    }

    public void LoseHighlight()
    {
        highlightedSprite.gameObject.SetActive(false);
    }

    public void PointNode()
    {
        pointerSprite.gameObject.transform.position = pointerPosition;
        pointerHighlightedSprite.gameObject.SetActive(true);
        pointerSprite.gameObject.SetActive(true);
        pointerCoroutine = StartCoroutine(PointerAnimation());
    }

    public void UnpointNode()
    {
        pointerHighlightedSprite.gameObject.SetActive(false);
        pointerSprite.gameObject.SetActive(false);
        StopCoroutine(pointerCoroutine);
    }

    IEnumerator PointerAnimation()
    {
        while (true) 
        {
            pointerSprite.gameObject.transform.DOMove(new Vector3(pointerSprite.gameObject.transform.position.x, pointerSprite.gameObject.transform.position.y + pointerUpValue, pointerSprite.gameObject.transform.position.z), pointerAnimSpeed).SetEase(pointerUpCurve);
            yield return new WaitForSeconds(pointerAnimSpeed);
            pointerSprite.gameObject.transform.DOMove(new Vector3(pointerSprite.gameObject.transform.position.x, pointerSprite.gameObject.transform.position.y - pointerUpValue, pointerSprite.gameObject.transform.position.z), pointerAnimSpeed).SetEase(pointerDownCurve);
            yield return new WaitForSeconds(pointerAnimSpeed);
        }
    }

    //public void AddEdge(Node otherNode, float leght, bool isBidirectional = true)
    //{
    //    Edges.Add(new Edge(otherNode, leght, isBidirectional));

    //    otherNode.Edges.Add(new Edge(this, leght, isBidirectional));
    //}

    //public void RemoveEdge(Node otherNode) 
    //{
    //    Edges.RemoveAll(edge => edge.ConnectedNode == otherNode);
    //    otherNode.Edges.RemoveAll(edge => edge.ConnectedNode == this);
    //}

    #endregion
}

//[System.Serializable]
//public struct EdgeInfo
//{
//    [SerializeField] public Node end;
//    [SerializeField] public float weight;
//}
