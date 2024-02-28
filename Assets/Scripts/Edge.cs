using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Edge : MonoBehaviour
{
    [SerializeField] private Node connectedNode;
    [SerializeField] private float weight;
    [SerializeField] private bool isBidirectional;

    ////Tile (Monobehaviour)
    //[SerializeField] private Node start;
    //[SerializeField] private Node end;

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
