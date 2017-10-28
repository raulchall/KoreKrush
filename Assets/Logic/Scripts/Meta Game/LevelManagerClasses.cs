using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEditor;

namespace KoreKrush
{
	//TODO: hacer todo esto serializable
	public static class LocalHelper
	{
		public static HideFlags globalFlag = HideFlags.DontUnloadUnusedAsset;
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

	[CreateAssetMenu(fileName="New Level", menuName="KoreKrush Elemens/Create Level")]
	[Serializable]
	public class Level:ScriptableObject
	{
		[SerializeField]
		public List<LevelEvent> EventManager; //TODO: deberian poder ser cualquier tipo de eventos, no solo meteoritos

		public float Distance;
		public PieceList Objectives; //TODO:pensar en una mejor estructura que sirva para esto
		public int Turns;
		public float Turn_time;
		public float StartPosition;
		public string WorldSceneName;

		public void OnEnable ()
		{
			hideFlags = LocalHelper.globalFlag;
		}
	}

	[Serializable]
	public class TileCollection
	{
		public Piece tile;
		public int Count;

		public TileCollection (Piece t, int c)
		{
			tile = t;
			Count = c;
		}
	}

//	[Serializable]
//	public class PieceList
//	{
//		[SerializeField]
//		public Dictionary<Piece, int> list;
//		public int Count;
//
//		public void Add(Piece item, int _Count = 1)
//		{
//			if (list.ContainsKey (item)) 
//			{
//				list [item] += _Count;
//			}
//			else{
//				list [item] = _Count;
//			}
//			Count += _Count;
//		}
//
//		public PieceList ()
//		{
//			list = new Dictionary<Piece, int> ();
//			Count = 0;
//		}
//
//
//		public void Subtract(PieceList loot)
//		{
//			foreach (var item in loot.list) {
//				if(list.ContainsKey(item.Key))
//					list [item.Key] -= loot.list[item.Key];
//				Count -= loot.list [item.Key];
//			}
//			Count = (Count < 0) ? 0 : Count;
//		}
//
//	}

	//TODO: esta es la cosa mas ineficiente del mundo, voy a hacerla mas decente en el futuro pero ahora solo me interesa que funcione
	[Serializable]
	public class PieceList: ISerializationCallbackReceiver, IEnumerable<KeyValuePair<Piece, int>>
	{
		[SerializeField]
		private Dictionary<Piece, int> d_list;
		public List<PieceElems> list;
		public int Count;


		public PieceList ()
		{
			list = new List<PieceElems> ();
			d_list = new Dictionary<Piece, int> ();
			Count = 0;
		}

		public PieceList (PieceList pl)
		{
			list = new List<PieceElems> ();
			d_list = new Dictionary<Piece, int> ();
			new List<Piece> (pl.d_list.Keys).ForEach(x => Add (x, pl.d_list[x]));
			Debug.Log ("Count "+ Count);
//			list.ForEach (x => Count += x.Count);
		}

		public void Add(Piece item, int _Count = 1)
		{
			Debug.Log (d_list);
			if (d_list.ContainsKey (item)) {
				d_list [item] += _Count;
			} else {
				d_list.Add(item, Count);
			}
			Count += _Count;
			//			if (list.Exists (x => x.Key == item)) 
			//			{
			//				list.ForEach( x => {
			//					if(x.Key == item){
			//						x.Count += _Count;
			//					}
			//				});
			//			}
			//			else{
			//				list.Add(new PieceElems(item, _Count));
			//			}
			//			Count += _Count;
		}

		public void Subtract(PieceList loot)
		{
			foreach (var item in loot.d_list) {
				if (d_list.ContainsKey (item.Key)) {
					Count -= Math.Min(loot.d_list [item.Key], d_list[item.Key]);

					var rest = d_list [item.Key] - loot.d_list [item.Key];
					if (rest < 0)
						rest = 0;
					d_list [item.Key] = rest;

					if (d_list [item.Key] == 0)
						d_list.Remove (item.Key);
				}
			}
			Count = (Count < 0) ? 0 : Count;

//			foreach (var item in loot.list) {
//				if (list.Exists (j => j.Key == item.Key) && this [item.Key] > 0) {
//					Debug.Log (item.Key);
//					this [item.Key] -= loot [item.Key];
//					//Count -= Math.Min(loot[item.Key], Count);
//				}
//			}
//			Count = (Count < 0) ? 0 : Count;
		}

		public int this [Piece index]
		{
			get {
				if(d_list.ContainsKey(index))
					return d_list[index];
				return 0;

			//				foreach (var item in list) {
			//					if (item.Key == index)
			//						return item.Count;
			//				}
			}
			set{
				if(value >= 0)
				{
					var tmp = value - d_list [index];
					Count += tmp;
					d_list [index] = value;
				}
				else{
					Count -= d_list [index];
					d_list [index] = 0;
				}
				Count += d_list [index] + value;
				d_list [index] = value;

//				foreach (var item in list) {
//					if (item.Key == index) {
//						if (value >= 0) {
//							var v = value - item.Count;
//							Count += v;
//							item.Count = value;
//						} else {
//							Count -= item.Count;
//							item.Count = 0;
//						}
//					}	
//				}
			}
		}

		public bool ContainsKey(Piece p)
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
			d_list = new Dictionary<Piece, int> ();

			foreach (var item in list) {
				d_list.Add (item.Key, item.Count);
			}
		}

		#region IEnumerable implementation

		public IEnumerator<KeyValuePair<Piece, int>> GetEnumerator ()
		{
			return d_list.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator ()
		{
			throw new NotImplementedException ();
		}

		#endregion



		#endregion
	}

	[Serializable]
	public class PieceElems
	{
		public Piece Key;
		public int Count;

		public PieceElems (Piece k, int c)
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

//	public interface LevelEvent
//	{
//		float PathPosition { get; set; }
//		SpeedObject Obj { get; set; }
//	}

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
  
	[CreateAssetMenu(fileName="New Obstacle", menuName="KoreKrush Elemens/Create Obstacle")]
	public class Obstacle: SpeedObject
	{
		public int GearToBreak; //marcha que es necesario completar para romperlo
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

	[CreateAssetMenu(fileName="New Ship", menuName="KoreKrush Elemens/Create Ship")]
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

	[CreateAssetMenu(fileName="New Motor", menuName="KoreKrush Elemens/Create Motor")]
	[Serializable]
	public class Motor: ScriptableObject
	{
		
		public float Multiplier;
		public Piece Tile; //TODO: en un futuro un motor podria servir con mas de un tile
		public Ability Power;
		public int Power_Fill_Count;

		public void OnEnable ()
		{
			hideFlags = LocalHelper.globalFlag;
		}
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
		public Piece tile;

		public PieceReward (Piece p, int c)
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
