using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    private Node connectedNode;
    private float weight;
    private bool isBidirectional;

    public Node ConnectedNode { get { return connectedNode; } }
    public float Weight { get { return weight; } }
    public bool IsBidirectional { get { return isBidirectional; } }

    public Edge(Node connectedNode, float lenght, bool isBidirectional = true)
    {
        this.connectedNode = connectedNode;
        this.weight = lenght;
        this.isBidirectional = isBidirectional;
    }
}
