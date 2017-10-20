using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KoreKrush
{
	//TODO: hacer todo esto serializable

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

	[CreateAssetMenu(fileName="/Utilities/Create/New Level")]
	public class Level:ScriptableObject
	{

		public List<MeteorAppear> EventManager = new List<MeteorAppear> (); //TODO: deberian poder ser cualquier tipo de eventos, no solo meteoritos
		public float Distance { get; set; }
		public PieceList Objectives{ get; set; } //TODO:pensar en una mejor estructura que sirva para esto
		public int Turns {get;set;}
		public float Turn_time { get; set; }
		public float StartPosition { get; set; }
		public string WorldSceneName { get; set; }
	}

	public abstract class Reward
	{
		public int Count { get; set; }

	}

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

	public class PieceList
	{
		public Dictionary<Piece, int> list { get; set; }
		public int Count { get; set; }

		public void Add(Piece item, int _Count = 1)
		{
			if (list.ContainsKey (item)) 
			{
				list [item] += _Count;
			}
			else{
				list [item] = _Count;
			}
			Count += _Count;
		}

		public PieceList ()
		{
			list = new Dictionary<Piece, int> ();
			Count = 0;
		}


		public void Subtract(PieceList loot)
		{
			foreach (var item in loot.list) {
				if(list.ContainsKey(item.Key))
					list [item.Key] -= loot.list[item.Key];
				Count -= loot.list [item.Key];
			}
			Count = (Count < 0) ? 0 : Count;
		}

	}


	public abstract class LevelEvent
	{
		public abstract void Announce ();
		public float PathPosition;
	}

	public abstract class ObstacleEvent: LevelEvent
	{
		public List<PieceReward> Rewards { get; set; } //TODO: que funcione para todo tipor de rewards
		public float MinRewardTime { get; set; }
		public float MaxRewardTime { get; set; }

	}

	public class LevelEventManager
	{
		public SortedList<float,LevelEvent> Events_list{ get; set; }

		public LevelEventManager ()
		{
			Events_list = new SortedList<float, LevelEvent> ();
		}

		public void SubscribeEvent(LevelEvent e, float distance){
			Events_list.Add (distance,e);
		}

	}
  
	public class MeteorAppear: ObstacleEvent
	{
		public GameObject prefab;
		public float Speed;
		public int GearToBreak; //marcha que es necesario completar para romperlo
		public float SpeedDamageWhenBreak; // cuando es roto le hace este daño a la velocidad de la nave
		public float SpeedDamagePerTimeUnit {
			get { 
				return this.SpeedDamageWhenBreak / 10 + this.Speed / 20; //SpeedDamageWhenBreak/a + Speed/b + c
			}
		}
		public float SpeedDamageTimeUnit = 1;
		//TODO:debilidades y fortalezas del meteorito

		public override void Announce()
		{
			//KoreKrush.Events.Logic.MetheorAnnounce (this);
		}
		 
	}

	[CreateAssetMenu(fileName="/Utilities/Create/New Ship")]
	public class Ship: ScriptableObject
	{
		public List<Gear> GearsBox { get; set; }
		public List<Motor> Motors { get; set; }
		public float MinSpeed { get; set; }
		public string Prefab_Path { get; set; }
		public float WarpDuration { get; set; }
		public float WarpBreakDamage { get; set; }
		public float MaxSpeed;

	}

	[CreateAssetMenu(fileName="/Utilities/Create/New Motor")]
	public class Motor: ScriptableObject
	{
		public float Multiplier { get; set; }
		public Piece Tile { get; set; } //TODO: en un futuro un motor podria servir con mas de un tile
		public Ability Power {get; set; }
		public int Power_Fill_Count {get; set; }
	}

	public class Gear
	{
		public float additional_base_speed { get; set; }
		public float speed_breaker { get; set; }
		public float multiplier { get; set;} //TODO: multiplicadores personalizados por tile
		public float speed_lost_per_second;

		public Gear ()
		{
			multiplier = 1;
		}
		public Gear (float base_speed, float speed_breaker, float spd, float multiplier = 1)
		{
			this.additional_base_speed = base_speed;
			this.speed_breaker = speed_breaker;
			this.multiplier = multiplier;
			this.speed_lost_per_second = spd;
		}

	}

	public class PieceReward: Reward
	{
		public Piece tile { get; set; }

		public PieceReward (Piece p, int c)
		{
			tile = p;
			Count = c;
		}
	}

	public class TurnReward: Reward
	{
		public int turns { get; set; }

		public TurnReward (int t, int c)
		{
			turns = t;
			Count = c;
		}
	}
}
