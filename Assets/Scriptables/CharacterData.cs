using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    #region Variables & Properties

    #region Local
    [Header("Base informations")]
    [SerializeField] string characterName;
    [SerializeField] string characterClass;

    [Header("Stats")]
    [SerializeField] int baseMaxHp;
    [SerializeField] int baseMaxMp;
    [SerializeField] int move;
    [SerializeField] int jump;
    [SerializeField] int evade;
    [SerializeField] int weaponAtk;
    [SerializeField] int weaponDef;
    [SerializeField] int maicPow;
    [SerializeField] int magicRes;
    [SerializeField] int speed;

    [Header("Misc")]
    [SerializeField] Sprite[] frontIdle;
    [SerializeField] Sprite[] backIdle;
    #endregion

    #region Public

    #endregion

    #endregion

    #region Monobehaviour
    #endregion

    #region Methods
    #endregion
}
