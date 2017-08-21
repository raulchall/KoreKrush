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
        KoreKrush.Events.Logic.TilesSequenceCompleted_L += OnTilesSequenceCompleted_L;
    }

    private void OnBoardBuilt_L()
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

        KoreKrush.Events.Graphics.BoardBuilt_G();
    }

    private void OnGameStarted_L()
    {

    }

    private void OnTileConnected_L(TileController tile)
    {
        tiles_graphics[tile.row, tile.col].StateImage = rightImage;
        
        selectionLine.positionCount++;
        selectionLine.SetPosition(selectionLine.positionCount - 1, tile.transform.position + new Vector3(0, 0, -5));
    }

    private void OnTileDisconnected_L(TileController tile)
    {
        tiles_graphics[tile.row, tile.col].StateImage = null;

        selectionLine.positionCount--;
    }

    private void OnTilesSequenceStarted_L()
    {
        
    }

    private void OnTilesSequenceCompleted_L()
    {
        foreach (var tile in Board.tilesSequence)
        {
            tiles_graphics[tile.row, tile.col].StateImage = null;
            tiles_graphics[tile.row, tile.col].Color = tilesColors[tile.color];
        }
        
        selectionLine.positionCount = 0;
    }
}
