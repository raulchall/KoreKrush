using System;


namespace KoreKrush
{
    namespace Events
    {
        public static class Logic
        {
            public static Action
                BoardBuild_L,                   // the grid of tiles was initialized
                GameStart_L,                    // well... the game just... started
                TilesSequenceStart_L,           // a new sequence of tiles has begun
                TilesSequenceFinish_L,          // the user chose to finish the current sequence
                TilesSequenceCancel_L;          // the sequence of tiles was formed by a single tile
            public static Action<TileController>
                TileSpawn_L,                    // a tile appears on the board
                TileHover_L,                    // the cursor passed over a tile
                TileSelect_L,                   // the user clicked a tile
                TileConnect_L,                  // a new tile joined to the sequence
                TileDisconnect_L,               // the last tile on the sequence was removed from it
                TileDestroy_L;                  // a tile was removed from the board
            public static Action<TileController, Board.Cell>
                TileDisplace_L;                 // a tile was moved from one to another cell
        }

        public static class Graphics
        {
            public static Action
                BoardBuild_G,                   // every tile was placed on the scene
                TilesSequenceCancel_G,          // the sequence of tiles was formed by a single tile
                TilesSequenceDestroy_G;         // the final sequence of tiles was removed
        }
    }
}
