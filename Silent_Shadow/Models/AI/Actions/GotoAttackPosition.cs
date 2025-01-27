
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.AI.Navigation;

namespace Silent_Shadow.Models.AI.Actions
{
	public class GotoAttackPosition : GotoNodeAbstract
	{
		public GotoAttackPosition()
		{
			ActionName = "Goto_Attack_Position";
			ActionType = GActionType.GOTO_ACTION;
			Duration = 0f;
			Cost = 1;
			Effects.Add("TargetInRange", 0);
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			return true;
		}

		private static Node FindNode(Agent agent, float radius, Vector2 targetPosition, List<Node> nodes)
		{
			Node closestNode = null;
			float closestDistanceToTarget = float.MaxValue;

			foreach (var node in nodes)
			{
				float distance = Vector2.Distance(agent.Position, node.Position);

				if (distance <= radius && distance >= agent.StoppingDistance && node.NodeType == NodeType.Node)
				{
					float distanceToTarget = Vector2.Distance(node.Position, targetPosition);
					if (distanceToTarget < closestDistanceToTarget)
					{
						closestNode = node;
						closestDistanceToTarget = distanceToTarget;
					}
				}
			}

			return closestNode;
		}

		public override void ActivateAction(Agent agent)
		{
			targetNode = FindNode(agent, 100f, agent.WorldState.LastKnownPlayerPosition, nodes);
			
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
			agent.WorldState.SetState("TargetInRange", 0);

			agent.Path = null;
			agent.CurrentWaypointIndex = 0;
			targetNode = null;
			SearchingForPath = false;
		}
	}
}
