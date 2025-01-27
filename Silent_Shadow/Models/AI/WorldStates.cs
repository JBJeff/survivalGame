
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Silent_Shadow.States;

namespace Silent_Shadow.Models.AI
{
	public class WorldStates : ICloneable
	{
		public Dictionary<string, int> States { get; set; }
		public Vector2 LastKnownPlayerPosition { get; set; }

		public WorldStates()
		{
			States = [];
		}

		public bool HasState(string key)
		{
			return States.ContainsKey(key);
		}

		public void ModifyState(string key, int value)
		{
			if (States.ContainsKey(key))
			{
				States[key] += value;
				if (States[key] < 0 )
				{
					States.Remove(key);
				}
				else
				{
					States.Add(key, value);
				}
			}
		}

		public void SetState(string key, int value)
		{
			if (States.ContainsKey(key))
			{
				States[key] = value;
			}
			else
			{
				States.Add(key, value);
			}
		}

		public void RemoveState(string key)
		{
			States.Remove(key);
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
