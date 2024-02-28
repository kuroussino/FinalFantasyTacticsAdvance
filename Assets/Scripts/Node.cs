using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //Tile (Monobehaviour)
    [SerializeField] private Tile relatedTile = null;
    public Tile RelatedTile { get { return relatedTile; } }

    //Graph (Pure)
    private int value;
    private List<Edge> edges = new List<Edge>();

    public int Value { get { return value; } }
    public List<Edge> Edges { get { return edges; } }

    //Struct Edges
    [SerializeField] List<EdgeInfo> edgesInfo = new List<EdgeInfo>();

    //Dijkstra
    [HideInInspector] public float DistanceFromSource = float.MaxValue;
    [HideInInspector] public Node PreviousNode = null;

    //A*
    [HideInInspector] public float HeuristicValue = float.MaxValue;

    #region Monobehaviour
    private void Awake()
    {
        relatedTile = GetComponent<Tile>();
    }

    private void Start()
    {
        //Populate the edges list with the ones got in inspector
        if (edgesInfo.Count != 0)
        {
            foreach (EdgeInfo edgeInfo in edgesInfo)
            {
                AddEdge(edgeInfo.end, edgeInfo.weight);
            }
        }
    }
    #endregion

    public Node(int value) 
    {
        this.value = value;
    }

    public void AddEdge(Node otherNode, float leght, bool isBidirectional = true)
    {
        Edges.Add(new Edge(otherNode, leght, isBidirectional));

        otherNode.Edges.Add(new Edge(this, leght, isBidirectional));
    }

    public void RemoveEdge(Node otherNode) 
    {
        Edges.RemoveAll(edge => edge.ConnectedNode == otherNode);
        otherNode.Edges.RemoveAll(edge => edge.ConnectedNode == this);
    }
}

[System.Serializable]
public struct EdgeInfo
{
    [SerializeField] public Node end;
    [SerializeField] public float weight;
}
