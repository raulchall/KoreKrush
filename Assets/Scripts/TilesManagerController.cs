using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManagerController : MonoBehaviour
{
    public int tilesRows = 4;
    public int tilesCols = 4;

    public Vector3 tilesPivot = Vector3.zero;
    public int tilesScale = 70;
    public int tilesSpacing = 15;
    public GameObject tilesPrefab;

    private TileController[,] tiles;

    void Awake()
    {
        tiles = new TileController[tilesRows, tilesCols];

        for (int i = 0, wi = 0; i < tilesRows; i++, wi += tilesSpacing) {  // wi: world i (row in world space)
            for (int j = 0; j < tilesCols; j++) {
                tiles [i, j] = GameObject.Instantiate (tilesPrefab, this.transform).GetComponent<TileController> ();
                tiles [i, j].transform.localPosition = new Vector3 ();
            }
        }
    }
	
    // Update is called once per frame
    void Update()
    {
		
    }

    void BuildTiles()
    {
		
    }
}
