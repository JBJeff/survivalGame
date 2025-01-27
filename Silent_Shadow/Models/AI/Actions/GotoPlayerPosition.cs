
using System.Diagnostics;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.AI.Navigation;

namespace Silent_Shadow.Models.AI.Actions
{
	public class GotoPlayerPosition : GotoNodeAbstract
	{
		private Node temporaryNode;
		private Node closestNormalNode;

		public GotoPlayerPosition()
		{
			ActionName = "GotoPlayerPosition";
			ActionType = GActionType.GOTO_ACTION;
			Duration = 0f;
			Cost = 10;
			Effects.Add("TargetInMeleeRange", 0);
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			return true;
		}

		private void UpdateTemporaryNode()
		{
			closestNormalNode = Agent.FindNearestValidNode(Hero.Instance.Position, nodes);

			if (closestNormalNode == null)
			{
				Debug.WriteLine("No valid normal node near the player's position.");
				return;
			}

			if (temporaryNode == null)
			{
				temporaryNode = new Node(Hero.Instance.Position, NodeType.MovingNode);
				nodes.Add(temporaryNode);
			}
			else
			{
				temporaryNode.Position = Hero.Instance.Position;
			}

			if (!closestNormalNode.Neighbors.Contains(temporaryNode))
			{
				closestNormalNode.Neighbors.Add(temporaryNode);
			}

			if (!temporaryNode.Neighbors.Contains(closestNormalNode))
			{
				temporaryNode.Neighbors.Add(closestNormalNode);
			}
		}

		public override void ActivateAction(Agent agent)
		{
			UpdateTemporaryNode();

			if (temporaryNode == null || closestNormalNode == null)
			{
				Debug.WriteLine("No valid temporary or closest normal node found.");
				return;
			}

			agent.Path = null;
			agent.CurrentWaypointIndex = 0;
			SearchingForPath = false;
		}

		public override bool UpdateAction(Agent agent, float deltaTime)
		{
			if (temporaryNode == null || closestNormalNode == null)
			{
				return false; // NOTE: Action fails without valid nodes.
			}

			UpdateTemporaryNode();

			return TraverseToNode(agent, temporaryNode, deltaTime);
		}

		public override void DeactivateAction(Agent agent)
		{
			if (temporaryNode != null && closestNormalNode != null)
			{
				closestNormalNode.Neighbors.Remove(temporaryNode);
			}

			agent.Path = null;
			agent.CurrentWaypointIndex = 0;
			temporaryNode = null;
			closestNormalNode = null;
			SearchingForPath = false;
		}
	}
}
