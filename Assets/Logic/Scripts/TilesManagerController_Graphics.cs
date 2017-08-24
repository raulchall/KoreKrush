using System.Collections;
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

        KoreKrush.Events.Logic.BoardBuilt_L                 += OnBoardBuilt_L;
        KoreKrush.Events.Logic.GameStarted_L                += OnGameStarted_L;
        KoreKrush.Events.Logic.TileSpawned_L                += OnTileSpawned_L;
        KoreKrush.Events.Logic.TileDisplaced_L              += OnTileDisplaced_L;
        KoreKrush.Events.Logic.TileConnected_L              += OnTileConnected_L;
        KoreKrush.Events.Logic.TileDisconnected_L           += OnTileDisconnected_L;
        KoreKrush.Events.Logic.TilesSequenceStarted_L       += OnTilesSequenceStarted_L;
        KoreKrush.Events.Logic.TilesSequenceCanceled_L      += OnTilesSequenceCanceled_L;
        KoreKrush.Events.Logic.TilesSequenceCompleted_L     += OnTilesSequenceCompleted_L;
    }

    private void OnBoardBuilt_L()
    {
        var cells = Board.cells;
        int rows = Board.Rows, cols = Board.Cols;
        tiles_graphics = new TileController_Graphics[rows, cols];

        float col, row;  // row and col in world space

        var p = RealBaseTilesPosition();

        row = p.x;
        for (int i = 0; i < rows; i++, row -= tilesSpacing)
        {
            col = p.y;
            for (int j = 0; j < cols; j++, col += tilesSpacing)
            {
                var tile_graphics = cells[i, j].tile.GetComponent<TileController_Graphics>();

                tiles_graphics[i, j] = tile_graphics;
                tile_graphics.Color = tilesColors[cells[i, j].tile.color];
                tile_graphics.transform.localPosition = new Vector3(col, row);
            }
        }

        KoreKrush.Events.Graphics.BoardBuilt_G();
    }

    private Vector2 RealBaseTilesPosition(int i = 0, int j = 0)
    {
        return new Vector2
        {
            x =  tilesSpacing * (Board.Rows / 2) + (Board.Rows % 2 == 0 ? tilesSpacing / 2f : 0) - i * tilesSpacing,
            y = -tilesSpacing * (Board.Cols / 2) + (Board.Cols % 2 == 0 ? tilesSpacing / 2f : 0) + j * tilesSpacing
        };
    }

    private void OnGameStarted_L()
    {

    }

    private void OnTileConnected_L(TileController tile)
    {
        tiles_graphics[tile.Row, tile.Col].StateImage = rightImage;
        
        selectionLine.positionCount++;
        selectionLine.SetPosition(selectionLine.positionCount - 1, tile.transform.position + new Vector3(0, 0, -5));
    }

    private void OnTileDisconnected_L(TileController tile)
    {
        tiles_graphics[tile.Row, tile.Col].StateImage = null;

        selectionLine.positionCount--;
    }

    private void OnTilesSequenceStarted_L()
    {
        
    }

    private void OnTilesSequenceCanceled_L()
    {
        KoreKrush.Events.Graphics.TilesSequenceCanceled_G();
    }

    private void OnTilesSequenceCompleted_L()
    {
        StartCoroutine(HideTilesSequence());
    }

    private IEnumerator HideTilesSequence()
    {
        foreach (var tile in Board.tilesSequence)
            tiles_graphics[tile.Row, tile.Col].StateImage = null;

        selectionLine.positionCount = 0;

        foreach (var tile in Board.tilesSequence)
        {
            yield return new WaitForSeconds(.1f);

            var newColor = tiles_graphics[tile.Row, tile.Col].Color;
            newColor.a = 0;
            tiles_graphics[tile.Row, tile.Col].Color = newColor;
        }

        KoreKrush.Events.Graphics.TilesSequenceDestroyed_G();
    }

    private void OnTileSpawned_L(TileController tile)
    {
        int i = tile.Row, j = tile.Col;

        var p = RealBaseTilesPosition(i: tile.Row, j: tile.Col);

        var tile_graphics = Board.cells[i, j].tile.GetComponent<TileController_Graphics>();

        tiles_graphics[i, j] = tile_graphics;
        tile_graphics.Color = tilesColors[Board.cells[i, j].tile.color];
        tile_graphics.transform.localPosition = new Vector3(p.y, p.x);
    }

    private void OnTileDisplaced_L(TileController tile, Board.Cell from)
    {
        int i = tile.Row, j = tile.Col;

        var p = RealBaseTilesPosition(i: tile.Row, j: tile.Col);

        var tile_graphics = tiles_graphics[from.row, from.col];

        tiles_graphics[i, j] = tile_graphics;
        tile_graphics.transform.localPosition = new Vector3(p.y, p.x);
    }
}
