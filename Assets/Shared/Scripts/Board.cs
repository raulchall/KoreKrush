using System.Collections.Generic;
using UnityEngine;


namespace KoreKrush
{
    public static class Board
    {
        public class Cell
        {
            public int row, col;
            public TileController tile;

            public bool IsEmpty { get { return tile == null; } }
            public bool IsSpawningPoint { get { return row == 0; } }

            public bool AdjacentTo(Cell other)
            {
                return Mathf.Abs(row - other.row) + Mathf.Abs(col - other.col) == 1
                    || Mathf.Abs(row - other.row) == 1 && Mathf.Abs(col - other.col) == 1;
            }
        }

        public static Cell[,] cells;
        public static List<TileController> tilesSequence;
        public static int numberOfColors;

        public static int Rows
        {
            get { return cells.GetLength(0); }
        }

        public static int Cols
        {
            get { return cells.GetLength(1); }
        }

        public static TileController Last
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

        public static TileController SecondLast
        {
            get { return tilesSequence.Count > 1 ? tilesSequence[tilesSequence.Count - 2] : null; }
        }

        public static List<Cell> EmptyCells
        {
            get
            {
                var emptyCells = new List<Cell>();

                for (int i = 0; i < Rows; i++)
                    for (int j = 0; j < Cols; j++)
                        if (!cells[i, j].tile)
                            emptyCells.Add(cells[i, j]);

                return emptyCells;
            }
        }

        public static void ClearSelecteds()
        {
            tilesSequence.Clear();
        }
    }
}
