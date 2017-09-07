using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoreKrush
{
	//TODO: hacer todo esto serializable

	public enum Piece //TODO: las piezas no se deberian definir por el color, podrian ser cualquier cosa
	{
		red,
		blue,
		yellow,
		green
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


	public class Level
	{

		public LevelEventManager EventManager = new LevelEventManager ();
		public float Distance { get; set; }
		public Dictionary<Piece,TileCollection> Objectives{ get; set; } //TODO:pensar en una mejor estructura que sirva para esto y para los motores
		//array de lista de eventos, uno por cada turno
		public int Turns {get;set;}
		public float Turn_time { get; set; }

		public bool Completed {
			get
			{
				foreach (var item in Objectives.Values) {
					if (item.Count > 0)
						return false;
				}
				return true;
			} 
		}


		public void UpdateGoals(List<TileCollection> loot)
		{
			foreach (var item in loot) {
				if(Objectives.ContainsKey(item.tile))
					Objectives [item.tile].Count -= item.Count;
			}
		}
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



	public abstract class LevelEvent
	{
		public void Announce ();
	}

	public abstract class ObstacleEvent: LevelEvent
	{
		public List<Reward> Rewards { get; set; }
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
		public float Speed { get; set; }
		public float PathPosition { get; set; }
		public int GearToBreak { get; set; } //marcha que es necesario completar para romperlo
		public float SpeedDamageWhenBreak { get; set; } // cuando es roto le hace este daño a la velocidad de la nave
		public float SpeedDamagePerSecond {
			get { 
				return this.SpeedDamageWhenBreak / 100 + this.Speed / 20; //SpeedDamageWhenBreak/a + Speed/b + c
			}
			set;
		}
		//TODO:debilidades y fortalezas del meteorito

		public void Announce()
		{
			KoreKrush.Events.Logic.MetheorAnnounce (this);
		}
		 
	}


	public class Ship
	{
		public List<Gear> GearsBox { get; set; }
		List<Motor> Motors { get; set; }
		public float MinSpeed { get; set; }
		public string Prefab_Path { get; set; }

	}

	public class Motor
	{
		public float Multiplier { get; set; }
		public Piece Tile { get; set; } //TODO: en un futuro un motor podria servir con mas de un tile
		public Ability Power {get; set; }
		public int Power_Fill_Count {get; set; }
	}

	public class Gear
	{
		public float base_speed { get; set; }
		public float speed_breaker { get; set; }
		public float multiplier { get; set;} //TODO: multiplicadores personalizados por tile
		public float speed_lost_per_second;

		public Gear ()
		{
			multiplier = 1;
		}
		public Gear (float base_speed, float speed_breaker, float spd, float multiplier = 1)
		{
			this.base_speed = base_speed;
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

	#region PIE

	public class Tuple<T,R>{
		public T obj1;
		public R obj2;

		public Tuple (T o1, R o2)
		{
			obj1 = o1;
			obj2 = o2;
		}
	}

	#endregion
}
