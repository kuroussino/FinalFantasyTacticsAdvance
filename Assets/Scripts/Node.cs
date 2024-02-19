using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private int value;
    private List<Edge> edges = new List<Edge>();

    public int Value { get { return value; } }
    public List<Edge> Edges { get { return edges; } }

    //Dijkstra
    public float DistanceFromSource = float.MaxValue;
    public Node PreviousNode = null;

    public Node(int value) 
    {
        this.value = value;
    }

    public void AddEdge(Node otherNode, float leght, bool isBidirectional)
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
