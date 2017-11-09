using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

 
namespace KoreKrush
{
	public static class LocalHelpers
	{
		public static HideFlags globalFlag = HideFlags.DontUnloadUnusedAsset | HideFlags.DontSaveInEditor;

        public static float Multiplier(int count)
        {
            if (count < 7)
                return 1;
            else if (count < 11)
                return 1.25f;
            else if (count < 16)
                return 1.5f;
            else
                return 2;
        }

        public static float VirtualSpeedToPathSpeed(float virtualSpeed)
        {
            float a1 = 0.04f / 4950;
            float a0 = 0.01f - 50 * a1;
            return a0 + virtualSpeed * a1;
        }

        public static float VirtualDistanceToPathDistance(float virtualDistance, float pathSize, float virtualPathSize)
        {
            return (virtualDistance * pathSize) / virtualPathSize;
        }

    }

	[CreateAssetMenu(fileName="New Level", menuName="KoreKrush/Create Level")]
	[Serializable]
	public class Level:ScriptableObject
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
			hideFlags = LocalHelpers.globalFlag;
		}
	}

	[CreateAssetMenu(fileName="New Obstacle", menuName="KoreKrush/Create Obstacle")]
	public class Obstacle: SpeedObject
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

		public void OnEnable ()
		{
			hideFlags = LocalHelpers.globalFlag;
		}

	}

	[CreateAssetMenu(fileName="New Ship", menuName="KoreKrush/Create Ship")]
	[Serializable]
	public class Ship: ScriptableObject
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
			hideFlags = LocalHelpers.globalFlag;
		}
	}

	[CreateAssetMenu(fileName="New Motor", menuName="KoreKrush/Create Motor")]
	[Serializable]
	public class Motor: ScriptableObject
    {

		public float Multiplier;
		public TileType Tile; //TODO: en un futuro un motor podria servir con mas de un tile.... o NO!!!!
		public int PowerFillCount = 15;

        public GameObject TileGenerated;  //TODO: no necesariamente la habilidad de un motor tiene que ser generar un nuevo tile, pero para ello debo serializar delegados

        public void OnEnable ()
		{
			hideFlags = LocalHelpers.globalFlag;
		}
	}

	[Serializable]
	public class PieceList: ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TileType, int>>
	{
		[SerializeField]
		private Dictionary<TileType, int> d_list;
		public List<PieceElems> list;
		public int Count {
			get{
				int c = 0;
				foreach (var item in d_list) {
					c += item.Value;
				}
				return c;
			}
		}

		public int LCount {
			get{
				return d_list.Keys.Count;
			}
		}

		public PieceList ()
		{
			list = new List<PieceElems> ();
			d_list = new Dictionary<TileType, int> ();
		}

		public PieceList (PieceList pl)
		{
			list = new List<PieceElems> ();
			d_list = new Dictionary<TileType, int> ();

			foreach (var item in pl) {
				Add (item.Key, item.Value);
			}
		}

		public void Add(TileType item, int _Count = 1)
		{
			if (d_list.ContainsKey (item)) {
				d_list [item] += _Count;
			} else {
				d_list.Add (item, _Count);
			}
		}

		public void Subtract(PieceList loot)
		{
			foreach (var item in loot.d_list) {
				if (d_list.ContainsKey (item.Key)) {
					var rest = d_list [item.Key] - loot [item.Key];
					if (rest < 0)
						rest = 0;
					d_list [item.Key] = rest;

					//TODO: hacer que se elimine del diccionario si lo quiero así y no se estropee la parte visual
//					if (d_list [item.Key] == 0)
//						d_list.Remove (item.Key);
				}
			}
		}

		public int this [TileType index]
		{
			get {
				if(d_list.ContainsKey(index))
					return d_list[index];
				return 0;
			}
			set{
				d_list [index] = value;
				if (value <= 0)
					d_list [index] = 0;
			}
		}

		public bool ContainsKey(TileType p)
		{
			return d_list.ContainsKey (p);
		}

		public void OnBeforeSerialize ()
		{
			list = new List<PieceElems> ();

			foreach (var item in d_list) {
				list.Add (new PieceElems (item.Key, item.Value));
			}

		}

		public void OnAfterDeserialize ()
		{
			d_list = new Dictionary<TileType, int> ();

			foreach (var item in list) {
				d_list[item.Key] = item.Count;
			}
		}

		#region IEnumerable implementation

		public IEnumerator<KeyValuePair<TileType, int>> GetEnumerator ()
		{
			return d_list.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return d_list.GetEnumerator();
		}

		#endregion
	}

	[Serializable]
	public class PieceElems
	{
		public TileType Key;
		public int Count;

		public PieceElems (TileType k, int c)
		{
			Key = k;
			Count = c;
		}

		public PieceElems (PieceElems p)
		{
			Key = p.Key;
			Count = p.Count;
		}
	}

	[Serializable]
	public class LevelEvent
	{

		public float PathPosition;
		public SpeedObject Obj;

		public List<PieceReward> Rewards;
		//TODO: que funcione para todo tipor de rewards
		public float MinRewardTime;
		public float MaxRewardTime;
	}

	[Serializable]
	public class RewardEvent
	{
		public List<PieceReward> Rewards;
		//TODO: que funcione para todo tipor de rewards
		public float MinRewardTime;
		public float MaxRewardTime;
	}

	[Serializable]
	public class LevelEventManager: IEnumerable<LevelEvent>
	{
		public List<LevelEvent> Events_list;

		public LevelEventManager ()
		{
			Events_list = new List<LevelEvent> ();
		}

		public LevelEventManager (List<LevelEvent> l)
		{
			Events_list = l;
		}

		public void SubscribeEvent(LevelEvent e){
			Events_list.Add (e);
		}

		#region IEnumerable implementation
		public IEnumerator<LevelEvent> GetEnumerator ()
		{
			return Events_list.GetEnumerator();
		}
		#endregion
		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return Events_list.GetEnumerator();
		}
		#endregion
	}

	[Serializable]
	public class SpeedObject: ScriptableObject
	{
		public GameObject prefab;
		public float Speed;
	} 

	[Serializable]
	public class Gear
	{
		public float additional_base_speed;
		public float speed_breaker; //amount of speed to break gear limit and advance to the next gear
		public float speed_lost_per_second;


		public Gear (float base_speed, float speed_breaker, float spd)
		{
			this.additional_base_speed = base_speed;
			this.speed_breaker = speed_breaker;
			this.speed_lost_per_second = spd;
		}

	}

	[Serializable]
	public class Reward
	{
		public int Count;
	}

	[Serializable]
	public class PieceReward: Reward
	{
		public TileType tile;

		public PieceReward (TileType p, int c)
		{
			tile = p;
			Count = c;
		}
	}

	[Serializable]
	public class TurnReward: Reward
	{
		public int turns;

		public TurnReward (int t, int c)
		{
			turns = t;
			Count = c;
		}
	}
}
