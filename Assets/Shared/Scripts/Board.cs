using System.Collections.Generic;
using UnityEngine;


namespace KoreKrush
{    
    public static class Board
    {
        public class Cell
        {
            public int row, col;
            public BaseTile tile;
            public bool usedInCurrentStage;

            public bool IsEmpty
            {
                get { return tile == null; }
                set { if (value) tile = null; }
            }
            public bool IsSpawningPoint { get { return row == 0; } }

            public bool AdjacentTo(Cell other)
            {
                return Mathf.Abs(row - other.row) + Mathf.Abs(col - other.col) == 1
                    || Mathf.Abs(row - other.row) == 1 && Mathf.Abs(col - other.col) == 1;
            }
        }

        public static Cell[,] Cells;
        public static List<BaseTile> tilesSequence;

        public static int Rows
        {
            get { return Cells.GetLength(0); }
        }

        public static int Cols
        {
            get { return Cells.GetLength(1); }
        }

        public static BaseTile Last
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

        public static BaseTile SecondLast
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
                        if (!Cells[i, j].tile)
                            emptyCells.Add(Cells[i, j]);

                return emptyCells;
            }
        }

        public static void ClearSelecteds()
        {
            tilesSequence.Clear();
        }
    }
}
