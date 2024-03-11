using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph_Test : MonoBehaviour
{
    private void Start()
    {
        Graph myGraph = new Graph();

        myGraph.AddNode(1);
        myGraph.AddNode(2);
        myGraph.AddNode(3);
        myGraph.AddNode(4);
        myGraph.AddNode(5);
        myGraph.AddNode(6);

        myGraph.AddEdge(1, 2, 4f);
        myGraph.AddEdge(1, 3, 4f);
        myGraph.AddEdge(2, 3, 2f);
        myGraph.AddEdge(3, 4, 3f);
        myGraph.AddEdge(3, 5, 1f);
        myGraph.AddEdge(3, 6, 6f);
        myGraph.AddEdge(4, 6, 2.5f);
        myGraph.AddEdge(5, 6, 3f);

        Debug.Log("BFS Print:");
        myGraph.PrintGraph(1, Order.BFS);

        Debug.Log("DFS Print:");
        myGraph.PrintGraph(1, Order.DFS);

        myGraph.RemoveNode(5);
        Debug.Log("After removing node 5:");

        Debug.Log("BFS Print:");
        myGraph.PrintGraph(1, Order.BFS);

        Debug.Log("DFS Print:");
        myGraph.PrintGraph(1, Order.DFS);

        Debug.Log("Dijkstra from node 1 to node 6:");
        List<Node> path = myGraph.Dijkstra(1, 6);
        foreach (Node node in path)
        {
            Debug.Log($"Walked node {node.Value}");
        }

        Debug.Log($"Distance traveled: {path[path.Count - 1].DistanceFromSource}");
    }

    //[SerializeField] List<Node> nodes;

    //private void Start()
    //{
    //    Graph graph = new Graph();
    //}
}
