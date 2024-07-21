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
    //List<Node> highlightedNodes = new List<Node>();

    [SerializeField] Color unselectedTileColor;
    [SerializeField] Color selectedTileColor;
    [SerializeField] private LayerMask sampleMask;
    [SerializeField] private LayerMask characterMask;
    [SerializeField] Camera mainCamera;
    [SerializeField] float cameraFollowDuration;
    [SerializeField] Character[] availableCharacters;
    Character selectedCharacter;
    int characterIndex;

    public TurnState turnState;

    //[SerializeField] Node testStart;
    //[SerializeField] Node testEnd;

    public NodeB currentHighlightedNode;
    #endregion

    #region Public
    public Graph MapGrid { get { return mapGrid; } }
    public Character SelectedCharacter { get { return selectedCharacter; } }
    public Color UnselectedTileColor { get {  return unselectedTileColor; } }
    public Color SelectedTileColor { get { return selectedTileColor; } }

    //public Node CurrentHighlightedNode { get {  return currentHighlightedNode; } set { currentHighlightedNode = CurrentHighlightedNode; } }
    #endregion

    #endregion

    #region Monobehaviour
    private IEnumerator Start()
    {
        selectedCharacter = availableCharacters[0];
        characterIndex = 0;

        yield return new WaitForSeconds(1f);

        currentHighlightedNode = null;

        EventsManager.AllNodesRegistered.Invoke();

        yield return new WaitForSeconds(0.1f);

        EventsManager.AllNodesRegisteredP2.Invoke();

        turnState = TurnState.CHOOSING;

        //Node[] path = mapGrid.GetPath(testStart, testEnd, 0, stepHeight);

        //for (int i = 0; i < path.Length; i++)
        //{
        //    Debug.DrawLine(path[i].transform.position, path[i].transform.position + Vector3.up * 5f, Color.red, 10f);
        //    yield return new WaitForSeconds(0.5f);
        //}
    }

    private void Update()
    {
        //if(!selectedCharacter.IsWalking)
        //    CheckHighlightedNode();

        NodeSelector();

        if (Input.GetKeyDown(KeyCode.Z) && currentHighlightedNode != null && selectedCharacter.HighlightedNodes.Contains(currentHighlightedNode))
        {
            //selectedCharacter.CurrentOccupiedNode.UnpointNode();
            currentHighlightedNode.UnpointNode();
            selectedCharacter.ClearHighlightedNodes();
            NodeB currStart = selectedCharacter.CurrentOccupiedNode;
            //Debug.Log("start: " + currStart.name + " currHighlight: " + currentHighlightedNode.name);
            EventsManager.NodeClicked(mapGrid.GetPath(currStart, currentHighlightedNode, selectedCharacter.Team, selectedCharacter.Jump));
            currentHighlightedNode = null;
        }

        if (selectedCharacter.IsChoosingDirection)
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

        if (selectedCharacter != null || currentHighlightedNode != null)
            CameraFollow();
    }
    #endregion

    #region Methods
    public void RegisterTile(NodeB tile)
    {
        mapGrid.AddTile(tile);
    }

    private void CheckHighlightedNode() 
    {
        NodeB hitNode = null;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, sampleMask))
            hitNode = hit.collider.GetComponent<NodeB>();

        if (hitNode != null && selectedCharacter.HighlightedNodes.Contains(hitNode))
        {
            if (currentHighlightedNode == null)
            {
                currentHighlightedNode = hitNode;
                currentHighlightedNode.HighlightNode(selectedTileColor);
            }
            else
            {
                currentHighlightedNode.HighlightNode(unselectedTileColor);
                currentHighlightedNode = hitNode;
                hitNode.HighlightNode(selectedTileColor);
            }
        }
        else if(currentHighlightedNode != null && selectedCharacter.HighlightedNodes.Contains(currentHighlightedNode))
        {
            currentHighlightedNode.HighlightNode(unselectedTileColor);
            currentHighlightedNode = null;
        }
    }

    private void NodeSelector()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            UpdatePointedNode(Vector3.back);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            UpdatePointedNode(Vector3.forward);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            UpdatePointedNode(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            UpdatePointedNode(Vector3.left);
        }
    }

    private void UpdatePointedNode(Vector3 dir)
    {
        NodeB oldNode = currentHighlightedNode;

        Debug.DrawRay(currentHighlightedNode.transform.position + dir * 2f + Vector3.up, Vector3.down, Color.red, 5f);

        if (Physics.Raycast(currentHighlightedNode.transform.position + dir * 2f + Vector3.up, Vector3.down, out RaycastHit hit, float.MaxValue, sampleMask))
        {
            if (hit.collider.GetComponent<NodeB>() == currentHighlightedNode)
                return;

            currentHighlightedNode.UnpointNode();
            currentHighlightedNode = hit.collider.GetComponent<NodeB>();
            currentHighlightedNode.PointNode();
        }
        else if (Physics.Raycast(currentHighlightedNode.transform.position + dir * 2f + Vector3.down, Vector3.up, out RaycastHit hit0, float.MaxValue, sampleMask))
        {
            if (hit0.collider.GetComponent<NodeB>() == currentHighlightedNode)
                return;

            currentHighlightedNode.UnpointNode();
            currentHighlightedNode = hit0.collider.GetComponent<NodeB>();
            currentHighlightedNode.PointNode();
        }

        if (oldNode != currentHighlightedNode)
        {
            Character hitCharacter = null;
            if (Physics.Raycast(currentHighlightedNode.transform.position, Vector3.up, out RaycastHit hit1, float.MaxValue, characterMask))
            {
                hitCharacter = hit1.collider.GetComponent<Character>();
            }

            if (hitCharacter != null)
            {
                if(hitCharacter != selectedCharacter)
                    EventsManager.ShowTargetUI(hitCharacter);
                else
                    EventsManager.ShowControlledCharacterUI(selectedCharacter);
            }
            else
                EventsManager.HideAllUI();
        }
    }

    private void CameraFollow()
    {
        if (turnState == TurnState.CHOOSING && currentHighlightedNode != null)
        {
            Camera.main.transform.DOMove(currentHighlightedNode.transform.position + new Vector3(15f, 12f, 15f), cameraFollowDuration);
        }
        else
        {
            Camera.main.transform.DOMove(selectedCharacter.transform.position + new Vector3(15f, 12f, 15f), cameraFollowDuration);
        }
    }

    public NodeB[] GetAreaUtility(NodeB start, float range, int teamId, float stepHeight, bool includeStart, bool includeAllies, bool includeEnemies, AreaMode mode)
    {
        return mapGrid.GetArea(start, range, teamId, stepHeight, includeStart, includeAllies, includeEnemies, mode);
    }

    public void PassTurn()
    {
        characterIndex++;

        if (characterIndex >= availableCharacters.Length)
        {
            Debug.Log("Returned to character 0");
            characterIndex = 0;
        }

        selectedCharacter = availableCharacters[characterIndex];
        Debug.Log("Character changed to: " + selectedCharacter.name);
        EventsManager.TurnChanged.Invoke();
    }
    #endregion
}
