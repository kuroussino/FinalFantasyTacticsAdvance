using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphTest : MonoBehaviour
{
    private void Start()
    {
        BinaryGraph testGraph = new BinaryGraph();
        BG_Node node_1 = testGraph.AddNode(2);
        BG_Node node_2 = testGraph.AddNode(12, node_1);
        BG_Node node_3 = testGraph.AddNode(1, node_2);
        BG_Node node_4 = testGraph.AddNode(7, node_3);
        BG_Node node_5 = testGraph.AddNode(5, node_4);
        
        node_1.edges.Clear();
        node_1.AddEdge(node_1, node_2);
        node_5.AddEdge(node_5, node_3);
        node_5.AddEdge(node_5, node_2);
        node_5.AddEdge(node_5, node_1);

        testGraph.PrintGraph(node_1, Order.BFS);
    }
}
