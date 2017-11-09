using UnityEngine;


namespace KoreKrush
{
    [CreateAssetMenu(fileName="New Obstacle", menuName="KoreKrush/Create Obstacle")]
    public class Obstacle : SpeedObject
    {
        public int GearToBreak; //marcha que es necesaria completar para romperlo
        public float SpeedDamageWhenBreak; // cuando es roto le hace este daño a la velocidad de la nave
        public float SpeedDamagePerTimeUnit {
            get { 
                return this.SpeedDamageWhenBreak / 10 + this.Speed / 20; //SpeedDamageWhenBreak/a + Speed/b + c
            }
        }
        public float SpeedDamageTimeUnit = 1;
        //TODO:debilidades y fortalezas del meteorito
    }
}
