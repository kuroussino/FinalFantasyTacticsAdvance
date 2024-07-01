using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : Singleton<TileGrid>
{
    #region Variables & Properties

    #region Local
    Graph grid = new Graph();
    List<Node> highlightedNodes = new List<Node>();

    [SerializeField] private LayerMask sampleMask;

    [SerializeField] Node start;
    [SerializeField] Node end;
    [SerializeField] float stepHeight;
    [SerializeField] float range;
    #endregion

    #region Public
    #endregion

    #endregion

    #region Monobehaviour
    private IEnumerator Start()
    {
        yield return null;

        //Node[] path = grid.GetPath(start, end, 0, stepHeight);

        Node[] path = grid.GetPath(start, end, 0, stepHeight);

        for (int i = 0; i < path.Length; i++) 
        {
            Debug.DrawLine(path[i].transform.position, path[i].transform.position + Vector3.up * 5f, Color.red, 10f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    #endregion

    #region Methods

    public void RegisterTile(Node tile)
    {
        grid.AddTile(tile);
    }
    #endregion
}
