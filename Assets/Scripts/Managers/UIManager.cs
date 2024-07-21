using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [Header("DetailsPanels")]
    [SerializeField] DetailsPanel characterDetails;
    [SerializeField] DetailsPanel characterShortDetails;
    [SerializeField] DetailsPanel targetDetails;
    [SerializeField] DetailsPanel attackerDetails;
    [SerializeField] DetailsPanel counterDetails;

    [Header("Other UI Elements")]
    [SerializeField] DetailsPanel menu;
    [SerializeField] DetailsPanel actions;
    [SerializeField] GameObject confirmationMenu;
    [SerializeField] DetailsPanel heightCounter;

    [Header("Animations")]
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] float animationTime;

    [Header("Colors")]
    [SerializeField] Color alliesColor;
    [SerializeField] Color enemiesColor;
    //[Header("Menus")]
    //[SerializeField] 

    [Header("Buttons")]
    [SerializeField] Button move;
    [SerializeField] Button action;
    [SerializeField] Button wait;
    [SerializeField] Button status;
    [SerializeField] Button fight;
    [SerializeField] Button skills;
    [SerializeField] Button item;

    [SerializeField] Image movePointerSprite;
    [SerializeField] Image actionPointerSprite;
    [SerializeField] Image waitPointerSprite;
    [SerializeField] Image statusPointerSprite;
    [SerializeField] Image fightPointerSprite;
    [SerializeField] Image skillPointerSprite;
    [SerializeField] Image itemPointerSprite;

    [SerializeField] Sprite pointerEmpty;
    [SerializeField] Sprite pointerFull;
    #endregion

    #region Public
    #endregion

    #endregion

    #region Monobehaviour
    private void Start()
    {
        HideAll();
    }

    private void OnEnable()
    {
        EventsManager.ShowControlledCharacterUI += ShowControlledCharacterUI;
        EventsManager.ShowTargetUI += ShowTargetUI;
        EventsManager.HideAllUI += HideAll;
        EventsManager.toggleMoveBtn += OnToggleMoveBtn;
        EventsManager.toggleAttackBtn += OnToggleAttackBtn;

        EventsManager.HideActionAndShowMenu += HideActionAndShowMenu;

        EventsManager.StartDmgShow += StartDmgShow;
    }

    private void OnDisable()
    {
        EventsManager.ShowControlledCharacterUI -= ShowControlledCharacterUI;
        EventsManager.ShowTargetUI -= ShowTargetUI;
        EventsManager.HideAllUI -= HideAll;
        EventsManager.toggleMoveBtn -= OnToggleMoveBtn;
        EventsManager.toggleAttackBtn -= OnToggleAttackBtn;

        EventsManager.HideActionAndShowMenu -= HideActionAndShowMenu;

        EventsManager.StartDmgShow -= StartDmgShow;
    }
    #endregion

    #region Methods

    //Multiple
    private void ShowControlledCharacterUI(Character character)
    {
        StartCoroutine(ShowControlledCharacterUI_CR(character));
    }

    private IEnumerator ShowControlledCharacterUI_CR(Character character)
    {
        HideAll();

        yield return new WaitForSeconds(animationTime);

        ShowHeightCounter((int)character.transform.position.y + 2);
        ShowDetailsPanel(characterDetails, character);
        ShowDetailsPanel(menu, character);
    }

    private void ShowTargetUI(Character character)
    {
        StartCoroutine(ShowTargetUI_CR(character));
    }

    private IEnumerator ShowTargetUI_CR(Character character)
    {
        HideAll();

        yield return new WaitForSeconds(animationTime);

        ShowHeightCounter((int)character.transform.position.y + 2);
        ShowDetailsPanel(targetDetails, character);
    }

    private void HideActionAndShowMenu()
    {
        HideDetailsPanel(actions);
        ShowDetailsPanel(menu, null);
    }

    //Single
    private void ShowDetailsPanel(DetailsPanel dp, Character character)
    {
        if(character != null)
            dp.CompileDetails(character, character.Team == 0 ? alliesColor : enemiesColor);

        if (!dp.Visible)
        {
            dp.transform.DOMove(dp.VisiblePosition, animationTime).SetEase(animationCurve);
            dp.Visible = true;
        }
    }

    private void HideDetailsPanel(DetailsPanel dp)
    {
        //dp.CompileDetails(character);
        if (dp.Visible)
        {
            dp.transform.DOMove(dp.HiddenPosition, animationTime).SetEase(animationCurve);
            dp.Visible = false;
        }
    }

    private void ShowConformationMenu()
    {
        confirmationMenu.SetActive(true);
    }

    private void HideConformationMenu() 
    {
        confirmationMenu.SetActive(false);
    }

    private void ShowHeightCounter(int height)
    {
        Debug.Log("height: " + height);
        heightCounter.CompileHeightCounter(height);

        if (!heightCounter.Visible)
        {
            heightCounter.transform.DOMove(heightCounter.VisiblePosition, animationTime).SetEase(animationCurve);
            heightCounter.Visible = true;
        }
    }

    private void HideHeightCounter()
    {
        if (heightCounter.Visible)
        {
            heightCounter.transform.DOMove(heightCounter.HiddenPosition, animationTime).SetEase(animationCurve);
            heightCounter.Visible = false;
        }
    }

    private void HideAll()
    {
        HideConformationMenu();
        HideHeightCounter();
        HideDetailsPanel(characterDetails);
        HideDetailsPanel(characterShortDetails);
        HideDetailsPanel(targetDetails);
        HideDetailsPanel(attackerDetails);
        HideDetailsPanel(counterDetails);
        HideDetailsPanel(menu);
        HideDetailsPanel(actions);
    }

    //Buttons
    public void OnMoveClick()
    {
        EventsManager.MoveClicked?.Invoke();
    }

    public void OnActionClick()
    {
        HideDetailsPanel(menu);
        ShowDetailsPanel(actions, null);
    }

    public void OnWaitClick()
    {
        TileGrid.Instance.currentHighlightedNode.UnpointNode();
        TileGrid.Instance.SelectedCharacter.BeginChoosingDirection();
    }

    public void OnStatusClick()
    {

    }

    public void OnFightClick()
    {
        EventsManager.FightClicked?.Invoke();
    }

    public void OnSkillsClick()
    {

    }

    public void OnItemClick()
    {

    }

    public void OnMoveHover()
    {
        movePointerSprite.sprite = pointerFull;
    }

    public void OnMoveUnhover()
    {
        movePointerSprite.sprite = pointerEmpty;
    }

    public void OnActionHover()
    {
        actionPointerSprite.sprite = pointerFull;
    }

    public void OnActionUnhover()
    {
        actionPointerSprite.sprite = pointerEmpty;
    }

    public void OnWaitHover()
    {
        waitPointerSprite.sprite = pointerFull;
    }

    public void OnWaitUnhover()
    {
        waitPointerSprite.sprite = pointerEmpty;
    }

    public void OnStatusHover()
    {
        statusPointerSprite.sprite = pointerFull;
    }

    public void OnStatusUnhover()
    {
        statusPointerSprite.sprite = pointerEmpty;
    }

    public void OnFightHover()
    {
        fightPointerSprite.sprite = pointerFull;
    }

    public void OnFightUnhover()
    {
        fightPointerSprite.sprite = pointerEmpty;
    }

    public void OnSkillHover()
    {
        skillPointerSprite.sprite = pointerFull;
    }

    public void OnSkillUnhover()
    {
        skillPointerSprite.sprite = pointerEmpty;
    }

    public void OnItemHover()
    {
        itemPointerSprite.sprite = pointerFull;
    }

    public void OnItemUnhover()
    {
        itemPointerSprite.sprite = pointerEmpty;
    }

    private void OnToggleMoveBtn(bool condition)
    {
        move.interactable = condition;
    }

    private void OnToggleAttackBtn(bool condition)
    {
        fight.interactable = condition;
    }

    private void StartDmgShow(int dmg)
    {
        StartCoroutine(StartDmgShowCR(dmg));
    }

    private IEnumerator StartDmgShowCR(int dmg)
    {

        yield return null;

        EventsManager.EndDmgShow?.Invoke();
    }
    #endregion
}
