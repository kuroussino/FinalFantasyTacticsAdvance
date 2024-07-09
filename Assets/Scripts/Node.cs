using CustomSorting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //Graph (Pure)
    private int value;
    private List<Edge> edges;

    [SerializeField, Min(1)] float cost = 1;
    [SerializeField] LayerMask sampleMask;
    [SerializeField] float blockScale;
    int occupationId = -1;

    [SerializeField] SpriteRenderer highlightedSprite;

    public int Value { get { return value; } }
    public List<Edge> Edges { get { return edges; } }
    public int OccupationId => occupationId;

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

    public Node(int value)
    {
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

    public float GetManhattanDistance(Node tile)
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
            if (hit.transform.gameObject.TryGetComponent(out Node tile))
            { 
                edges.Add(new Edge(this, tile, cost, CalculateStepHeight(tile)));
            }
        }
    }

    public float CalculateStepHeight(Node tileB) //pivot must be on top of the cube mesh and character at its foot
    {
        return Mathf.Abs(tileB.transform.position.y - transform.position.y);
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
