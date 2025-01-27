
using System.Collections.Generic;
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Models.AI.Goals
{
	public class Patrol : Goal
	{
		public Patrol() : base(initialPriority: 5, removeAfterCompletion: false) {
			Subgoals = new Dictionary<string, int>
			{
				{ "hasArrivedatNode", 1 }
			};
		}

		public override bool ReplanRequired(Agent agent)
		{
			// INFO: If player has been spotted, there is so need to patrol anymore.
			if (agent.WorldState.HasState("playerDetected"))
			{
				return true;
			}
			
			return false;
		}

		public override void UpdatePriority(Agent agent)
		{
			// NOTE: Priority does not change
		}

		public override bool IsSatisfied(Agent agent)
		{
			// NOTE: This should never be satisfied, i.e run forever
			return false;
		}
	}
}

