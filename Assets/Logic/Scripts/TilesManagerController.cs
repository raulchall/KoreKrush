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

        KoreKrush.Events.Logic.TileSelected_L               += OnTileSelected_L;
        KoreKrush.Events.Graphics.BoardBuilt_G              += OnBoardBuilt_G;
        KoreKrush.Events.Graphics.TilesSequenceCanceled_G   += OnTilesSequenceCanceled_G;
        KoreKrush.Events.Graphics.TilesSequenceDestroyed_G  += OnTilesSequenceDestroyed_G;
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

    private void OnTilesSequenceCanceled_G()
    {
        Board.ClearSelecteds();
    }

    private void OnTilesSequenceDestroyed_G()
    {
        Board.tilesSequence.ForEach(t => { t.cell.IsEmpty = true; Destroy(t.gameObject); } );
        Board.ClearSelecteds();

        RefillBoard();
    }

    private void RefillBoard()
    {
        Board.EmptyCells.ForEach(TryFillCell);
    }

    private void TryFillCell(Board.Cell cell)
    {
        
        if (cell.IsEmpty)
        {
            var fillerCell = GetFillerCell(of: cell);

            if (fillerCell != null)
            {
                TryFillCell(fillerCell);

                if (!fillerCell.IsEmpty)
                {
                    DisplaceTile(from: fillerCell, to: cell);

                    TryFillCell(fillerCell);
                }
            }
            else if (cell.IsSpawningPoint)
                SpawnNewTile(on: cell);
        }
    }

    private Board.Cell GetFillerCell(Board.Cell of)
    {
        if (of.row > 0)
        {
            var cell1 = Board.cells[of.row - 1, of.col];

            if (cell1.IsEmpty || cell1.tile.IsMovable) return cell1;

            if (of.col > 0)
            {
                var cell2 = Board.cells[of.row - 1, of.col - 1];

                if (cell2.IsEmpty || cell2.tile.IsMovable) return cell2;
            }

            if (of.col < Board.Cols - 1)
            {
                var cell3 = Board.cells[of.row - 1, of.col + 1];

                if (cell3.IsEmpty || cell3.tile.IsMovable) return cell3;
            }
        }

        return null;
    }

    private void SpawnNewTile(Board.Cell on)
    {
        var tile = Instantiate(tilesPrefab, tilesContainer)
            .GetComponent<TileController>();

        tile.cell = on;
        on.tile = tile;
        tile.color = Random.Range(0, numberOfColors);

        KoreKrush.Events.Logic.TileSpawned_L(tile);
    }

    private void DisplaceTile(Board.Cell from, Board.Cell to)
    {
        from.tile.cell = to;
        to.tile = from.tile;
        from.tile = null;

        KoreKrush.Events.Logic.TileDisplaced_L(to.tile, from);
    }
}
