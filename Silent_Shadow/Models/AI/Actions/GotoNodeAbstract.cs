
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.AI.Navigation;
using Silent_Shadow.States;

namespace Silent_Shadow.Models.AI.Actions
{
	public abstract class GotoNodeAbstract() : GAction()
	{
		protected List<Node> nodes = GameState.Instance.Level.NodeGraph.Nodes;
		protected Node targetNode;
		protected bool SearchingForPath;

		public override void ActivateAction(Agent agent)
		{
			throw new NotImplementedException();
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			throw new NotImplementedException();
		}

		public override void DeactivateAction(Agent agent)
		{
			throw new NotImplementedException();
		}

		public override bool UpdateAction(Agent agent, float deltaTime)
		{
			throw new NotImplementedException();
		}

		private Node FindStartNode(Agent agent)
		{
			Node startNode = Agent.FindNearestValidNode(agent.Position, nodes);
			if (startNode == null)
			{
				#if DEBUG
				Debug.WriteLine("No valid start node found!");
				#endif

				return null;
			}
			return startNode;
		}

		private List<Node> GetPath(Node origin, Node targetNode)
		{
			List<Node> foundPath = AStar.FindPath(origin, targetNode);

			if (foundPath != null && foundPath.Count > 0)
			{
				return foundPath;
			}

			return [];
		}

		protected bool TraverseToNode(Agent agent, Node targetNode, float deltaTime)
		{
			if (agent.Path == null || agent.Path.Count == 0)
			{
				Node startNode = FindStartNode(agent);

				if (!SearchingForPath)
				{
					SearchingForPath = true;
					agent.Path = GetPath(startNode, targetNode);
				}
				return false;
			}
			else
			{
				if (agent.CurrentWaypointIndex < agent.Path.Count)
				{
					Vector2 targetPosition = agent.Path[agent.CurrentWaypointIndex].Position;
					agent.RemainingDistance = Vector2.Distance(agent.Position, targetPosition);

					if (agent.RemainingDistance <= agent.StoppingDistance)
					{
						agent.CurrentWaypointIndex++;
						if (agent.CurrentWaypointIndex >= agent.Path.Count)
						{
							agent.Path = null;
							SearchingForPath = false;
							return true;
						}
					}
					else
					{
						agent.Goto(targetPosition, deltaTime);
					}
				}
			}

			return false;
		}

	}
}
