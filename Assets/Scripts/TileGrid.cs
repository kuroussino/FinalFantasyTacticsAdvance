using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;

public class TileGrid : Singleton<TileGrid>
{
    #region Variables & Properties

    #region Local
    Graph grid = new Graph();
    List<Node> highlightedNodes = new List<Node>();

    [SerializeField] private LayerMask sampleMask;

    [SerializeField] Camera mainCamera;
    [SerializeField] float cameraFollowDuration;
    [SerializeField] Character selectedCharacter;
    [SerializeField] Node start;
    Node end;
    [SerializeField] float stepHeight;
    [SerializeField] float range;

    Node currentHighlightedNode;
    #endregion

    #region Public
    #endregion

    #endregion

    #region Monobehaviour
    //private IEnumerator Start()
    //{
    //    yield return new WaitForSeconds(1f);

    //    if (Physics.Raycast(selectedCharacter.gameObject.transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, sampleMask))
    //    {
    //        Debug.Log("Zio Pera");
    //        start = hit.collider.GetComponent<Node>();
    //    }
    //    Debug.DrawRay(selectedCharacter.transform.position, Vector3.down, Color.black, 10f);

    //    //Node[] path = grid.GetPath(start, end, 0, stepHeight);

    //    //Node[] path = grid.GetPath(start, end, 0, stepHeight);

    //    //for (int i = 0; i < path.Length; i++)
    //    //{
    //    //    Debug.DrawLine(path[i].transform.position, path[i].transform.position + Vector3.up * 5f, Color.red, 10f);
    //    //    yield return new WaitForSeconds(0.5f);
    //    //}
    //}

    private void Update()
    {
        CheckHighlightedNode();

        if (Input.GetMouseButtonDown(0) && currentHighlightedNode != null)
        {
            if(Physics.Raycast(selectedCharacter.transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, sampleMask))
                start = hit.collider.GetComponent<Node>();

            Debug.Log("start: " + start.name + " currHighlight: " + currentHighlightedNode.name);

            EventsManager.NodeClicked(grid.GetPath(start, currentHighlightedNode, 0, 1));

            start = currentHighlightedNode;
        }

        if (selectedCharacter != null)
            CameraFollow();
    }
    #endregion

    #region Methods
    public void RegisterTile(Node tile)
    {
        grid.AddTile(tile);
    }

    private void CheckHighlightedNode() 
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, sampleMask))
        {
            Node hitNode = hit.collider.GetComponent<Node>();

            if (currentHighlightedNode != null)
            {
                if(currentHighlightedNode == hitNode)
                    return;
                else
                    currentHighlightedNode.LoseHighlight();
            }

            currentHighlightedNode = hitNode;
            currentHighlightedNode.HighlightNode();
            //EventsManager.NodeClicked.Invoke(hit.collider.GetComponent<Node>());
        }
        else 
        {
            if (currentHighlightedNode == null)
                return;
            else
                currentHighlightedNode.LoseHighlight();
            currentHighlightedNode = null; 
        }
        //Debug.DrawLine(ray.origin, ray.direction * 50f, Color.red, 50f);
    }

    private void CameraFollow()
    {
        Camera.main.transform.DOMove(selectedCharacter.transform.position + new Vector3(20f, 20f, 20f), cameraFollowDuration).SetEase(Ease.InOutQuad);
    }
    #endregion
}
