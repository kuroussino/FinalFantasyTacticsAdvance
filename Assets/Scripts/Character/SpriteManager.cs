using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [SerializeField] Sprite[] frontSprites;
    [SerializeField] Sprite[] backSprites;
    [SerializeField] float spritesScrollTime;
    float spritesTimer;
    [SerializeField] SpriteRenderer spriteRenderer;
    int currSpriteIndex;
    Sprite[] currSprites;
    #endregion

    #region Public
    #endregion

    #endregion

    #region Monobehaviour
    private void Start()
    {
        currSprites = backSprites;
        spritesTimer = spritesScrollTime;
        currSpriteIndex = 0;
        UpdateSprite();
    }
    private void Update()
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
    #endregion

    #region Methods
    public void UpdateOrientation(float characterYRot)
    {
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y - characterYRot, 0f);
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
