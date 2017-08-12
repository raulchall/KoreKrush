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
        CheckTilesSequenceCompleted();
    }

    private void BuildBoard()
    {   
        Board.tiles = new TileController[tilesRows, tilesCols];
        Board.tilesSequence = new List<TileController>();
        Board.numberOfColors = numberOfColors;

        for (int i = 0; i < tilesRows; i++)
            for (int j = 0; j < tilesCols; j++) 
            {
                Board.tiles[i, j] = Instantiate(tilesPrefab, tilesContainer)
                    .GetComponent<TileController>();
                Board.tiles[i, j].row = i;
                Board.tiles[i, j].col = j;
                Board.tiles[i, j].color = Random.Range(0, numberOfColors);
            }
    }

    private void CheckTilesSequenceCompleted()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (Board.tilesSequence.Count == 1)
            {
                var tile = Board.Last;
                tile.selected = false;

                KoreKrush.Events.Logic.TileDisconnected(tile);
                Board.Clear();
            }
            else if (Board.tilesSequence.Count > 1)
            {
                for (int i = 0; i < Board.tilesSequence.Count; i++)
                {
                    Board.tilesSequence[i].selected = false;
                    Board.tilesSequence[i].color = Random.Range(0, numberOfColors);
                }
                
                KoreKrush.Events.Logic.TilesSequenceCompleted();
                Board.Clear();
            }
        }
    }

    private void OnBoardPlaced()
    {
        KoreKrush.Events.Logic.GameStarted();
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
        }
        else if (tile == Board.SecondLast)
        {
            Board.Last = null;
            lastTile.selected = false;

            KoreKrush.Events.Logic.TileDisconnected(lastTile);
        }
        else if (tile.color == lastTile.color && tile.AdjacentTo(lastTile) && !tile.selected)
        {
            Board.Last = tile;
            tile.selected = true;

            KoreKrush.Events.Logic.TileConnected(tile);
        }
    }
}
