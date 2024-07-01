using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : IComparable<Edge>
{
    Node tile_A;
    Node tile_B;
    float weight;
    float stepHeight;
    //b-a direction check generate on start

    ////Tile (Monobehaviour)
    //[SerializeField] private Node start;
    //[SerializeField] private Node end;

    public float Weight => weight;
    public float StepHeight => stepHeight;

    public Edge(Node tile_A, Node tile_B, float weight, float stepHeight)
    {
        if(tile_A == null || tile_B == null)
            return;

        if(tile_A == tile_B)
            return;

        this.tile_A = tile_A;
        this.tile_B = tile_B;
        this.weight = weight;
        this.stepHeight = stepHeight;
    }

    public void SetWeight(float weight)
    {
        this.weight = weight;
    }

    public void SetStepHeight(float stepHeight)
    {
        this.stepHeight = stepHeight;
    }

    public Node GetNeighbor(Node tile)
    {
        if(tile == tile_A)
            return tile_B;
        else if(tile == tile_B)
            return tile_A;
        
        return null;
    }

    public int CompareTo(Edge other)
    {
        return weight > other.weight ? 1 : -1;
    }
}
