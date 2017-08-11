using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    private List<TileController> selectedTiles = new List<TileController>();
    private Transform tilesContainer;
    private LineRenderer selectionLine;

    void Awake()
    {
        tilesContainer = transform.GetChild(0);
        selectionLine = GetComponent<LineRenderer>();
        tiles = new TileController[tilesRows, tilesCols];
    }

    void Start()
    {
        BuildTiles();
    }
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (selectedTiles.Count == 1)
            {
                selectedTiles[0].Unselect();
            }
            else
            {
                for (int i = 0; i < selectedTiles.Count; i++)
                    selectedTiles[i].ChangeColor(tilesColors.Choice());
                selectionLine.positionCount = 0;
            }
            selectedTiles.Clear();
            for (int i = 0; i < tilesRows; i++)
            {
                for (int j = 0; j < tilesCols; j++)
                {
                    tiles[i, j].Unselect();
                }
            }
        }
    }

    private void BuildTiles()
    {   
        float row, col;  // row and col in world space

        col = -tilesSpacing * (tilesRows / 2) + (tilesRows % 2 == 0 ? tilesSpacing / 2f : 0);
        for (int i = 0; i < tilesRows; i++, col += tilesSpacing)
        {
            row = -tilesSpacing * (tilesCols / 2) + (tilesCols % 2 == 0 ? tilesSpacing / 2f : 0);
            for (int j = 0; j < tilesCols; j++, row += tilesSpacing)
            {
                tiles[i, j] = GameObject.Instantiate(tilesPrefab, tilesContainer)
                    .GetComponent<TileController>();
                tiles[i, j].SetUp(i, j, tilesColors, this);
                tiles[i, j].transform.localPosition = new Vector3(row, col);
            }
        }
    }

    private bool AdjacentTiles(TileController tile1, TileController tile2)
    {
        return Mathf.Abs(tile1.row - tile2.row) + Mathf.Abs(tile1.col - tile2.col) == 1
        || Mathf.Abs(tile1.row - tile2.row) == 1 && Mathf.Abs(tile1.col - tile2.col) == 1;
    }

    public void BeginTilesSelection(TileController firstTile)
    {
        Assert.IsTrue(selectedTiles.Count == 0, "The list must be empty to begin the selection");
        selectedTiles.Add(firstTile);
    }

    public bool TryAddTile(TileController tile)
    {
        if (selectedTiles.Count == 0)
        {
            BeginTilesSelection(tile);
            return true;
        }

        var lastTile = selectedTiles.Last();
        if (tile.SameColor(lastTile))
        {
            if (tile.selected)
            {
                if (selectedTiles.Count > 1 && tile == selectedTiles[selectedTiles.Count - 2])
                {
                    lastTile.Unselect();
                    selectedTiles.RemoveAt(selectedTiles.Count - 1);
                    if (selectedTiles.Count == 1)
                        selectionLine.positionCount = 0;
                    else
                        selectionLine.positionCount--;
                }
                return true;
            }
            else if (AdjacentTiles(tile, lastTile))
            {
                selectedTiles.Add(tile);
                if (selectedTiles.Count == 2)
                {
                    selectionLine.positionCount = 2;
                    for (int i = 0; i < 2; i++)
                        selectionLine.SetPosition(i, selectedTiles[i].transform.position + new Vector3(0, 0, -5));    
                }
                else
                {
                    selectionLine.positionCount++;
                    selectionLine.SetPosition(selectedTiles.Count - 1, tile.transform.position + new Vector3(0, 0, -5));
                }
                return true;
            }
        }
        return false;
    }
}
