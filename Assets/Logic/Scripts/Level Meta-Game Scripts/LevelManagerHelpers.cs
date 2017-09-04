using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagerHelpers
{

	public enum Piece //TODO: las piezas no se deberian definir por el color, podrian ser cualquier cosa
	{
		red,
		blue,
		yellow,
		green
	}

	public interface Tile
	{
		string Name { get; set;}
		Piece tile_type { get; set; }

	}

	public interface Ship
	{
		Dictionary<Piece, Motor> Speed_processor { get; set; }
		GearsBox Speed_bars { get; set; }
		float MinSpeed { get; set; }
	}

	public interface Motor
	{
		float Multiplier { get; set; }
		//TODO:habilidad
		//TODO:los motores deben tener el tile que ellos representan
	}

	public interface LevelEvent
	{
		
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


	public class Meteor: LevelEvent
	{
		public float Speed { get; set; }

	}

	public class GearsBox //Cambiar nombre
	{
		public List<Gear> gears;
		public float speed_lost_per_second;


		public GearsBox ()
		{
			gears = new List<Gear> ();
			speed_lost_per_second = 0;
		}

		public GearsBox (List<Gear> gears_list, float speed_lost)
		{
			this.gears = gears_list;
			this.speed_lost_per_second = speed_lost;
		}
	}

	public class Gear
	{
		public float base_speed { get; set; }
		public float speed_breaker { get; set; }
		public float multiplier { get; set;} //TODO: multiplicadores personalizados por tile


		public Gear ()
		{
			multiplier = 1;
		}
		public Gear (float base_speed, float speed_breaker, float multiplier = 1)
		{
			this.base_speed = base_speed;
			this.speed_breaker = speed_breaker;
			this.multiplier = multiplier;
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