using System;
using System.Collections.Generic;
using UnityEngine;


namespace KoreKrush
{
    [CreateAssetMenu(fileName="New Ship", menuName="KoreKrush/Create Ship")]
    public class Ship : ScriptableObject
    {
        public List<Gear> GearsBox;
        public List<Motor> Motors;
        public float MinSpeed;
        public string Prefab_Path;
        public float WarpDuration;
        public float WarpBreakDamage;
        public float MaxSpeed;

        public void OnEnable ()
        {
            hideFlags = LocalHelper.globalFlag;
        }
    }
}
