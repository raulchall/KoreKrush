using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

using KoreKrush;
using KoreKrush.Events;


public class TilesManagerController : MonoBehaviour
{
    public int Rows = 7;
    public int Cols = 7;
    public Color[] Colors;

    private GameObject tilesPrefab;
    private RawImage splash;
    private const int tilesSpacing = 15;
    private const float refillTime = .3f;
    private int refillStage;
    private LineRenderer selectionLine;


    private void Awake()
    {
        DOTween.Init(true);
        DOTween.defaultAutoKill = false;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name + "_Graphics", LoadSceneMode.Additive);
        
        selectionLine = GetComponent<LineRenderer>();
        splash = GameObject.Find("Splash").GetComponent<RawImage>();
        tilesPrefab = Resources.Load<GameObject>("Tile");

        Logic.TileSelect_L += OnTileSelect_L;
    }

	private void OnDestroy()
	{
		Logic.TileSelect_L -= OnTileSelect_L;
	}

    private void Start()
    {
        splash.DOColor(Color.clear, 1);
        
        BuildBoard();
    }
	
    private void Update()
    {
        CheckTilesSequenceCompleted();
    }

    private void BuildBoard()
    {
        Board.Cells = new Board.Cell[Rows, Cols];
        Board.tilesSequence = new List<TileController>();
        Board.numberOfColors = Colors.Length;

        for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Cols; j++)
            {
                var tile = Instantiate(tilesPrefab, transform)
                    .GetComponent<TileController>();
                
                var cell = new Board.Cell
                {
                    tile = tile,
                    row = i,
                    col = j
                };
                
                tile.Cell = Board.Cells[i, j] = cell;
                tile.Color = Colors.Choice();
                tile.transform.localPosition = TileWorldPosition(i, j);
            }
    }

    private void OnTileSelect_L(TileController tile)
    {
        var lastTile = Board.Last;

        if (!lastTile)
            ConnectTile(tile);
        else if (tile == Board.SecondLast)
            DisconnectLastTile();
        else if (tile.IsCompatible(lastTile) && tile.IsAdjacent(lastTile) && !tile.IsConnected)
            ConnectTile(tile);
    }

    private void ConnectTile(TileController tile)
    {
        var newStart = Board.Last == null;
        
        Board.Last = tile;
        tile.Connect();
    
//        Logic.TileConnect_L(tile);
//        if (newStart) Logic.TilesSequenceStart_L();
        
        selectionLine.positionCount++;
        selectionLine.SetPosition(selectionLine.positionCount - 1, 
            tile.transform.position + new Vector3(0, 0, -5));
    }

    private void DisconnectLastTile()
    {
        var lastTile = Board.Last;

        Board.Last = null;
        lastTile.Disconnect();

//        Logic.TileDisconnect_L(lastTile);
        
        selectionLine.positionCount--;
    }

    private void CheckTilesSequenceCompleted()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        
        if (Board.tilesSequence.Count > 1)
        {
            StartCoroutine(DestroySelectedTiles());
            
//            Logic.TilesSequenceFinish_L();
        }
        else
        {
            var tile = Board.Last;
            tile.Disconnect();
            
            Board.ClearSelecteds();

//            Logic.TileDisconnect_L(tile);
//            Logic.TilesSequenceCancel_L();
        }
    }
    
    private IEnumerator DestroySelectedTiles()
    {
        selectionLine.positionCount = 0;

        foreach (var tile in Board.tilesSequence)
        {
            yield return new WaitForSeconds(.05f);

            tile.Cell.IsEmpty = true;
            Destroy(tile.gameObject);
        }

        Board.ClearSelecteds();

        RefillBoard();
    }

    #region Refill board API
    private void RefillBoard()
    {
        refillStage = 0;
        bool boardChanged;

        do
        {
            boardChanged = false;

            var emptyCells = Board.EmptyCells;

            for (var i = 0; i < emptyCells.Count; i++)
            {
                var cell = emptyCells[i];

                boardChanged = TryFillCell(cell, emptyCells) || boardChanged;

                cell.usedInCurrentStage = true;
            }

            emptyCells.ForEach(c => c.usedInCurrentStage = false);

            refillStage++;
        }
        while (boardChanged);
        
    }

    private bool TryFillCell(Board.Cell cell, List<Board.Cell> emptyCells)
    {
        var fillerCell = GetFillerCell(of: cell);

        if (fillerCell != null && 
            !fillerCell.usedInCurrentStage && 
            !fillerCell.IsEmpty)
        {
            DisplaceTile(from: fillerCell, to: cell);
            emptyCells.Add(fillerCell);
        }
        else if (cell.IsSpawningPoint)
            SpawnNewTile(on: cell);
        else return false;

        return true;
    }

    private Board.Cell GetFillerCell(Board.Cell of)
    {
        if (of.row > 0)
        {
            var cell1 = Board.Cells[of.row - 1, of.col];

            if (cell1.IsEmpty || cell1.tile.IsMovable) return cell1;

            if (of.col > 0)
            {
                var cell2 = Board.Cells[of.row - 1, of.col - 1];

                if (cell2.IsEmpty || cell2.tile.IsMovable) return cell2;
            }

            if (of.col < Board.Cols - 1)
            {
                var cell3 = Board.Cells[of.row - 1, of.col + 1];

                if (cell3.IsEmpty || cell3.tile.IsMovable) return cell3;
            }
        }

        return null;
    }

    private void DisplaceTile(Board.Cell from, Board.Cell to)
    {
        var tile = from.tile;
        
        tile.Cell = to;
        to.tile = tile;
        from.tile = null;

        var newPos = TileWorldPosition(i: tile.Row, j: tile.Col);

        var animDelay = refillStage * refillTime;
        tile.transform.DOLocalMove(newPos, refillTime)
            .SetDelay(animDelay)
            .SetEase(Ease.Linear);
    }

    private void SpawnNewTile(Board.Cell on)
    {
        var tile = Instantiate(tilesPrefab, transform)
            .GetComponent<TileController>();

        tile.Cell = on;
        on.tile = tile;
        tile.Color = Colors.Choice();

        tile.transform.localPosition = TileWorldPosition(i: tile.Row, j: tile.Col);

        var animDelay = refillStage * refillTime;

        tile.transform.DOLocalMoveZ(10, refillTime)
            .From()
            .SetDelay(animDelay);

        tile.Sprite.DOColor(Color.clear, refillTime)
            .From()
            .SetDelay(animDelay)
            .SetEase(Ease.Linear);

        tile.transform.DOScale(0, refillTime)
            .From()
            .SetDelay(animDelay);
    }

    #endregion
    
    private Vector2 TileWorldPosition(int i = 0, int j = 0)
    {
        return new Vector2
        {
            x = -tilesSpacing * (Board.Cols / 2) + (Board.Cols % 2 == 0 ? tilesSpacing / 2f : 0) + j * tilesSpacing,
            y =  tilesSpacing * (Board.Rows / 2) + (Board.Rows % 2 == 0 ? tilesSpacing / 2f : 0) - i * tilesSpacing
        };
    }
}
