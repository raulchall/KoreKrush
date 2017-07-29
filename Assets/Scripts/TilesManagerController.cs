using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TilesManagerController : MonoBehaviour
{
    public int tilesRows = 4;
    public int tilesCols = 4;
    public Color[] tilesColors;

    public int tilesSpacing = 15;
    public GameObject tilesPrefab;
    public Sprite rightImage;
    public Sprite wrongImage;

    private TileController[,] tiles;
    private Transform tilesContainer;

    void Awake()
    {
        tilesContainer = transform.GetChild (0);

        tiles = new TileController[tilesRows, tilesCols];
    }

    void Start()
    {
        BuildTiles ();
    }
	
    // Update is called once per frame
    void Update()
    {
		
    }

    void BuildTiles()
    {   
        float row, col;  // row and col in world space

        col = -tilesSpacing * (tilesRows / 2) + (tilesRows % 2 == 0 ? tilesSpacing / 2f : 0);
        for (int i = 0; i < tilesRows; i++, col += tilesSpacing) {
            row = -tilesSpacing * (tilesCols / 2) + (tilesCols % 2 == 0 ? tilesSpacing / 2f : 0);
            for (int j = 0; j < tilesCols; j++, row += tilesSpacing) {
                tiles [i, j] = GameObject.Instantiate (tilesPrefab, tilesContainer)
                    .GetComponent<TileController> ();
                tiles [i, j].SetUp (i, j, tilesColors, this);
                tiles [i, j].transform.localPosition = new Vector3 (row, col);
            }
        }
    }
}
