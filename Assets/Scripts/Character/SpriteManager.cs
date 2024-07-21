using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [SerializeField] float spritesScrollTime;
    Sprite[] frontSprites;
    Sprite[] backSprites;
    Sprite[] currSprites;
    float spritesTimer;
    int currSpriteIndex;

    SpriteRenderer spriteRenderer;
    Character character;
    #endregion

    #region Public
    public SpriteRenderer CharacterSprite => spriteRenderer;
    #endregion

    #endregion

    #region Monobehaviour
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        character = GetComponent<Character>();
    }

    private void Start()
    {
        frontSprites = character.CharacterData.FrontIdle;
        backSprites = character.CharacterData.BackIdle;

        currSprites = backSprites;
        spritesTimer = spritesScrollTime;

        currSpriteIndex = 0;
        UpdateSprite();
    }
    private void Update()
    {
        SpriteTimer();
    }
    #endregion

    #region Methods
    public void UpdateSpriteOrientation(CharacterDirection dir)
    {
        switch (dir)
        {
            case CharacterDirection.FRONT:
                spriteRenderer.flipX = true;
                currSprites = frontSprites;
                break;
            case CharacterDirection.RIGHT:
                spriteRenderer.flipX = false;
                currSprites = frontSprites;
                break;
            case CharacterDirection.LEFT:
                spriteRenderer.flipX = true;
                currSprites = backSprites;
                break;
            case CharacterDirection.BACK:
                spriteRenderer.flipX = false;
                currSprites = backSprites;
                break;
        }

        spritesTimer = 0f;
        //Debug.Log("Current spritePack: " + currSprites);
    }

    private void SpriteTimer()
    {
        if (spritesTimer > 0f)
        {
            spritesTimer -= Time.deltaTime;
        }
        else
        {
            spritesTimer = spritesScrollTime;
            UpdateSprite();
        }
    }

    private void UpdateSprite()
    {
        currSpriteIndex++;

        if (currSpriteIndex < currSprites.Length)
        {
            spriteRenderer.sprite = currSprites[currSpriteIndex];
        }
        else
        {
            currSpriteIndex = 0;
            spriteRenderer.sprite = currSprites[currSpriteIndex];
        }
    }
    #endregion
}
