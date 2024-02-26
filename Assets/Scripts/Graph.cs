using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Graph
{
    private Dictionary<int, Node> nodes = new Dictionary<int, Node>();
    public int Count { get { return nodes.Count; } }
    
    //public Dictionary<int, Node> Nodes { get { return nodes; } }

    public void AddNode(int nodeValue)
    {
        if (!nodes.ContainsKey(nodeValue))
        {
            Node newNode = new Node(nodeValue);
            nodes[nodeValue] = newNode;
        }
        else { Debug.LogWarning($"A node with value {nodeValue} aready exists in the graph"); }
    }

    public void RemoveNode(int nodeValue) 
    {
        if (nodes.ContainsKey(nodeValue))
        {
            Node nodeToRemove = nodes[nodeValue];

            foreach (Node otherNode in nodes.Values)
            {
                otherNode.RemoveEdge(nodeToRemove);
            }

            nodes.Remove(nodeValue);
        }
        else { Debug.LogWarning($"A node with value {nodeValue} does not exist in the graph"); }
    }

    public void AddEdge(int nodeValue_a, int nodeValue_b, float lenght, bool isBidirectional = true)
    {
        if(nodes.ContainsKey(nodeValue_a) && nodes.ContainsKey(nodeValue_b)) 
        {
            nodes[nodeValue_a].AddEdge(nodes[nodeValue_b], lenght, isBidirectional);
        }
        else { Debug.LogWarning("One or both nodes does not exist in the graph"); }
    }

    public void RemoveEdge(int nodeValue_a, int nodeValue_b)
    {
        if (nodes.ContainsKey(nodeValue_b) && nodes.ContainsKey(nodeValue_a))
        {
            nodes[nodeValue_a].RemoveEdge(nodes[nodeValue_b]);
        }
        else { Debug.LogWarning($"One or both nodes does not exist in the graph"); }
    }

    public void PrintGraph(int startNodeValue, Order printOrder = Order.BFS)
    {
        if (printOrder == Order.BFS)
        {
            Node startNode = nodes[startNodeValue];
            Queue<Node> queue = new Queue<Node>();
            HashSet<Node> visited = new HashSet<Node>();

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0) 
            {
                Node currentNode = queue.Dequeue();
                Debug.Log($"Visited Node {currentNode.Value}");

                foreach (Edge edge in currentNode.Edges)
                {
                    if (!visited.Contains(edge.ConnectedNode))
                    {
                        queue.Enqueue(edge.ConnectedNode);
                        visited.Add(edge.ConnectedNode);
                    }
                }
            }
        }
        else if (printOrder == Order.DFS)
        {
            Node startNode = nodes[startNodeValue];
            HashSet<Node> visited = new HashSet<Node>();

            DFS_Recursive(startNode, visited);
        }
    }

    private void DFS_Recursive(Node currentNode, HashSet<Node> visited)
    {
        visited.Add(currentNode);
        Debug.Log($"Visited Node {currentNode.Value}");

        foreach (Edge edge in currentNode.Edges)
        {
            if(!visited.Contains(edge.ConnectedNode)) DFS_Recursive(edge.ConnectedNode, visited);
        }
    }

    public List<Node> Dijkstra(int startNodeValue, int endNodeValue)
    {
        if (!nodes.ContainsKey(startNodeValue) || !nodes.ContainsKey(endNodeValue))
        {
            Debug.Log("Start, end or both nodes does not exist in the graph");
            return null;
        }

        Node startNode = nodes[startNodeValue];

        foreach (Node node in nodes.Values)
        {
            node.DistanceFromSource = float.MaxValue;
            node.PreviousNode = null;
        }

        startNode.DistanceFromSource = 0f;

        PriorityQueue priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue(startNode, 0);

        while (priorityQueue.Count > 0)
        {
            Node currentNode = priorityQueue.Dequeue();

            if (currentNode.Value == endNodeValue)
            {
                List<Node> shortestPath = new List<Node>();
                Node node = currentNode;

                while (node != null)
                {
                    shortestPath.Insert(0, node);
                    node = node.PreviousNode;
                }

                return shortestPath;
            }

            foreach (var edge in currentNode.Edges)
            {
                float newDistance = currentNode.DistanceFromSource + edge.Weight;

                if (newDistance < edge.ConnectedNode.DistanceFromSource)
                {
                    edge.ConnectedNode.DistanceFromSource = newDistance;
                    edge.ConnectedNode.PreviousNode = currentNode;

                    priorityQueue.Enqueue(edge.ConnectedNode, newDistance);
                }
            }
        }
        return null;
    }

    public List<Node> A_Star(int startNodeValue, int endNodeValue)
    {
        if (!nodes.ContainsKey(startNodeValue) || !nodes.ContainsKey(endNodeValue))
        {
            Debug.Log("Start, end or both nodes does not exist in the graph");
            return null;
        }

        Node startNode = nodes[startNodeValue];       
        Node endNode = nodes[endNodeValue];

        foreach (Node node in nodes.Values)
        {
            node.DistanceFromSource = float.MaxValue;
            //node.HeuristicValue = Utils.ManhattanDistance();
            node.PreviousNode = null;
        }

        startNode.DistanceFromSource = 0f;
        endNode.HeuristicValue = 0f;

        PriorityQueue priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue(startNode, 0);

        while (priorityQueue.Count > 0)
        {
            //you can initialize heuristic there or in the foreach, it depends
            Node currentNode = priorityQueue.Dequeue();

            if (currentNode.Value == endNodeValue)
            {
                List<Node> shortestPath = new List<Node>();
                Node node = currentNode;

                while (node != null)
                {
                    shortestPath.Insert(0, node);
                    node = node.PreviousNode;
                }

                return shortestPath;
            }

            foreach (var edge in currentNode.Edges)
            {
                float newDistance = currentNode.DistanceFromSource + edge.Weight; //per ff si aggiunge il movement cost dipendentemente dal tipo di terreno che vogliamo attraversare
                //currentNode.HeuristicValue = Utils.ManhattanDistance(); --> float newHeuristic

                if (newDistance < edge.ConnectedNode.DistanceFromSource)
                {
                    edge.ConnectedNode.DistanceFromSource = newDistance;
                    //edge.ConnectedNode.HeuristicValue += newHeuristic;
                    edge.ConnectedNode.PreviousNode = currentNode;

                    //priorityQueue.Enqueue(edge.ConnectedNode, newDistance + newHeuristic);
                }
            }
        }
        return null;
    }
}

