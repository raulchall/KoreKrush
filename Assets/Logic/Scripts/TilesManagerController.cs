using System.Collections.Generic;
using UnityEngine;

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

        KoreKrush.Events.Graphics.BoardBuilt_G += OnBoardBuilt_G;
        KoreKrush.Events.Logic.TileSelected_L += OnTileSelected_L;
    }

    void Start()
    {
        KoreKrush.Events.Logic.BoardBuilt_L();
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

                KoreKrush.Events.Logic.TileDisconnected_L(tile);
                Board.Clear();
            }
            else if (Board.tilesSequence.Count > 1)
            {
                for (int i = 0; i < Board.tilesSequence.Count; i++)
                {
                    Board.tilesSequence[i].selected = false;
                    Board.tilesSequence[i].color = Random.Range(0, numberOfColors);
                }
                
                KoreKrush.Events.Logic.TilesSequenceCompleted_L();
                Board.Clear();
            }
        }
    }

    private void OnBoardBuilt_G()
    {
        KoreKrush.Events.Logic.GameStarted_L();
    }

    private void OnTileSelected_L(TileController tile)
    {
        var lastTile = Board.Last;

        if (!lastTile)
        {
            Board.Last = tile;
            tile.selected = true;

            KoreKrush.Events.Logic.TileConnected_L(tile);
            KoreKrush.Events.Logic.TilesSequenceStarted_L();
        }
        else if (tile == Board.SecondLast)
        {
            Board.Last = null;
            lastTile.selected = false;

            KoreKrush.Events.Logic.TileDisconnected_L(lastTile);
        }
        else if (tile.color == lastTile.color && tile.AdjacentTo(lastTile) && !tile.selected)
        {
            Board.Last = tile;
            tile.selected = true;

            KoreKrush.Events.Logic.TileConnected_L(tile);
        }
    }
}
