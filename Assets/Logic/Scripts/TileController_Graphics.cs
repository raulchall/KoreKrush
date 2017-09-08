using UnityEngine;


public class TileController_Graphics : MonoBehaviour
{
    private SpriteRenderer stateImage;

    public SpriteRenderer Sprite { get; set; }

    public Color Color
    {
        get { return Sprite.color; }
        set { Sprite.color = value; }
    }

    public Sprite StateImage 
    {
        set { stateImage.sprite = value; }
    }

    void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
        stateImage = transform.GetChild(0)
            .GetComponent<SpriteRenderer>();
    }
}