public class PriorityQueue
{
    private SortedDictionary<float, Queue<Node>> elements = new SortedDictionary<float, Queue<Node>>();

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(Node item, float priority)
    {
        if (!elements.ContainsKey(priority))
        {
            elements[priority] = new Queue<Node>();
        }

        elements[priority].Enqueue(item);
    }

    public Node Dequeue()
    {
        if (elements.Count == 0)
        {
            throw new InvalidOperationException("Priority queue is empty");
        }

        KeyValuePair<float, Queue<Node>> queue = elements.First();
        Node item = queue.Value.Dequeue();

        if (queue.Value.Count == 0)
        {
            elements.Remove(queue.Key);
        }

        return item;
    }
}

public class Utils
{
    ///<summary>
    ///Calculates the Manhattan distance between the two points.
    ///</summary>
    ///<param name="x1">The first x coordinate.</param>
    ///<param name="x2">The second x coordinate.</param>
    ///<param name="y1">The first y coordinate.</param>
    ///<param name="y2">The second y coordinate</param>
    public static float ManhattanDistance(float x1, float x2, float y1, float y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }

    ///<summary>
    ///Calculates the Manhattan distance between the two points.
    ///</summary>
    ///<param name="x1">The first x coordinate.</param>
    ///<param name="x2">The second x coordinate.</param>
    ///<param name="y1">The first y coordinate.</param>
    ///<param name="y2">The second y coordinate</param>
    public static int ManhattanDistance(int x1, int x2, int y1, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }
}

public enum Order 
{
    BFS,
    DFS
}
