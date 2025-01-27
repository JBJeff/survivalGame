
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Silent_Shadow.Managers.EntityManager;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.AI.Navigation;

namespace Silent_Shadow.Models.AI.Actions
{
	public class GotoCCTV : GotoNodeAbstract
	{
        private Vector2 cctvPosition;
		private Node temporaryNode;
		private Node closestNormalNode;

		public GotoCCTV()
		{
			ActionName = "GotoCCTV";
			ActionType = GActionType.GOTO_ACTION;
			Duration = 0f;
			Cost = 10;
			Effects.Add("TargetInView", 0);
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			return true;
		}

		private void UpdateTemporaryNode()
		{   
            var entities = EntityManager.Instance.Entities;
            foreach (var entitie in entities)
			{
				if (entitie is CctvCam cctv){
					if (cctv.seePlayer) {
						Debug.WriteLine("See Player");
                        cctvPosition = entitie.Position;
                        break;
                    }
                }
            }
            closestNormalNode = Agent.FindNearestValidNode(cctvPosition, nodes);

			if (closestNormalNode == null)
			{
				Debug.WriteLine("No valid normal node near the cctv's position.");
				return;
			}

			if (temporaryNode == null)
			{
				temporaryNode = new Node(cctvPosition, NodeType.MovingNode);
				nodes.Add(temporaryNode);
			}
			else
			{
				temporaryNode.Position = cctvPosition;
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

