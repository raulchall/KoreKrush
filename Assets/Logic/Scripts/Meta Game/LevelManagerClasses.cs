﻿using System.Collections;
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

	[CreateAssetMenu(fileName="New Level", menuName="KoreKrush Elemens/Create Level")]
	public class Level:ScriptableObject
	{

		public List<LevelEvent> EventManager = new List<LevelEvent> (); //TODO: deberian poder ser cualquier tipo de eventos, no solo meteoritos
		public float Distance;
		public PieceList Objectives; //TODO:pensar en una mejor estructura que sirva para esto
		public int Turns;
		public float Turn_time;
		public float StartPosition;
		public string WorldSceneName;
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

	[Serializable]
	public class PieceList
	{
		public Dictionary<Piece, int> list;
		public int Count;

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

	[Serializable]
	public abstract class LevelEvent
	{
		public float PathPosition;
		public SpeedObject Obj;
	}

	[Serializable]
	public class RewardEvent: LevelEvent
	{
		public List<PieceReward> Rewards; //TODO: que funcione para todo tipor de rewards
		public float MinRewardTime;
		public float MaxRewardTime;
	}

//	[Serializable]
//	public class LevelEventManager
//	{
//		public SortedList<float,LevelEvent> Events_list;
//
//		public LevelEventManager ()
//		{
//			Events_list = new SortedList<float, LevelEvent> ();
//		}
//
//		public void SubscribeEvent(LevelEvent e, float distance){
//			Events_list.Add (distance,e);
//		}
//
//	}

	public abstract class SpeedObject: ScriptableObject
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


		 
	}

	[CreateAssetMenu(fileName="New Ship", menuName="KoreKrush Elemens/Create Ship")]
	public class Ship: ScriptableObject
	{
		public List<Gear> GearsBox;
		public List<Motor> Motors;
		public float MinSpeed;
		public string Prefab_Path;
		public float WarpDuration;
		public float WarpBreakDamage;
		public float MaxSpeed;

	}

	[CreateAssetMenu(fileName="New Motor", menuName="KoreKrush Elemens/Create Motor")]
	public class Motor: ScriptableObject
	{
		public float Multiplier;
		public Piece Tile; //TODO: en un futuro un motor podria servir con mas de un tile
		public Ability Power;
		public int Power_Fill_Count;
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
	public abstract class Reward
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
