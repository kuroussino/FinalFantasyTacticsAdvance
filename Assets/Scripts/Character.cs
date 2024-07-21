using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class Character : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [Header("Animations")]
    [SerializeField] AnimationCurve walkCurve;
    [SerializeField] AnimationCurve jumpUpCurve;
    [SerializeField] AnimationCurve jumpDownCurve;
    [SerializeField] AnimationCurve dodgingCurve;
    [SerializeField] float walkAnimationSpeed;
    SpriteManager spriteManager;
    float jumpDuration;
    float pathIterationWait;

    [Header("Pathfinding")]
    [SerializeField] LayerMask sampleMask;
    [SerializeField] LayerMask characterMask;
    List<NodeB> highlightedNodes = new List<NodeB>();
    NodeB currentOccupiedNode;
    NodeB currentTargetNode;

    [Header("Gameplay")]
    [SerializeField] CharacterData characterData;
    [SerializeField] CharacterDirection startDirection;
    [SerializeField] int team;
    [SerializeField] GameObject directionChooseMenu;
    [SerializeField] Color deadColor;
    bool isDead;
    CharacterDirection currentDirection;
    bool isWalking;
    bool isChoosingDirection;
    int actionsLeft;
    bool moved;
    bool attacked;

    [Header("Combat Stats")]
    int currentHp;
    int currentMp;
    #endregion

    #region Public
    public CharacterData CharacterData { get { return characterData; } }
    public NodeB CurrentOccupiedNode { get { return currentOccupiedNode; } }
    public List<NodeB> HighlightedNodes { get { return highlightedNodes; } }
    public bool IsWalking { get { return isWalking; } }
    public float Jump { get { return characterData.Jump; } }
    public bool IsChoosingDirection {  get { return isChoosingDirection; } }
    public int Team { get { return team; } }
    public int CurrentHp { get { return currentHp; } }
    public int CurentMp { get { return currentMp; } }
    public bool IsDead => isDead;
    #endregion

    #endregion

    #region Monobehaviour
    private void Awake()
    {
        spriteManager = GetComponent<SpriteManager>();    
    }

    private void Start()
    {
        switch (startDirection)
        {
            case CharacterDirection.FRONT:
                UpdateDirection(Vector3.forward);
                break;
            case CharacterDirection.RIGHT:
                UpdateDirection(Vector3.right);
                break;
            case CharacterDirection.LEFT:
                UpdateDirection(Vector3.left);
                break;
            case CharacterDirection.BACK:
                UpdateDirection(Vector3.back);
                break;
        }
    }

    private void OnEnable()
    {
        EventsManager.NodeClicked += MoveTo;
        EventsManager.AllNodesRegistered += GetCurrentOccupiedNode;
        //EventsManager.AllNodesRegisteredP2 += ShowWalkArea;
        EventsManager.AllNodesRegisteredP2 += SetSelfOccupationId;
        EventsManager.AllNodesRegisteredP2 += InitializeCharacterStats;
        EventsManager.AllNodesRegisteredP2 += StartTurn;
        EventsManager.TurnChanged += GetCurrentOccupiedNode;
        EventsManager.DirectionChosen += EndTurnDirectionChange;

        EventsManager.TurnChanged += StartTurn;
        EventsManager.MoveClicked += ShowWalkArea;
        EventsManager.FightClicked += ShowFightArea;
        EventsManager.TargetChosen += AttackTarget;
    }

    private void OnDisable()
    {
        EventsManager.NodeClicked -= MoveTo;
        EventsManager.AllNodesRegistered -= GetCurrentOccupiedNode;
        //EventsManager.AllNodesRegisteredP2 -= ShowWalkArea;
        EventsManager.AllNodesRegisteredP2 -= SetSelfOccupationId;
        EventsManager.AllNodesRegisteredP2 -= InitializeCharacterStats;
        EventsManager.AllNodesRegisteredP2 -= StartTurn;
        EventsManager.TurnChanged -= GetCurrentOccupiedNode;
        EventsManager.DirectionChosen -= EndTurnDirectionChange;

        EventsManager.TurnChanged -= StartTurn;
        EventsManager.MoveClicked -= ShowWalkArea;
        EventsManager.FightClicked -= ShowFightArea;
        EventsManager.TargetChosen -= AttackTarget;
        //EventsManager.MoveClicked -= UseAction;
    }
    #endregion

    #region Methods
    private void MoveTo(NodeB[] path)
    {
        if(TileGrid.Instance.SelectedCharacter != this)
            return;

        //Debug.Log("Clicked node named: " + path[path.Length-1].name);
        if (isWalking)
        {
            Debug.LogWarning("Character still walking!");
            return;
        }

        if (path == null)
        {
            Debug.LogWarning("Path[] is null!");
            return;
        }

        Debug.Log("team: " + team);
        UpdateDirection((path[1].transform.position - transform.position).normalized);
        currentTargetNode = path[path.Length - 1];
        currentOccupiedNode.SetOccupationId(-1);
        StartCoroutine(PathWalkCoroutine(path));
    }

    IEnumerator PathWalkCoroutine(NodeB[] path)
    {
        isWalking = true;
        moved = true;
        TileGrid.Instance.turnState = TurnState.MOVING;
        HighlightTargetNode();

        UseAction();

        for (int i = 1; i < path.Length; i++) 
        {
            UpdateDirection((path[i].transform.position - transform.position).normalized);

            yield return new WaitForEndOfFrame();

            if (path[i] != path[path.Length - 1])
                CheckIfNextTileOccupied(path[i - 1], path[i], path[i + 1]);

            //path[i].GetComponent<Node>().SetOccupationId(-1);

            if (path[i].transform.position.y != transform.position.y)
            {
                int jumpSteps = (int)Mathf.Abs(path[i].transform.position.y - transform.position.y);

                if (characterData.Jump == jumpSteps)
                    jumpSteps--;

                jumpDuration = jumpSteps >= characterData.Jump - 2  && jumpSteps > 2 ? ((walkAnimationSpeed / 2f) * characterData.Jump) / (characterData.Jump - jumpSteps) : walkAnimationSpeed;

                bool jumpDir = transform.position.y < path[i].transform.position.y;

                Vector3 origin = transform.position;
                Vector3 target = path[i].transform.position;

                float timer = 0;
                while (timer <= jumpDuration)
                {
                    timer += Time.deltaTime;
                    float percent = Mathf.Clamp01(timer / jumpDuration);
                    float curvePercentX = walkCurve.Evaluate(percent);
                    float curvePercentY = jumpDir ? jumpUpCurve.Evaluate(percent) : jumpDownCurve.Evaluate(percent);
                    float curvePercentZ = walkCurve.Evaluate(percent);
                    transform.position = new Vector3(Mathf.LerpUnclamped(origin.x, target.x, curvePercentX), Mathf.LerpUnclamped(origin.y, target.y, curvePercentY), Mathf.LerpUnclamped(origin.z, target.z, curvePercentZ));
                    yield return null;
                }

                pathIterationWait = 0f;
            }
            else
            {
                transform.DOMove(path[i].transform.position, walkAnimationSpeed).SetEase(walkCurve);
                pathIterationWait = walkAnimationSpeed;
            }

            yield return new WaitForSeconds(pathIterationWait);

            transform.position = path[i].transform.position;

            UpdateDirection((path[i].transform.position - transform.position).normalized);
        }

        yield return null;
        transform.position = path[path.Length - 1].transform.position;

        ClearHighlightTargetNode();
        GetCurrentOccupiedNode();
        currentOccupiedNode.SetOccupationId(team);

        isWalking = false;

        TileGrid.Instance.turnState = TurnState.CHOOSING;

        CheckActionsLeft();
    }

    private void UpdateDirection(Vector3 dir)
    {
        //Debug.Log("Direction: " + transform.rotation.eulerAngles);
        Quaternion lookRot = Quaternion.LookRotation(dir);
        Quaternion resultRot = transform.rotation * lookRot;

        if ((int)resultRot.eulerAngles.y == 0)
        {
            spriteManager.UpdateSpriteOrientation(CharacterDirection.FRONT);
            currentDirection = CharacterDirection.FRONT;
            return;
        }

        int yRot = 540 / (int)resultRot.eulerAngles.y;

        switch (yRot)
        {
            case 2:
                spriteManager.UpdateSpriteOrientation(CharacterDirection.LEFT);
                currentDirection = CharacterDirection.LEFT;
                break;
            case 3:
                spriteManager.UpdateSpriteOrientation(CharacterDirection.BACK);
                currentDirection = CharacterDirection.BACK;
                break;
            case 6:
                spriteManager.UpdateSpriteOrientation(CharacterDirection.RIGHT);
                currentDirection = CharacterDirection.RIGHT;
                break;
            default:
                break;
        }
    }

    private void GetCurrentOccupiedNode()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, float.MaxValue, sampleMask))
        {
            currentOccupiedNode = hit.collider.GetComponent<NodeB>();
            //Debug.Log("current occupied node: " + currentOccupiedNode.name);
        }

        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        TileGrid.Instance.currentHighlightedNode = currentOccupiedNode;
        currentOccupiedNode.PointNode();
        //Debug.DrawRay(transform.position + Vector3.up, Vector3.down * 5f, Color.red, 10f);
    }

    private void SetSelfOccupationId()
    {
        currentOccupiedNode.SetOccupationId(team);
    }

    private void ShowWalkArea()
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        EventsManager.HideAllUI();
        TileGrid.Instance.turnState = TurnState.MOVING;
        HighlightNodesInRange(AreaMode.WALKING);
    }

    private void ShowFightArea()
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        EventsManager.HideAllUI();
        TileGrid.Instance.turnState = TurnState.ATTACKING;
        HighlightNodesInRange(AreaMode.ATTACKING);
    }

    private void StartTurn()
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        actionsLeft = 2;
        EventsManager.ShowControlledCharacterUI(this);

        moved = false;
        attacked = false;

        EventsManager.toggleMoveBtn(true);
        EventsManager.toggleAttackBtn(true);
    }

    private void UseAction()
    {
        actionsLeft--;
    }

    public void GainAction()
    {
        actionsLeft++;

        if (!moved)
        {
            if(!attacked)
                attacked = true;
            else
                moved = true;
        }
        else
            moved = true;

        if (actionsLeft > 2)
            actionsLeft = 2;
    }

    private void CheckActionsLeft()
    {
        if (actionsLeft <= 0)
        {
            isChoosingDirection = true;
            BeginChoosingDirection();
        }
        else
        {
            TileGrid.Instance.currentHighlightedNode = currentOccupiedNode;
            currentOccupiedNode.PointNode();
            EventsManager.ShowControlledCharacterUI(this);

            if (actionsLeft == 1)
            {
                if (moved)
                    EventsManager.toggleMoveBtn(false);
                if (attacked)
                    EventsManager.toggleAttackBtn(false);
            }
        }
    }

    private void CheckIfNextTileOccupied(NodeB currentNode, NodeB nextNode, NodeB nextNextNode)
    {
        if (nextNode.OccupationId == team) 
        {
            Debug.Log("Ally detected");
            if (Physics.Raycast(nextNode.transform.position, Vector3.up, out RaycastHit hit, float.MaxValue, characterMask))
            {
                Debug.DrawRay(nextNode.transform.position, Vector3.up * 10f, Color.green, 10f);
                StartCoroutine(DodgeCR(hit.collider.GetComponent<Character>(), currentNode.transform.position, nextNextNode.transform.position, nextNode.transform.position));
            }
        }
    }

    private IEnumerator DodgeCR(Character dodgingCharacter, Vector3 fromPosition, Vector3 toPosition, Vector3 occupiedPosition)
    {
        Vector3 dodgeDirection = Vector3.zero;
        List<Vector3> blockedDirections = new List<Vector3>();

        Vector3 fromDir = fromPosition - occupiedPosition;
        Vector3 toDir = toPosition - occupiedPosition;
        fromDir = Vector3.ProjectOnPlane(fromDir, Vector3.up).normalized;
        toDir = Vector3.ProjectOnPlane(toDir, Vector3.up).normalized;

        Debug.Log("from dir: " + fromDir + "to dir: " + toDir);
        Debug.DrawRay(dodgingCharacter.transform.position, fromDir, Color.blue, 15f);
        Debug.DrawRay(dodgingCharacter.transform.position, toDir, Color.red, 15f);

        blockedDirections.Add(fromDir);
        blockedDirections.Add(toDir);

        List<Vector3> availableDirections = new List<Vector3>();

        if (!blockedDirections.Contains(Vector3.forward))
            availableDirections.Add(Vector3.forward);

        if (!blockedDirections.Contains(Vector3.back))
            availableDirections.Add(Vector3.back);

        if (!blockedDirections.Contains(Vector3.right))
            availableDirections.Add(Vector3.right);

        if (!blockedDirections.Contains(Vector3.left))
            availableDirections.Add(Vector3.left);

        Debug.Log("available0: " + availableDirections[0] + "available1: " + availableDirections[1]);

        int random = Random.Range(0, availableDirections.Count);

        dodgeDirection= availableDirections[random];

        Debug.Log("dodge DIR: " + dodgeDirection);

        Vector3 targetDodgePosition = occupiedPosition + (dodgeDirection);

        if (dodgeDirection != Vector3.zero)
        {
            dodgingCharacter.transform.DOMove(targetDodgePosition, walkAnimationSpeed / 2).SetEase(dodgingCurve).SetDelay(walkAnimationSpeed / 2);
            dodgingCharacter.transform.DOMove(occupiedPosition, walkAnimationSpeed / 2).SetEase(dodgingCurve).SetDelay(walkAnimationSpeed + walkAnimationSpeed / 2);
            yield return new WaitForSeconds(walkAnimationSpeed);
        }
        yield return null;
    }

    private void HighlightNodesInRange(AreaMode mode)
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        switch (mode)
        {
            case AreaMode.WALKING:
                highlightedNodes = TileGrid.Instance.GetAreaUtility(currentOccupiedNode, characterData.Move, team, characterData.Jump, false, false, false, AreaMode.WALKING).ToList();
                break;
            case AreaMode.ATTACKING:
                highlightedNodes = TileGrid.Instance.GetAreaUtility(currentOccupiedNode, 1, team, characterData.Jump, false, true, true, AreaMode.ATTACKING).ToList();
                break;
        }
        
        //Debug.Log("areaResult count: " + highlightedNodes.Count);

        foreach (NodeB node in highlightedNodes)
        {
            node.HighlightNode(mode == AreaMode.WALKING ? TileGrid.Instance.UnselectedTileColor_Move : TileGrid.Instance.UnselectedTileColor_Attack);
        }

        //Debug.Log("CurrentHighLightedNode: " + TileGrid.Instance.currentHighlightedNode.name);
    }

    public void ClearHighlightedNodes()
    {
        for (int i = 0; i < highlightedNodes.Count; i++)
        {
            highlightedNodes[i].LoseHighlight();
        }

        highlightedNodes.Clear();
    }

    private void HighlightTargetNode()
    {
        currentTargetNode.HighlightNode(TileGrid.Instance.SelectedTileColor);
    }

    private void ClearHighlightTargetNode()
    {
        currentTargetNode.LoseHighlight();
    }

    private void EndTurnDirectionChange(CharacterDirection direction)
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        switch (direction)
        {
            case CharacterDirection.FRONT:
                UpdateDirection(Vector3.forward);
                break;
            case CharacterDirection.RIGHT:
                UpdateDirection(Vector3.right);
                break;
            case CharacterDirection.LEFT:
                UpdateDirection(Vector3.left);
                break;
            case CharacterDirection.BACK:
                UpdateDirection(Vector3.back);
                break;
        }

        StopChoosingDirection();
    }
    #endregion

    #region Combat & UI
    private void InitializeCharacterStats()
    {
        currentHp = characterData.BaseMaxHp * characterData.Lvl;
        currentMp = characterData.BaseMaxMp * characterData.Lvl;

        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        EventsManager.ShowControlledCharacterUI.Invoke(this);
    }

    private void AttackTarget(Character character)
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        int dmgToGive = Mathf.Clamp(characterData.WeaponAtk - character.characterData.WeaponDef / 2, 1, 999);
        character.TakeDamage(dmgToGive);
        UseAction();
        attacked = true;
        CheckActionsLeft();
        //EventsManager.StartDmgShow?.Invoke(dmgToGive);
    }

    public void BeginChoosingDirection()
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        isChoosingDirection = true;
        directionChooseMenu.SetActive(true);
    }

    public void StopChoosingDirection()
    {
        if (TileGrid.Instance.SelectedCharacter != this)
            return;

        isChoosingDirection = false;
        directionChooseMenu.SetActive(false);

        currentOccupiedNode.UnpointNode();
        Debug.Log("passturn called by: " + this.name);
        TileGrid.Instance.PassTurn();
    }

    public void TakeDamage(int dmg)
    {
        currentHp -= dmg;

        if (currentHp <= 0)
        {
            currentHp = 0;
            spriteManager.CharacterSprite.color = deadColor;
            isDead = true;
        }
    }
    #endregion
}

public enum CharacterDirection
{
    FRONT,
    RIGHT,
    LEFT,
    BACK
}

public enum TurnState
{
    WAITING,
    CHOOSING,
    MOVING,
    ATTACKING
}

public enum Team
{
    ALLY,
    ENEMY,
    NEUTRAL
}
