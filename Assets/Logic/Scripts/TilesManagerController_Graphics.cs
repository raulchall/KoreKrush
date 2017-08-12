using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoreKrush;

public class TilesManagerController_Graphics : MonoBehaviour
{
    public Color[] tilesColors;
    public int tilesSpacing = 15;
    public Sprite rightImage;
    public Sprite wrongImage;

    private LineRenderer selectionLine;
    private TileController_Graphics[,] tiles_graphics;

    void Awake()
    {
        selectionLine = GetComponent<LineRenderer>();

        KoreKrush.Events.Logic.BoardBuilt += OnBoardBuilt;
        KoreKrush.Events.Logic.GameStarted += OnGameStarted;
        KoreKrush.Events.Logic.TileConnected += OnTileConnected;
        KoreKrush.Events.Logic.TileDisconnected += OnTileDisconnected;
        KoreKrush.Events.Logic.TilesSequenceStarted += OnTilesSequenceStarted;
        KoreKrush.Events.Logic.TilesSequenceCompleted += OnTilesSequenceCompleted;
    }

    private void OnBoardBuilt()
    {
        var tiles = Board.tiles;
        int rows = Board.Rows, cols = Board.Cols;
        tiles_graphics = new TileController_Graphics[rows, cols];

        float row, col;  // row and col in world space

        col = -tilesSpacing * (rows / 2) + (rows % 2 == 0 ? tilesSpacing / 2f : 0);
        for (int i = 0; i < rows; i++, col += tilesSpacing)
        {
            row = -tilesSpacing * (cols / 2) + (cols % 2 == 0 ? tilesSpacing / 2f : 0);
            for (int j = 0; j < cols; j++, row += tilesSpacing)
            {
                tiles_graphics[i, j] = tiles[i, j].GetComponent<TileController_Graphics>();
                tiles_graphics[i, j].Color = tilesColors[tiles[i, j].color];
                tiles[i, j].transform.localPosition = new Vector3(row, col);
            }
        }

        KoreKrush.Events.Graphics.BoardPlaced();
    }

    private void OnGameStarted()
    {

    }

    private void OnTileConnected(TileController tile)
    {
        tiles_graphics[tile.row, tile.col].StateImage = rightImage;
        
        selectionLine.positionCount++;
        selectionLine.SetPosition(selectionLine.positionCount - 1, tile.transform.position + new Vector3(0, 0, -5));
    }

    private void OnTileDisconnected(TileController tile)
    {
        tiles_graphics[tile.row, tile.col].StateImage = null;

        selectionLine.positionCount--;
    }

    private void OnTilesSequenceStarted()
    {
        
    }

    private void OnTilesSequenceCompleted()
    {
        foreach (var tile in Board.tilesSequence)
        {
            tiles_graphics[tile.row, tile.col].StateImage = null;
            tiles_graphics[tile.row, tile.col].Color = tilesColors[tile.color];
        }
        
        selectionLine.positionCount = 0;
    }
}
