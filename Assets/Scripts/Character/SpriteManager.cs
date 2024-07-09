using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [SerializeField] Sprite[] frontSprites;
    [SerializeField] Sprite[] backSprites;
    [SerializeField] float spritesScrollTime;
    float spritesTimer;
    SpriteRenderer spriteRenderer;
    Character character;
    int currSpriteIndex;
    Sprite[] currSprites;
    #endregion

    #region Public
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
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y - character.transform.rotation.eulerAngles.y, 0f);

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
