using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailsPanel : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [Header("Details To Fill")]
    [SerializeField] Image characterSprite;
    [SerializeField] Image bg;
    [SerializeField] TMP_Text characterName;
    [SerializeField] TMP_Text hpValue;
    [SerializeField] TMP_Text mpValue;
    [SerializeField] TMP_Text jpValue;
    [SerializeField] TMP_Text wtValue;
    [SerializeField] TMP_Text lvlExpValue;
    [SerializeField] TMP_Text hitDmg;
    [SerializeField] TMP_Text hitAcc;

    [SerializeField] TMP_Text heightCounter;

    [SerializeField] Transform visiblePosition;
    [SerializeField] Transform hiddenPosition;

    bool visible;
    #endregion

    #region Public
    public Vector2 VisiblePosition { get { return visiblePosition.position; } }
    public Vector2 HiddenPosition { get { return hiddenPosition.position; } }
    public bool Visible { get { return visible; } set { visible = value; } }
    #endregion

    #endregion

    #region Monobehaviour
    private void Start()
    {
        visible = true;
    }
    #endregion

    #region Methods
    public void CompileDetails(Character character, Color color)
    {
        if (character == null)
            return;

        if (bg != null)
            bg.color = color;

        if (characterSprite != null)
            characterSprite.sprite = character.CharacterData.CharacterSprite;

        if(characterName != null)
            characterName.text = character.CharacterData.CharacterName;

        if(hpValue != null)
            hpValue.text = character.CurrentHp + " / " + character.CharacterData.BaseMaxHp * character.CharacterData.Lvl;

        if(mpValue != null)
            mpValue.text = character.CurentMp + " / " + character.CharacterData.BaseMaxMp * character.CharacterData.Lvl;

        if (jpValue != null)
            jpValue.text = "0";

        if(wtValue != null)
            wtValue.text = "0";

        if (lvlExpValue != null)
            lvlExpValue.text = "Lvl: " + character.CharacterData.Lvl + " Exp: " + character.CharacterData.Exp;

        if (hitDmg != null)
            hitDmg.text = character.CharacterData.WeaponAtk.ToString();

        if (hitAcc != null)
            hitAcc.text = "100";
    }

    public void CompileHeightCounter(int height)
    {
        if(heightCounter != null)
            heightCounter.text = height + "h";
    }
    #endregion
}
