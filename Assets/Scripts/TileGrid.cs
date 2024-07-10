using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using static UnityEngine.UI.ContentSizeFitter;

public class TileGrid : Singleton<TileGrid>
{
    #region Variables & Properties

    #region Local
    Graph mapGrid = new Graph();
    List<Node> highlightedNodes = new List<Node>();

    [SerializeField] private LayerMask sampleMask;

    [SerializeField] Camera mainCamera;
    [SerializeField] float cameraFollowDuration;
    [SerializeField] Character[] availableCharacters;
    Character selectedCharacter;
    int characterIndex;
    //[SerializeField] float stepHeight;
    //[SerializeField] float range;

    //[SerializeField] Node testStart;
    //[SerializeField] Node testEnd;

    Node currentHighlightedNode;
    #endregion

    #region Public
    public Graph MapGrid { get { return mapGrid; } }
    public Character SelectedCharacter { get { return selectedCharacter; } }
    #endregion

    #endregion

    #region Monobehaviour
    private IEnumerator Start()
    {
        selectedCharacter = availableCharacters[0];
        characterIndex = 0;

        yield return new WaitForSeconds(0.5f);

        currentHighlightedNode = null;

        EventsManager.AllNodesRegistered.Invoke();

        //Node[] path = mapGrid.GetPath(testStart, testEnd, 0, stepHeight);

        //for (int i = 0; i < path.Length; i++)
        //{
        //    Debug.DrawLine(path[i].transform.position, path[i].transform.position + Vector3.up * 5f, Color.red, 10f);
        //    yield return new WaitForSeconds(0.5f);
        //}
    }

    private void Update()
    {
        if(!selectedCharacter.Walking)
            CheckHighlightedNode();

        if (Input.GetMouseButtonDown(0) && currentHighlightedNode != null)
        {
            selectedCharacter.ClearHighlightedNodes();
            Node currStart = selectedCharacter.CurrentOccupiedNode;
            //Debug.Log("start: " + currStart.name + " currHighlight: " + currentHighlightedNode.name);
            EventsManager.NodeClicked(mapGrid.GetPath(currStart, currentHighlightedNode, 0, selectedCharacter.StepHeight));
            currentHighlightedNode = null;
        }

        if (selectedCharacter.ChoosingDirection)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                EventsManager.DirectionChosen.Invoke(CharacterDirection.FRONT);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                EventsManager.DirectionChosen.Invoke(CharacterDirection.BACK);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                EventsManager.DirectionChosen.Invoke(CharacterDirection.RIGHT);
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                EventsManager.DirectionChosen.Invoke(CharacterDirection.LEFT);
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
        Node hitNode = null;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, sampleMask))
            hitNode = hit.collider.GetComponent<Node>();

        if (hitNode != null && selectedCharacter.HighlightedNodes.Contains(hitNode))
        {
            if (currentHighlightedNode == null)
            {
                currentHighlightedNode = hitNode;
                currentHighlightedNode.HighlightNode(Color.red);
            }
            else
            {
                currentHighlightedNode.HighlightNode(Color.cyan);
                currentHighlightedNode = hitNode;
                hitNode.HighlightNode(Color.red);
            }
        }
        else if(currentHighlightedNode != null && selectedCharacter.HighlightedNodes.Contains(currentHighlightedNode))
        {
            currentHighlightedNode.HighlightNode(Color.cyan);
            currentHighlightedNode = null;
        }
    }

    private void CameraFollow()
    {
        Camera.main.transform.DOMove(selectedCharacter.transform.position + new Vector3(15f, 13f, 15f), cameraFollowDuration);
    }

    public Node[] GetAreaUtility(Node start, float range, float stepHeight)
    {
        return mapGrid.GetArea(start, range, 0, stepHeight);
    }

    public void PassTurn()
    {
        characterIndex++;

        if (characterIndex >= availableCharacters.Length)
            characterIndex = 0;

        selectedCharacter = availableCharacters[characterIndex];
        EventsManager.TurnChanged.Invoke();
    }
    #endregion
}
