using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Node
{
    public int value;
    public List<BG_Edge> edges;

    public BG_Node(int value)
    {
        this.value = value;
        edges = new List<BG_Edge>();
    }

    public void AddEdge(BG_Node node_a, BG_Node node_b)
    {
        BG_Edge temp = new BG_Edge(node_a, node_b);
        edges.Add(temp);
    }

    public void RemoveEdge(BG_Node node_a, BG_Node node_b)
    {
        foreach (BG_Edge edge in edges) 
        {
            if ((edge.neighbour_a == node_a || edge.neighbour_b == node_a) && (edge.neighbour_a == node_b || edge.neighbour_b == node_b))
            {
                edges.Remove(edge);
                return;
            }
        }
    }


}
