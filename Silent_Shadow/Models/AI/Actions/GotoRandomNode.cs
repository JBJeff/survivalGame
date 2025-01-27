
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.AI.Navigation;

namespace Silent_Shadow.Models.AI.Actions
{
	/// <summary>
	/// Moves the Agent to a
	/// </summary>
	public class GotoRandomNode : GotoNodeAbstract
	{
		public GotoRandomNode()
		{
			ActionName = "GotoRandomNode";
			ActionType = GActionType.GOTO_ACTION;
			Duration = 0f;
			Cost = 1;
			Effects.Add("hasArrivedatNode", 0);
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			return true;
		}

		private static Node FindRandomNodeWithinRadius(Agent agent, float radius, List<Node> nodes)
		{
			var candidates = new List<Node>();

			foreach (var node in nodes)
			{
				float distance = Vector2.Distance(agent.Position, node.Position);

				if (distance <= radius && distance >= agent.StoppingDistance + 10f && node.NodeType == NodeType.Node)
				{
					candidates.Add(node);
				}
			}

			if (candidates.Count == 0)
			{
				return null;
			}

			Random random = new();
			int randomIndex = random.Next(candidates.Count);
			return candidates[randomIndex];
		}

		public override void ActivateAction(Agent agent)
		{
			targetNode = FindRandomNodeWithinRadius(agent, 400f, nodes);
			if (targetNode == null)
			{
				Debug.WriteLine("No valid target node found.");
			}

			agent.Path = null;
			agent.CurrentWaypointIndex = 0;
			SearchingForPath = false;
		}

		public override bool UpdateAction(Agent agent, float deltaTime)
		{
			if (targetNode == null)
			{
				return false; // Action fails without a target.
			}

			return TraverseToNode(agent, targetNode, deltaTime);
		}

		public override void DeactivateAction(Agent agent)
		{
			agent.Path = null;
			agent.CurrentWaypointIndex = 0;
			targetNode = null;
			SearchingForPath = false;
		}
	}
}
