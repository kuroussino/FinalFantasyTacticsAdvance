using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.TextCore.Text;

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
    [SerializeField] DetailsPanel action;
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
    }

    private void OnDisable()
    {
        EventsManager.ShowControlledCharacterUI -= ShowControlledCharacterUI;
        EventsManager.ShowTargetUI -= ShowTargetUI;
        EventsManager.HideAllUI -= HideAll;
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

    //Single
    private void ShowDetailsPanel(DetailsPanel dp, Character character)
    {
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
        HideDetailsPanel(action);
    }

    //Buttons
    public void MoveBtn()
    {
        
    }
    #endregion
}
