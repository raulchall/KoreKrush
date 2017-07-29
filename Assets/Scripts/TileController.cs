using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int row;
    public int col;

    private SpriteRenderer sprite;
    private Color originalColor;
    private TilesManagerController tilesManager;
    private SpriteRenderer stateImage;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer> ();
        stateImage = transform.GetChild (0)
            .GetComponent<SpriteRenderer> ();
    }
	
    // Update is called once per frame
    void Update()
    {
		
    }

    public void SetUp(int i, int j, Color[] colors, TilesManagerController tilesManager)
    {
        row = i;
        col = j;
        sprite.color = colors.Choice ();
        originalColor = sprite.color;
        this.tilesManager = tilesManager;
    }

    void OnMouseDown()
    {
        print (string.Format ("Hello from tile ({0}, {1})", row, col));
        stateImage.sprite = tilesManager.rightImage;
    }

    void OnMouseExit()
    {
        stateImage.sprite = null;
    }
}
