
using System.Collections.Generic;
using System.Diagnostics;
using Silent_Shadow.Models.AI;
using Silent_Shadow.Models.AI.Actions;
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Managers.Skynet
{
	/// <summary>
	/// Node for GOAP planer. Represents an GAction.
	/// </summary>
	///
	/// <param name="parent">Parent node</param>
	/// <param name="cost">Cost of the action</param>
	/// <param name="states">Worldstate</param>
	/// <param name="action">Action i question</param>
	public class GoapNode(GoapNode parent, float cost, Dictionary<string, int> states, GAction action)
	{
		public GoapNode Parent { get; set;} = parent;
		public float Cost { get; set; } = cost;
		public Dictionary<string, int> State { get; set; } = new Dictionary<string, int>(states);
		public GAction Action {get; set; } = action;
	}

	/// <summary>
	/// GOAP Planner
	/// </summary>
	public class Skynet : ISkynet
	{
		public Skynet() { }

		public Queue<GAction> Plan(Agent agent, Dictionary<string, int> goal, WorldStates states)
		{
			List<GAction> usableActions = [];
			foreach (GAction action in agent.Actions)
			{
				if (action.CheckProceduralPreconditions(agent))
				{	
					usableActions.Add(action);
				}
			}

			List<GoapNode> leaves = [];
			GoapNode root = new(null, 0, states.States, null);

			bool success = BuildGraph(root, leaves, usableActions, goal);

			if (!success)
			{
				Debug.WriteLine("Skynet: NO PLAN");
				return [];
			}

			GoapNode cheapest = null;
			foreach (GoapNode leave in leaves)
			{
				if (cheapest == null)
				{
					cheapest = leave;
				}
				else
				{
					if (leave.Cost < cheapest.Cost)
					{
						cheapest = leave;
					}
				}
			}

			List<GAction> result = [];
			GoapNode node = cheapest;
			while( node != null )
			{
				if (node.Action != null)
				{
					result.Insert(0, node.Action);
				}
				node = node.Parent;
			}

			Queue<GAction> queue = new();
			foreach (GAction action in result)
			{
				queue.Enqueue(action);
			}

			return queue;
		}

		private bool BuildGraph(GoapNode parent, List<GoapNode> leaves, List<GAction> actions, Dictionary<string, int> goal)
		{
			bool foundPath = false;

			foreach (GAction action in actions)
			{
				if (action.IsAchievableGiven(parent.State))
				{
					Dictionary<string, int> currentState = new(parent.State);
					foreach(KeyValuePair<string, int> effect in action.Effects)
					{
						if (!currentState.ContainsKey(effect.Key))
						{
							currentState.Add(effect.Key, effect.Value);
						}
					}

					GoapNode node = new(parent, parent.Cost + action.Cost, currentState, action);

					if (GoalAchieved(goal, currentState))
					{
						leaves.Add(node);
						foundPath = true;
					}
					else
					{
						List<GAction> subset = ActionSubset(actions, action);
						bool found = BuildGraph(node, leaves, subset, goal);

						if (found)
						{
							foundPath = true;
						}
					}
				}
			}
			return foundPath;
		}

		private static bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
		{
			foreach(KeyValuePair<string, int> g in goal)
			{
				if (!state.ContainsKey(g.Key))
				{
					return false;
				}
			}
			return true;
		}

		private static List<GAction> ActionSubset(List<GAction> actions, GAction ignoreMe)
		{
			List<GAction> subset = [];

			foreach(GAction action in actions)
			{
				if (!action.Equals(ignoreMe))
				{
					subset.Add(action);
				}
			}
			return subset;
		}
	}
}