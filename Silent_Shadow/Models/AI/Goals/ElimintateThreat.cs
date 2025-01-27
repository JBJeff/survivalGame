
using System.Collections.Generic;
using System.Diagnostics;
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Models.AI.Goals
{
	public class ElimintateThreat : Goal
	{
		public ElimintateThreat() : base(initialPriority: 0, removeAfterCompletion: false) {
			Subgoals = new Dictionary<string, int>
			{
				{ "ThreatEliminated", 0 }
			};
		}

		public override bool ReplanRequired(Agent agent)
		{
			// INFO: Can't kill an enemy if you dont know where the fuk he is.
			if (agent.WorldState.HasState("playerLost"))
			{
				Priority = 0; // reset prio
				return true;
			}

			return false;
		}

		public override void UpdatePriority(Agent agent)
		{
			if (agent.WorldState.HasState("playerDetected"))
			{
				Priority = 10;
			}
			else
			{
				Priority = 0;
			}
		}

		public override bool IsSatisfied(Agent agent)
		{
			if (Globals.Game.PlayerAlive)
			{
				agent.WorldState.RemoveState("ThreatEliminated");
				return false;
			}

			Priority = 0;
			return true;
		}
	}
}

