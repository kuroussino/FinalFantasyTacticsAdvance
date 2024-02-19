using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BinaryGraph
{
    public List<BG_Node> nodes;
    private int steps = 0;
    List<BG_Node> checkedNodes = new List<BG_Node>();

    public BinaryGraph()
    {
        nodes = new List<BG_Node>();
    }

    public BG_Node AddNode(int value, BG_Node neighbour = null)
    {
        BG_Node tempNode = new BG_Node(value);
        nodes.Add(tempNode);
        tempNode.AddEdge(tempNode, neighbour);
        return tempNode;
    }

    public void RemoveNode(BG_Node toBeRemoved)
    {
        nodes.Remove(toBeRemoved);
        foreach (BG_Node node in nodes)
        {
            if (node == toBeRemoved)
            {
                foreach (BG_Edge edge in node.edges)
                {
                    edge.neighbour_b.RemoveEdge(node, edge.neighbour_b);
                    edge.neighbour_a.RemoveEdge(node, edge.neighbour_a);
                }
                node.edges.Clear();
                nodes.Remove(node);
            }
        }
    }

    public void PrintGraph(BG_Node node, Order order) 
    {
        if (order == Order.DFS)
        {
            if (!checkedNodes.Contains(node))
            {
                checkedNodes.Add(node);

                if (node.edges[0].neighbour_a == node && node.edges[0].passable_ab)
                    PrintGraph(node.edges[0].neighbour_b, order);
                else if (node.edges[0].neighbour_b == node && node.edges[0].passable_ba)
                    PrintGraph(node.edges[0].neighbour_a, order);

                steps++;
            }
            else if (steps == checkedNodes.Count - 1)
            {
                foreach (BG_Node currNode in checkedNodes)
                {
                    Debug.Log($"-> {currNode.value}");
                }
                Debug.Log($"Steps: {steps}, Count: {checkedNodes.Count}");
                steps = 0;
                checkedNodes.Clear();
            }
        }
        else if (order == Order.BFS)
        {
            if (!checkedNodes.Contains(node))
            {
                checkedNodes.Add(node);

                foreach (BG_Edge edge in node.edges)
                {
                    if (edge.neighbour_a == node && edge.passable_ab)
                        PrintGraph(edge.neighbour_b, order);
                    else if (edge.neighbour_b == node && edge.passable_ba)
                        PrintGraph(edge.neighbour_a, order);
                }

                steps++;
            }
            else if (steps == checkedNodes.Count - 1)
            {
                foreach (BG_Node currNode in checkedNodes)
                {
                    Debug.Log($"-> {currNode.value}");
                }
                Debug.Log($"Steps: {steps}, Count: {checkedNodes.Count}");
                steps = 0;
                checkedNodes.Clear();
            }
        }
    }
}

public enum Order 
{
    BFS,
    DFS
}
