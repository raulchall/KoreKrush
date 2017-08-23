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
        KoreKrush.Events.Graphics.TilesSequenceDestroyed_G += OnTilesSequenceDestroyed_G;
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
        Board.cells = new Board.Cell[tilesRows, tilesCols];
        Board.tilesSequence = new List<TileController>();
        Board.numberOfColors = numberOfColors;

        for (int i = 0; i < tilesRows; i++)
            for (int j = 0; j < tilesCols; j++) 
            {
                var tile = Instantiate(tilesPrefab, tilesContainer)
                    .GetComponent<TileController>();

                Board.cells[i, j] = new Board.Cell
                {
                    tile = tile,
                    row = i,
                    col = j
                };

                tile.cell = Board.cells[i, j];
                tile.color = Random.Range(0, numberOfColors);
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
                KoreKrush.Events.Logic.TilesSequenceCanceled_L();
            }
            else if (Board.tilesSequence.Count > 1)
            {
                for (int i = 0; i < Board.tilesSequence.Count; i++)
                {
                    Board.tilesSequence[i].selected = false;
                    Board.tilesSequence[i].color = Random.Range(0, numberOfColors);
                }
                
                KoreKrush.Events.Logic.TilesSequenceCompleted_L();
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

    private void OnTilesSequenceDestroyed_G()
    {   
        Board.ClearSelecteds();
    }
}
