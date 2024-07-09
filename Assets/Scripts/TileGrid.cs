using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;

public class TileGrid : Singleton<TileGrid>
{
    #region Variables & Properties

    #region Local
    Graph mapGrid = new Graph();
    List<Node> highlightedNodes = new List<Node>();

    [SerializeField] private LayerMask sampleMask;

    [SerializeField] Camera mainCamera;
    [SerializeField] float cameraFollowDuration;
    [SerializeField] Character selectedCharacter;
    [SerializeField] float stepHeight;
    [SerializeField] float range;

    Node currentHighlightedNode;
    #endregion

    #region Public
    public Graph MapGrid { get { return mapGrid; } }
    #endregion

    #endregion

    #region Monobehaviour
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        currentHighlightedNode = null;

        EventsManager.AllNodesRegistered.Invoke();

        //Node[] path = grid.GetPath(start, end, 0, stepHeight);

        //Node[] path = grid.GetPath(start, end, 0, stepHeight);

        //for (int i = 0; i < path.Length; i++)
        //{
        //    Debug.DrawLine(path[i].transform.position, path[i].transform.position + Vector3.up * 5f, Color.red, 10f);
        //    yield return new WaitForSeconds(0.5f);
        //}
    }

    private void Update()
    {
        CheckHighlightedNode();

        if (Input.GetMouseButtonDown(0) && currentHighlightedNode != null)
        {
            Node currStart = selectedCharacter.CurrentOccupiedNode;

            //Debug.Log("start: " + currStart.name + " currHighlight: " + currentHighlightedNode.name);

            EventsManager.NodeClicked(mapGrid.GetPath(currStart, currentHighlightedNode, 0, stepHeight));
        }

        if (selectedCharacter != null)
            CameraFollow();
    }
    #endregion

    #region Methods
    public void RegisterTile(Node tile)
    {
        mapGrid.AddTile(tile);
    }

    private void CheckHighlightedNode() 
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, sampleMask))
        {
            Node hitNode = hit.collider.GetComponent<Node>();

            if (selectedCharacter.HighlightedNodes.Contains(hitNode))
            {
                if (currentHighlightedNode == null)
                {
                    currentHighlightedNode = hitNode;
                    currentHighlightedNode.HighlightNode(Color.red);
                }

                if (currentHighlightedNode != hitNode)
                {
                    currentHighlightedNode.HighlightNode(Color.cyan);
                    currentHighlightedNode = hitNode;
                    hitNode.HighlightNode(Color.red);
                }
                //currentHighlightedNode.HighlightNode(Color.cyan);
            }

            //if (currentHighlightedNode != null)
            //{
            //    if(currentHighlightedNode == hitNode)
            //        return;
            //    else
            //        currentHighlightedNode.LoseHighlight();
            //}

            //currentHighlightedNode = hitNode;
            //currentHighlightedNode.HighlightNode();
            //EventsManager.NodeClicked.Invoke(hit.collider.GetComponent<Node>());
        }
        else 
        {
            if (currentHighlightedNode == null)
                return;
            else
            {
                currentHighlightedNode.HighlightNode(Color.cyan);
                currentHighlightedNode = null;
            }
        }
        //Debug.DrawLine(ray.origin, ray.direction * 50f, Color.red, 50f);
    }

    private void CameraFollow()
    {
        Camera.main.transform.DOMove(selectedCharacter.transform.position + new Vector3(15f, 13f, 15f), cameraFollowDuration).SetEase(Ease.InOutQuad);
    }

    public Node[] GetAreaUtility(Node start, float range, float stepHeight)
    {
        return mapGrid.GetArea(start, range, 0, stepHeight);
    }
    #endregion
}
