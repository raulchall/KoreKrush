using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

 
namespace KoreKrush
{
	public static class LocalHelper
	{
		public static HideFlags globalFlag = HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset;
	}

	public enum Piece //TODO: las piezas no se deberian definir por el color, podrian ser cualquier cosa
	{
		blue = 0,
		green = 1,
		yellow = 2,
		red = 3
	}

	public enum Ability
	{
		file2,
		file4,
		file6,
		row2,
		row4,
		row6,
		changecolor8
	}

	[CreateAssetMenu(fileName="New Level", menuName="KoreKrush/Create Level")]
	[Serializable]
	public class Level:ScriptableObject
	{
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
            Debug.Log(hideFlags);
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
			hideFlags = LocalHelper.globalFlag;
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
			hideFlags = LocalHelper.globalFlag;
		}
	}

	[CreateAssetMenu(fileName="New Motor", menuName="KoreKrush/Create Motor")]
	[Serializable]
	public class Motor: ScriptableObject, ISerializationCallbackReceiver
    {

		public float Multiplier;
		public Type Tile; //TODO: en un futuro un motor podria servir con mas de un tile
		public Ability Power;
		public int PowerFillCount;

        public string TileType;
        public GameObject TileGenerated;

        public void OnAfterDeserialize()
        {
            Tile = Type.GetType(TileType);
        }

        public void OnBeforeSerialize()
        {
            TileType = Tile.GetType().Name;
        }

        public void OnEnable ()
		{
			hideFlags = LocalHelper.globalFlag;
		}
	}

	[Serializable]
	public class PieceList: ISerializationCallbackReceiver, IEnumerable<KeyValuePair<Type, int>>
	{
		[SerializeField]
		private Dictionary<Type, int> d_list;
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
			d_list = new Dictionary<Type, int> ();
		}

		public PieceList (PieceList pl)
		{
			list = new List<PieceElems> ();
			d_list = new Dictionary<Type, int> ();
			foreach (var item in pl) {
				Add (item.Key, item.Value);
			}
		}

		public void Add(Type item, int _Count = 1)
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

		public int this [Type index]
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

		public bool ContainsKey(Type p)
		{
			return d_list.ContainsKey (p);
		}

		public void OnBeforeSerialize ()
		{
			list = new List<PieceElems> ();

			foreach (var item in d_list) {
				list.Add (new PieceElems (item.Key.Name, item.Value));
			}

		}

		public void OnAfterDeserialize ()
		{
			d_list = new Dictionary<Type, int> ();

			foreach (var item in list) {
				d_list.Add (Type.GetType(item.Key), item.Count);
			}
		}

		#region IEnumerable implementation

		public IEnumerator<KeyValuePair<Type, int>> GetEnumerator ()
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
		public string Key;
		public int Count;

		public PieceElems (string k, int c)
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
		public Type tile;

		public PieceReward (Type p, int c)
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
