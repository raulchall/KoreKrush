using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int row;
    public int col;
    public bool selected = false;

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

    void OnMouseDown()
    {
        tilesManager.BeginTilesSelection (this);
        Select ();
    }

    void OnMouseEnter()
    {
        if (Input.GetMouseButton (0)) {
            if (tilesManager.TryAddTile (this)) {
                Select ();
            } else {
                MarkAsWrong ();
            }
        }
    }

    void OnMouseExit()
    {
        if (!selected)
            ClearMark ();
    }

    public void SetUp(int i, int j, Color[] colors, TilesManagerController tilesManager)
    {
        row = i;
        col = j;
        sprite.color = colors.Choice ();
        originalColor = sprite.color;
        this.tilesManager = tilesManager;
    }

    public bool SameColor(TileController other)
    {
        return other.sprite.color == sprite.color;
    }

    public void Select()
    {
        selected = true;
        MarkAsRight ();
    }

    public void Unselect()
    {
        selected = false;
        ClearMark ();
    }

    private void MarkAsRight()
    {
        stateImage.sprite = tilesManager.rightImage;
    }

    private void MarkAsWrong()
    {
        stateImage.sprite = tilesManager.wrongImage;
    }

    private void ClearMark()
    {
        stateImage.sprite = null;
    }

}
