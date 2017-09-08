using System.Collections;
using UnityEngine;
using DG.Tweening;

using KoreKrush;


public class TilesManagerController_Graphics : MonoBehaviour
{
    public Color[] tilesColors;
    public int tilesSpacing = 15;
    public Sprite rightImage;
    public Sprite wrongImage;
    public float refillTime;

    private LineRenderer selectionLine;
    private TileController_Graphics[,] tiles_graphics;

    private int refillStage;

    void Awake()
    {
        DOTween.Init(true);
        DOTween.defaultAutoKill = false;

        selectionLine = GetComponent<LineRenderer>();

        KoreKrush.Events.Logic.GameStart_L                  += OnGameStart_L;
        KoreKrush.Events.Logic.BoardBuild_L                 += OnBoardBuild_L;
        KoreKrush.Events.Logic.BoardRefill_Begin_L          += OnBoardRefill_Begin_L;
        KoreKrush.Events.Logic.BoardRefill_End_L            += OnBoardRefill_End_L;
        KoreKrush.Events.Logic.BoardRefillStageStart_L      += OnBoardRefillStageStart_L;
        KoreKrush.Events.Logic.TileSpawn_L                  += OnTileSpawn_L;
        KoreKrush.Events.Logic.TileDisplace_L               += OnTileDisplace_L;
        KoreKrush.Events.Logic.TileConnect_L                += OnTileConnect_L;
        KoreKrush.Events.Logic.TileDisconnect_L             += OnTileDisconnect_L;
        KoreKrush.Events.Logic.TilesSequenceStart_L         += OnTilesSequenceStart_L;
        KoreKrush.Events.Logic.TilesSequenceCancel_L        += OnTilesSequenceCancel_L;
        KoreKrush.Events.Logic.TilesSequenceFinish_L        += OnTilesSequenceFinish_L;
    }

    private void OnBoardBuild_L()
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

        KoreKrush.Events.Graphics.BoardBuild_G();
    }

    private Vector2 RealBaseTilesPosition(int i = 0, int j = 0)
    {
        return new Vector2
        {
            x =  tilesSpacing * (Board.Rows / 2) + (Board.Rows % 2 == 0 ? tilesSpacing / 2f : 0) - i * tilesSpacing,
            y = -tilesSpacing * (Board.Cols / 2) + (Board.Cols % 2 == 0 ? tilesSpacing / 2f : 0) + j * tilesSpacing
        };
    }

    private void OnGameStart_L()
    {

    }

    private void OnTileConnect_L(TileController tile)
    {
        tiles_graphics[tile.Row, tile.Col].StateImage = rightImage;
        
        selectionLine.positionCount++;
        selectionLine.SetPosition(selectionLine.positionCount - 1, tile.transform.position + new Vector3(0, 0, -5));
    }

    private void OnTileDisconnect_L(TileController tile)
    {
        tiles_graphics[tile.Row, tile.Col].StateImage = null;

        selectionLine.positionCount--;
    }

    private void OnTilesSequenceStart_L()
    {
        
    }

    private void OnTilesSequenceCancel_L()
    {
        KoreKrush.Events.Graphics.TilesSequenceCancel_G();
    }

    private void OnTilesSequenceFinish_L()
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

        KoreKrush.Events.Graphics.TilesSequenceDestroy_G();
    }

    private void OnBoardRefill_Begin_L()
    {
        refillStage = 0;
    }

    private void OnBoardRefillStageStart_L()
    {
        refillStage++;
    }

    private void OnTileSpawn_L(TileController tile)
    {
        var p = RealBaseTilesPosition(i: tile.Row, j: tile.Col);

        var tile_graphics = tile.GetComponent<TileController_Graphics>();

        tiles_graphics[tile.Row, tile.Col] = tile_graphics;
        tile_graphics.transform.localPosition = new Vector3(p.y, p.x);

        var animDelay = refillStage * refillTime;

        tile_graphics.Sprite.DOColor(tilesColors[tile.color], refillTime)
            .SetDelay(animDelay)
            .SetEase(Ease.Linear);

        tile.transform.DOScale(0, refillTime)
            .From()
            .SetDelay(animDelay);
    }

    private void OnTileDisplace_L(TileController tile, Board.Cell from)
    {
        var p = RealBaseTilesPosition(i: tile.Row, j: tile.Col);

        var tile_graphics = tiles_graphics[from.row, from.col];

        tiles_graphics[tile.Row, tile.Col] = tile_graphics;

        var animDelay = refillStage * refillTime;

        tile.transform.DOLocalMove(new Vector3(p.y, p.x), refillTime)
            .SetDelay(animDelay)
            .SetEase(Ease.Linear);
    }

    private void OnBoardRefill_End_L()
    {
        refillStage = 0;
    }
}
