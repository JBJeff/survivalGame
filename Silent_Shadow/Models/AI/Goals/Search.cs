
using System.Collections.Generic;
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Models.AI.Goals
{
	public class Search : Goal
	{
		public Search() : base(initialPriority: 0, removeAfterCompletion: false) {
			Subgoals = new Dictionary<string, int>
			{
				{ "hasArrivedatNode", 1 }
			};
		}

		public override bool ReplanRequired(Agent agent)
		{
			return false;
		}

		public override void UpdatePriority(Agent agent)
		{
			if (agent.WorldState.HasState("playerLost"))
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
			return false;
		}
	}
}

