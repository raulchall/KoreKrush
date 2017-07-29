using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int row;
    public int col;
    private SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer> ();
    }
	
    // Update is called once per frame
    void Update()
    {
		
    }

    public void SetUp(int i, int j, Color[] colors)
    {
        row = i;
        col = j;
        sprite.color = colors.Choice ();
    }
}
