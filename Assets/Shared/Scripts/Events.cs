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
            public static event Action 
                GameStarted;
            public static event Action<TileController>
                TileSpawned,
                TileHovered,
                TileSelected,
                TileConnected,
                TileDisconnected,
                TileDisplaced,
                TileDestroyed;
            public static event Action<List<TileController>>
                TilesSequenceStarted,
                TilesSequenceCompleted;
        }

        public static class Graphics
        {
            
        }
    }
}
