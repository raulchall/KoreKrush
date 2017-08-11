using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace KoreKrush
{
    namespace Events
    {
        public static class Logic
        {
            public static Action 
                GameStarted;  // well... the game just... started
            public static Action<TileController>
                TileSpawned, // a tile appears on the board
                TileHovered, // the cursor passed over a tile
                TileSelected, // the user clicked a tile
                TileConnected, // a new tile joined to the sequence
                TileDisconnected, // the last tile on the sequence was removed from it
                TileDestroyed;  // a tile was removed from the board
            public static Action<TileController, int, int>
                TileDisplaced;  // a tile was moved from one to another position
            public static Action<List<TileController>>
                TilesSequenceStarted, // a new sequence of tiles has begun
                TilesSequenceCompleted;  // the user chose to finish the current sequence
        }

        public static class Graphics
        {
            
        }
    }
}
