using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Graphics : MonoBehaviour
{
    private SpriteRenderer sprite;
    private SpriteRenderer stateImage;

    public Color Color
    {
        get { return sprite.color; }
        set { sprite.color = value; }
    }

    public Sprite StateImage 
    {
        set { stateImage.sprite = value; }
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer> ();
        stateImage = transform.GetChild (0)
            .GetComponent<SpriteRenderer> ();
    }
}
