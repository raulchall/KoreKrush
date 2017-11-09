using System.Collections.Generic;
using UnityEngine;


namespace KoreKrush
{
    [CreateAssetMenu(fileName="New Level", menuName="KoreKrush/Create Level")]
    public class Level : ScriptableObject
    {
        //TODO: Añadir al nivel el tablero inicial, asi como los tiles que en el aparecerán y la frecuencia de estos
        public List<LevelEvent> EventManager; //TODO: deberian poder ser cualquier tipo de eventos, no solo meteoritos

        public float Distance;
        public PieceList Objectives; 
        public int Turns;
        public float TurnTime;
        public float StartPosition;
        public string WorldSceneName;

        public void OnEnable ()
        {
            hideFlags = LocalHelper.globalFlag;
        }
    }
}
