using System;
using UnityEngine;


namespace KoreKrush
{
    [CreateAssetMenu(fileName="New Motor", menuName="KoreKrush/Create Motor")]
    public class Motor : ScriptableObject
    {
        public float Multiplier;
        public TileType Tile; //TODO: en un futuro un motor podria servir con mas de un tile.... o NO!!!!
        public int PowerFillCount = 15;
        public MotorAbility ability;
    }
}
