
using System.Collections.Generic;
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Models.AI.Goals
{
	public abstract class Goal(int initialPriority, bool removeAfterCompletion)
	{
		/// <summary>
		/// Subgoals, e.g. Worldstates for the goal to reach.
		/// </summary>
		public Dictionary<string, int> Subgoals { get; set; }

		/// <summary>
		/// Current priority of the goal.
		/// </summary>
		public int Priority { get; protected set; } = initialPriority;

		/// <summary>
		/// Whether this goal should be removed after completion.
		/// </summary>
		public bool RemoveAfterCompletion { get; protected set; } = removeAfterCompletion;

		/// <summary>
		/// Updates the goal's priority dynamically based on the current world state.
		/// </summary>
		/// /// <param name="worldState"></param>
		public abstract void UpdatePriority(Agent agent);

		/// <summary>
		/// Checks if the goal should be followed or abandoned.
		/// </summary>
		/// 
		/// <param name="worldState"></param>
		public abstract bool ReplanRequired(Agent agent);

		public virtual bool IsSatisfied(Agent agent)
		{
			foreach (var subgoal in Subgoals)
			{
				if (!agent.WorldState.HasState(subgoal.Key) || agent.WorldState.States[subgoal.Key] != subgoal.Value)
				{
					return false;
				}
			}

			return true;
		}
	
	}
}	
