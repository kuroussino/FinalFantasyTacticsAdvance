using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Edge
{
    public BG_Node neighbour_a;
    public BG_Node neighbour_b;
    public bool passable_ab;
    public bool passable_ba;

    public BG_Edge(BG_Node neighbour_a, BG_Node neighbour_b, bool ab = true, bool ba = true)
    {
        this.neighbour_a = neighbour_a;
        this.neighbour_b = neighbour_b;
        passable_ab = ab;
        passable_ba = ba;
    }
}
