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
        float wi, wj;  // world i, j (row and col in world space)

        wi = -tilesSpacing * (tilesRows / 2) + (tilesRows % 2 == 0 ? tilesSpacing / 2f : 0);
        for (int i = 0; i < tilesRows; i++, wi += tilesSpacing) {
            wj = -tilesSpacing * (tilesCols / 2) + (tilesCols % 2 == 0 ? tilesSpacing / 2f : 0);
            for (int j = 0; j < tilesCols; j++, wj += tilesSpacing) {
                tiles [i, j] = GameObject.Instantiate (tilesPrefab, tilesContainer)
                    .GetComponent<TileController> ();
                tiles [i, j].SetUp (i, j, tilesColors);
                tiles [i, j].transform.localPosition = new Vector3 (wi, wj);
            }
        }
    }
}
