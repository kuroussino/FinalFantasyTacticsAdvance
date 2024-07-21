using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    #region Variables & Properties

    #region Local
    [Header("Basic infos")]
    [SerializeField] Sprite characterSprite;
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

    [Header("Level and EXP")]
    [SerializeField] int lvl;
    [SerializeField] int exp;

    [Header("Misc")]
    [SerializeField] Sprite[] frontIdle;
    [SerializeField] Sprite[] backIdle;
    #endregion

    #region Public
    public Sprite CharacterSprite { get { return characterSprite; } }
    public string CharacterName { get { return characterName; } }
    public string CharacterClass { get { return characterClass; } }
    public int BaseMaxHp { get { return baseMaxHp; } }
    public int BaseMaxMp { get { return baseMaxMp; } }
    public int Move { get { return move; } }
    public int Jump { get { return jump; } }
    public int Evade { get { return evade; } }
    public int WeaponAtk { get { return weaponAtk; } }
    public int WeaponDef { get { return weaponDef; } }
    public int MicPow { get { return maicPow; } }
    public int MagicRes { get { return magicRes; } }
    public int Speed { get { return speed; } }
    public Sprite[] FrontIdle { get { return frontIdle; } }
    public Sprite[] BackIdle { get { return backIdle; } }

    public int Lvl { get { return lvl; } }
    public int Exp { get { return exp; } }
    #endregion

    #endregion

    #region Monobehaviour
    #endregion

    #region Methods
    #endregion
}
