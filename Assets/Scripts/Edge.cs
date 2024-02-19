using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    private Node connectedNode;
    private float lenght;
    private bool isBidirectional;

    public Node ConnectedNode { get { return connectedNode; } }
    public float Lenght { get { return lenght; } }
    public bool IsBidirectional { get { return isBidirectional; } }

    public Edge(Node connectedNode, float lenght, bool isBidirectional = true)
    {
        this.connectedNode = connectedNode;
        this.lenght = lenght;
        this.isBidirectional = isBidirectional;
    }
}
