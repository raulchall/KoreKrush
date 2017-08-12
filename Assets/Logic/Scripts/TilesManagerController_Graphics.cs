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
}
