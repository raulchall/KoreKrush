using System;
using UnityEngine;

namespace KoreKrush
{
    namespace Events
    {
        public static class Logic
        {
            public static Action
                BoardBuilt_L,                   // the grid of tiles was initialized
                GameStarted_L,                  // well... the game just... started
                TilesSequenceStarted_L,         // a new sequence of tiles has begun
                TilesSequenceCompleted_L,       // the user chose to finish the current sequence
				TilesSequenceCanceled_L;        // the sequence of tiles was formed by a single tile
            public static Action<TileController>
                TileSpawned_L,                  // a tile appears on the board
                TileHovered_L,                  // the cursor passed over a tile
                TileSelected_L,                 // the user clicked a tile
                TileConnected_L,                // a new tile joined to the sequence
                TileDisconnected_L,             // the last tile on the sequence was removed from it
                TileDestroyed_L;                // a tile was removed from the board
            public static Action<TileController, Board.Cell>
                TileDisplaced_L;                // a tile was moved from one to another cell

			public static Action<LevelEvent> 
				MetheorAnnounce,                // a metheor apears in the way
				ShipObstacleCollision;          // ship and an obstacle enter in collision

			public static Action
				ShipCollisionStarted,           // ship start collision
				MetheorCollisionStarted,  
				WarpStarted,   					// ship start warp mode
				WarpEnded,   					// ship end warp mode
				LevelCompleted,                 // level objectives and distance are beated
				TurnsOut;                       // count of turns become 0

			public static Action<PieceList>
				ObjectivesUiBuilt,			    // the objectives UI was initialized
				ObjectivesUpdated,              // Objectives was updated
				ManageSpeed;					// the motors convert tiles in speed

			public static Action<int>
				TurnsUpdated;					// change the turnos of the actual level

			public static Action<float>
				SpeedMultiplied,                // ship receive a multiplied bonus to their speed
				SpeedAdded,                     // ship gain a speed bonus and will be Added without transformations  
				SpeedSubtracted;         		// ship lose speed
        }

        public static class Graphics
        {
            public static Action
                BoardBuilt_G,                   // every tile was placed on the scene
                TilesSequenceCanceled_G,        // the sequence of tiles was formed by a single tile
                TilesSequenceDestroyed_G;       // the final sequence of tiles was removed


        }
    }
}
