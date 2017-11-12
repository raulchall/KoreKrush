using System.Collections.Generic;
using UnityEngine;


namespace KoreKrush
{    
    public static class Board
    {
        public class Cell
        {
            public int row, col;
            public Vector2 Pos;
            public bool usedInCurrentStage;

            public StandardTile Tile
            {
                get { return Tiles.Peek(); }
                set
                {
                    ClearTiles(true);

                    if (!value) return;
                    
                    value.Cell = this;
                    Tiles.Push(value);
                }
            }
            public bool IsEmpty { get { return Tile == null; } }
            public bool IsSpawningPoint { get { return row == 0; } }
            
            private readonly LinkedList<StandardTile> Tiles = new LinkedList<StandardTile>();

            public bool AdjacentTo(Cell other)
            {
                return Mathf.Abs(row - other.row) + Mathf.Abs(col - other.col) == 1
                    || Mathf.Abs(row - other.row) == 1 && Mathf.Abs(col - other.col) == 1;
            }

            public void PushTile(StandardTile tile, bool hideTop = true)
            {
                var topTile = Tile;
                if (topTile && hideTop) topTile.gameObject.SetActive(false);

                tile.Cell = this;
                Tiles.Push(tile);
            }

            public StandardTile PopTile(bool showTop = true)
            {
                var tile = Tiles.Pop();
                var topTile = Tiles.Peek();
                
                if (topTile && showTop) topTile.gameObject.SetActive(true);
                
                if (!tile) Debug.LogWarning(string.Format("Cannot Pop from empty Cell [{0}, {1}]", row, col));
                return tile;
            }

            public void ClearTiles(bool destroyTiles = false)
            {
                if (destroyTiles)
                    foreach (var t in Tiles)
                        Object.Destroy(t.gameObject);
                
                Tiles.Clear();
            }
        }

        public static Cell[,] Cells;
        public static List<StandardTile> tilesSequence;

        public static int Rows
        {
            get { return Cells.GetLength(0); }
        }

        public static int Cols
        {
            get { return Cells.GetLength(1); }
        }

        public static StandardTile Last
        {
            get { return tilesSequence.Count > 0 ? tilesSequence[tilesSequence.Count - 1] : null; }
            set 
            {
                if (value) 
                    tilesSequence.Add(value);
                else 
                    tilesSequence.RemoveAt(tilesSequence.Count - 1); 
            }
        }

        public static StandardTile SecondLast
        {
            get { return tilesSequence.Count > 1 ? tilesSequence[tilesSequence.Count - 2] : null; }
        }

        public static List<Cell> EmptyCells
        {
            get
            {
                var emptyCells = new List<Cell>();

                for (var j = 0; j < Cols; j++)
                    for (var i = Rows - 1; i >= 0; i--)
                        if (!Cells[i, j].Tile)
                            emptyCells.Add(Cells[i, j]);

                return emptyCells;
            }
        }

        public static Cell RandomCell
        {
            get
            {
                var r = Random.Range(0, Cells.GetLength(0) - 1);
                var c = Random.Range(0, Cells.GetLength(1) - 1);

                return Cells[r, c];
            }
        }

        public static void ClearSelecteds()
        {
            tilesSequence.Clear();
        }
    }
}
