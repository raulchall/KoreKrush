using System;
using UnityEngine;

namespace KoreKrush
{
    namespace Events
    {
        public static class Logic
        {
            public static Action
                GameStart_L,                    // well... the game just... started
                BoardBuild_L,                   // the grid of tiles was initialized
                BoardRefill_Begin_L,            // the board refill process has begun
                BoardRefill_End_L,              // the board has been refilled
                BoardRefillStageStart_L,        // a new group of tiles actions (spawn or move) start
                TilesSequenceStart_L,           // a new sequence of tiles has begun
                TilesSequenceFinish_L,          // the user chose to finish the current sequence
                TilesSequenceCancel_L;          // the sequence of tiles was formed by a single tile

            public static Action<StandardTile>
                TileSpawn_L,                    // a tile appears on the board
                TileHover_L,                    // the cursor passed over a tile
                TileSelect_L,                   // the user clicked a tile
                TileConnect_L,                  // a new tile joined to the sequence
                TileDisconnect_L,               // the last tile on the sequence was removed from it
                TileDestroy_L;                  // a tile was removed from the board
            public static Action<StandardTile, Board.Cell>
                TileDisplace_L;                 // a tile was moved from one to another cell

			public static Action<ObstacleManager> 
				ObstacleSpawn,                  // a metheor apears in the way
				ShipObstacleCollision,          // ship and an obstacle enter in collision
				ShipCollisionStart;             // ship start collision
			 	
			public static Action
				MetheorCollisionStart,  
				ShipWarpStart,   				// ship start warp mode
				ShipWarpEnd,   					// ship end warp mode
				LevelCompleted,                 // level objectives and distance are beated
				TurnsOut,                       // count of turns become 0
				PlayerDefeat,					// ship lose the battle agains the meteor
				ShipCollisionFinish;		    // ship contine traveling
					

			public static Action<PieceList>
				ObjectivesUiBuild,			    // the objectives UI was initialized
				ObjectivesUpdate,               // Objectives was updated
				ManageSpeed;					// the motors convert tiles in speed

			public static Action<int>
				TurnsUpdate;					// change the turns of the actual level

			public static Action<float>
				SpeedMultiply,                  // ship receive a multiplied bonus to their speed
				SpeedAdd,                       // ship gain a speed bonus and will be Added without transformations  
				SpeedSubtract;         		    // ship lose speed
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
