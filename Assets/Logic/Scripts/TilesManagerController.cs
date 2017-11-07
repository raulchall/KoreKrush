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
    public int           Rows = 7;
    public int           Cols = 7;

    private const float  RefillTime   = .3f;
    private const int    TilesSpacing = 15;
    private int          RefillStage;
    private GameObject[] TilesPrefabs;
    private RawImage     Splash;
    private LineRenderer SelectionLine;


    private void Awake()
    {
        DOTween.Init(true);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name + "_Graphics", LoadSceneMode.Additive);
        
        SelectionLine = GetComponent<LineRenderer>();
        Splash = Instantiate(Resources.Load<GameObject>("Splash")).GetComponent<RawImage>();
        TilesPrefabs = Resources.LoadAll<GameObject>("Tiles/Standards Tiles");

        Logic.TileSelect_L   += OnTileSelect_L;
        Logic.MotorTileSpawn += OnMotorTileSpawn;
    }

	private void OnDestroy()
	{
		Logic.TileSelect_L   -= OnTileSelect_L;
	    Logic.MotorTileSpawn -= OnMotorTileSpawn;
	}

    private void Start()
    {
        Splash.DOColor(Color.clear, 1);
        
        BuildBoard();
    }
	
    private void Update()
    {
        CheckTilesSequenceCompleted();
    }

    private void BuildBoard()
    {
        Board.Cells = new Board.Cell[Rows, Cols];
        Board.tilesSequence = new List<StandardTile>();

        for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Cols; j++)
                Board.Cells[i, j] = new Board.Cell
                {
                    row = i,
                    col = j,
                    Pos = TileWorldPosition(i, j)
                };

        RefillBoard();
    }

    private void OnTileSelect_L(StandardTile tile)
    {
        var lastTile = Board.Last;

        if (!lastTile)
            ConnectTile(tile);
        else if (tile == Board.SecondLast)
            DisconnectLastTile();
        else if ((tile.IsCompatible(lastTile) || lastTile.IsCompatible(tile)) && 
                 tile.IsAdjacent(lastTile) && 
                 !tile.IsConnected)
            ConnectTile(tile);
    }

    private void OnMotorTileSpawn(GameObject tilePrefab)
    {
        var cell = Board.RandomCell;
        var tile = Instantiate(tilePrefab, transform).GetComponent<StandardTile>();

        tile.transform.position = cell.Tile.transform.position;
        cell.PushTile(tile);
    }

    private void ConnectTile(StandardTile tile)
    {
        var newStart = Board.Last == null;
        
        Board.Last = tile;
        tile.Connect(.2f, 0);
    
//        Logic.TileConnect_L(tile);
//        if (newStart) Logic.TilesSequenceStart_L();
        
        SelectionLine.positionCount++;
        SelectionLine.SetPosition(SelectionLine.positionCount - 1, 
            tile.transform.position + new Vector3(0, 0, -5));
    }

    private void DisconnectLastTile()
    {
        var lastTile = Board.Last;

        Board.Last = null;
        lastTile.Disconnect(.2f, 0);

//        Logic.TileDisconnect_L(lastTile);
        
        SelectionLine.positionCount--;
    }

    private void CheckTilesSequenceCompleted()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        
        if (Board.tilesSequence.Count > 1)
        {
            StartCoroutine(DestroySelectedTiles());
            
            Logic.TilesSequenceFinish_L();
        }
        else
        {
            var tile = Board.Last;
            tile.Disconnect(.2f, 0);
            
            Board.ClearSelecteds();

            SelectionLine.positionCount = 0;

//            Logic.TileDisconnect_L(tile);
//            Logic.TilesSequenceCancel_L();
        }
    }
    
    private IEnumerator DestroySelectedTiles()
    {
        SelectionLine.positionCount = 0;

        foreach (var tile in Board.tilesSequence)
        {
            yield return new WaitForSeconds(.05f);

            tile.Cell.Tile = null;
            Destroy(tile.gameObject);
        }

        Board.ClearSelecteds();

        RefillBoard();
    }

    #region Refill board API
    private void RefillBoard()
    {
        RefillStage = 0;
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

            RefillStage++;
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
            MoveTile(from: fillerCell, to: cell);
            emptyCells.Add(fillerCell);
        }
        else if (cell.IsSpawningPoint)
            SpawnNewTile(on: cell);
        else return false;

        return true;
    }

    private Board.Cell GetFillerCell(Board.Cell of)
    {
        if (of.row == 0) return null;
        
        var cell1 = Board.Cells[of.row - 1, of.col];

        if (cell1.IsEmpty || cell1.Tile.IsMovable) return cell1;

        if (of.col > 0)
        {
            var cell2 = Board.Cells[of.row - 1, of.col - 1];

            if (cell2.IsEmpty || cell2.Tile.IsMovable) return cell2;
        }

        if (of.col >= Board.Cols - 1) return null;
        
        var cell3 = Board.Cells[of.row - 1, of.col + 1];

        if (cell3.IsEmpty || cell3.Tile.IsMovable) return cell3;

        return null;
    }

    private void MoveTile(Board.Cell from, Board.Cell to)
    {
        var tile = from.Tile;
        
        tile.Cell = to;
        to.Tile = tile;
        from.Tile = null;

        var newPos = TileWorldPosition(i: tile.Row, j: tile.Col);

        var animDelay = RefillStage * RefillTime;
        
        tile.Move(newPos, RefillTime, animDelay);
    }

    private void SpawnNewTile(Board.Cell on)
    {
        var tile = Instantiate(TilesPrefabs.Choice(), transform)
            .GetComponent<StandardTile>();

        tile.Cell = on;
        on.Tile = tile;

        tile.transform.localPosition = TileWorldPosition(i: tile.Row, j: tile.Col);

        var animDelay = RefillStage * RefillTime;
        
        tile.Spawn(RefillTime, animDelay);
    }

    #endregion
    
    private Vector2 TileWorldPosition(int i = 0, int j = 0)
    {
        return new Vector2
        {
            x = -TilesSpacing * (Board.Cols / 2) + (Board.Cols % 2 == 0 ? TilesSpacing / 2f : 0) + j * TilesSpacing,
            y =  TilesSpacing * (Board.Rows / 2) + (Board.Rows % 2 == 0 ? TilesSpacing / 2f : 0) - i * TilesSpacing
        };
    }
}
