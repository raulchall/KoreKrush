using System.Collections.Generic;


namespace KoreKrush
{
    public static class Board
    {
        public class Cell
        {
            public int row, col;
            public TileController tile;
        }

        public static TileController[,] tiles;
        public static List<TileController> tilesSequence;
        public static int numberOfColors;

        public static int Rows
        {
            get { return tiles.GetLength(0); }
        }

        public static int Cols
        {
            get { return tiles.GetLength(1); }
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

        public static void Clear()
        {
            tilesSequence.Clear();
        }
    }
}
