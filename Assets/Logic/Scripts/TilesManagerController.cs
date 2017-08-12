using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using KoreKrush;

public class TilesManagerController : MonoBehaviour
{
    public int tilesRows = 7;
    public int tilesCols = 7;
    public int numberOfColors = 4;

    public GameObject tilesPrefab;
    public Transform tilesContainer;

    void Awake()
    {
        BuildBoard();

        KoreKrush.Events.Graphics.BoardPlaced += OnBoardPlaced;
        KoreKrush.Events.Logic.TileSelected += OnTileSelected;
    }

    void Start()
    {
        KoreKrush.Events.Logic.BoardBuilt();
    }
	
    // Update is called once per frame
    void Update()
    {
//        if (Input.GetMouseButtonUp(0))
//        {
//            if (selectedTiles.Count == 1)
//            {
//                selectedTiles[0].Unselect();
//            }
//            else
//            {
//                for (int i = 0; i < selectedTiles.Count; i++)
//                    selectedTiles[i].ChangeColor(tilesColors.Choice());
//                selectionLine.positionCount = 0;
//            }
//            selectedTiles.Clear();
//            for (int i = 0; i < tilesRows; i++)
//            {
//                for (int j = 0; j < tilesCols; j++)
//                {
//                    tiles[i, j].Unselect();
//                }
//            }
//        }
    }

    private void BuildBoard()
    {   
        Board.tiles = new TileController[tilesRows, tilesCols];
        Board.tilesSequence = new List<TileController>();
        Board.numberOfColors = numberOfColors;

        for (int i = 0; i < tilesRows; i++)
            for (int j = 0; j < tilesCols; j++) 
            {
                Board.tiles[i, j] = GameObject.Instantiate(tilesPrefab, tilesContainer)
                    .GetComponent<TileController>();
                Board.tiles[i, j].row = i;
                Board.tiles[i, j].col = j;
                Board.tiles[i, j].color = Random.Range(0, numberOfColors);
            }
    }

    private void OnBoardPlaced()
    {
//        KoreKrush.Events.Logic.GameStarted();
    }

    private void OnTileSelected(TileController tile)
    {
        var lastTile = Board.Last;

        if (!lastTile)
        {
            Board.Last = tile;
            tile.selected = true;
            KoreKrush.Events.Logic.TileConnected(tile);
            KoreKrush.Events.Logic.TilesSequenceStarted();
            return;
        }
        else if (tile == Board.SecondLast)
        {
            Board.Last = null;
            lastTile.selected = false;
            KoreKrush.Events.Logic.TileDisconnected(lastTile);
        }
        else if (tile.color == lastTile.color && tile.AdjacentTo(lastTile))
        {
            Board.Last = tile;
            tile.selected = true;
            KoreKrush.Events.Logic.TileConnected(tile);
        }
    }
        
//
//    public void BeginTilesSelection(TileController firstTile)
//    {
//        Assert.IsTrue(selectedTiles.Count == 0, "The list must be empty to begin the selection");
//        selectedTiles.Add(firstTile);
//    }
//
//    public bool TryAddTile(TileController tile)
//    {
//        if (selectedTiles.Count == 0)
//        {
//            BeginTilesSelection(tile);
//            return true;
//        }
//
//        var lastTile = selectedTiles.Last();
//        if (tile.SameColor(lastTile))
//        {
//            if (tile.selected)
//            {
//                if (selectedTiles.Count > 1 && tile == selectedTiles[selectedTiles.Count - 2])
//                {
//                    lastTile.Unselect();
//                    selectedTiles.RemoveAt(selectedTiles.Count - 1);
//                    if (selectedTiles.Count == 1)
//                        selectionLine.positionCount = 0;
//                    else
//                        selectionLine.positionCount--;
//                }
//                return true;
//            }
//            else if (AdjacentTiles(tile, lastTile))
//            {
//                selectedTiles.Add(tile);
//                if (selectedTiles.Count == 2)
//                {
//                    selectionLine.positionCount = 2;
//                    for (int i = 0; i < 2; i++)
//                        selectionLine.SetPosition(i, selectedTiles[i].transform.position + new Vector3(0, 0, -5));    
//                }
//                else
//                {
//                    selectionLine.positionCount++;
//                    selectionLine.SetPosition(selectedTiles.Count - 1, tile.transform.position + new Vector3(0, 0, -5));
//                }
//                return true;
//            }
//        }
//        return false;
//    }
}
