﻿using System.Collections;
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

    private const int    TilesSpacing = 15;
    private const float  DestructionStage_Duration = .1f;
    private const float  RefillStage_Duration = .2f;
    private int          RefillStage_Count;
    private bool         ProcessInput;
    private GameObject[] TilesPrefabs;
    private RawImage     Splash;
    private LineRenderer SelectionLine;


    private void Awake()
    {
        DOTween.Init(true);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name + "_Graphics", LoadSceneMode.Additive);
        
        SelectionLine = GetComponent<LineRenderer>();
        Splash = Instantiate(Resources.Load<GameObject>("Splash")).GetComponent<RawImage>();
        TilesPrefabs = Resources.LoadAll<GameObject>("Tiles/Standard Tiles");

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
        
        RefillBoard(BeginTurn);
    }
	
    private void Update()
    {
        CheckTilesSequenceCompleted();
    }

    private void BeginTurn()
    {
        ProcessInput = true;
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
    }

    private void OnTileSelect_L(StandardTile tile)
    {
        if (!ProcessInput) return;
        
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

        tile.transform.SetAs(cell.Tile.transform, setScale: false);

        cell.Tile = tile;
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
        if (!Input.GetMouseButtonUp(0) || !ProcessInput) return;
        
        if (Board.tilesSequence.Count > 1)
            EndTurn();
        else
        {
            var tile = Board.Last;

            if (!tile) return;  // There are no selected tiles
            
            tile.Disconnect(.2f, 0);
            
            Board.ClearSelecteds();

            SelectionLine.positionCount = 0;

//            Logic.TileDisconnect_L(tile);
//            Logic.TilesSequenceCancel_L();
        }
    }

    private void EndTurn()
    {
        ProcessInput = false;
        
        Logic.TilesSequenceFinish_L();

        StartCoroutine(EndTurnAsync());
    }

    private IEnumerator EndTurnAsync()
    {
        yield return DestroySelectedTiles();
        
        RefillBoard(BeginTurn);
    }
    
    private IEnumerator DestroySelectedTiles()
    {
        SelectionLine.positionCount = 0;

        foreach (var tile in Board.tilesSequence)
        {
            // Check nothing has been placed in
            // this cell because it will be removed
            if (tile != tile.Cell.Tile) continue;
            
            tile.Destroy(DestructionStage_Duration, 0);

            yield return new WaitForSeconds(DestructionStage_Duration);
        }

        Board.ClearSelecteds();
    }

    #region Refill board API
    private void RefillBoard(TweenCallback callback)
    {
        RefillStage_Count = 0;
        
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

            RefillStage_Count++;
        }
        while (boardChanged);

        DOVirtual.DelayedCall(RefillStage_Duration * (RefillStage_Count - 1), callback, false);
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
        from.ClearTiles();

        var newPos = TileWorldPosition(i: tile.Row, j: tile.Col);

        var animDelay = RefillStage_Count * RefillStage_Duration;
        
        tile.Move(newPos, RefillStage_Duration, animDelay);
    }

    private void SpawnNewTile(Board.Cell on)
    {
        var tile = Instantiate(TilesPrefabs.Choice(), transform)
            .GetComponent<StandardTile>();

        tile.Cell = on;
        on.Tile = tile;

        tile.transform.localPosition = TileWorldPosition(i: tile.Row, j: tile.Col);

        var animDelay = RefillStage_Count * RefillStage_Duration;
        
        tile.Spawn(RefillStage_Duration, animDelay);
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
