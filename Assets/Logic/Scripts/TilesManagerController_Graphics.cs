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

        KoreKrush.Events.Logic.BoardBuilt_L += OnBoardBuilt_L;
        KoreKrush.Events.Logic.GameStarted_L += OnGameStarted_L;
        KoreKrush.Events.Logic.TileConnected_L += OnTileConnected_L;
        KoreKrush.Events.Logic.TileDisconnected_L += OnTileDisconnected_L;
        KoreKrush.Events.Logic.TilesSequenceStarted_L += OnTilesSequenceStarted_L;
        KoreKrush.Events.Logic.TilesSequenceCanceled_L += OnTilesSequenceCanceled_L;
        KoreKrush.Events.Logic.TilesSequenceCompleted_L += OnTilesSequenceCompleted_L;
    }

    private void OnBoardBuilt_L()
    {
        var cells = Board.cells;
        int rows = Board.Rows, cols = Board.Cols;
        tiles_graphics = new TileController_Graphics[rows, cols];

        float row, col;  // row and col in world space

        col = -tilesSpacing * (rows / 2) + (rows % 2 == 0 ? tilesSpacing / 2f : 0);
        for (int i = 0; i < rows; i++, col += tilesSpacing)
        {
            row = -tilesSpacing * (cols / 2) + (cols % 2 == 0 ? tilesSpacing / 2f : 0);
            for (int j = 0; j < cols; j++, row += tilesSpacing)
            {
                tiles_graphics[i, j] = cells[i, j].tile.GetComponent<TileController_Graphics>();
                tiles_graphics[i, j].Color = tilesColors[cells[i, j].tile.color];
                cells[i, j].tile.transform.localPosition = new Vector3(row, col);
            }
        }

        KoreKrush.Events.Graphics.BoardBuilt_G();
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
        KoreKrush.Events.Graphics.TilesSequenceDestroyed_G();
    }

    private void OnTilesSequenceCompleted_L()
    {
        StartCoroutine(ChangeTilesColor());
    }

    private IEnumerator ChangeTilesColor()
    {
        foreach (var tile in Board.tilesSequence)
            tiles_graphics[tile.Row, tile.Col].StateImage = null;

        selectionLine.positionCount = 0;

        foreach (var tile in Board.tilesSequence)
        {
            yield return new WaitForSeconds(.1f);

            tiles_graphics[tile.Row, tile.Col].Color = tilesColors[tile.color];
        }

        KoreKrush.Events.Graphics.TilesSequenceDestroyed_G();
    }
}
