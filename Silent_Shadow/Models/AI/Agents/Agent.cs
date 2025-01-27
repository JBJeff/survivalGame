
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Silent_Shadow.Managers.Skynet;
using Silent_Shadow.Models.AI.Actions;
using Silent_Shadow.Models.AI.Goals;
using Silent_Shadow.Models.AI.States;
using Silent_Shadow.Models.AI.Navigation;
using Silent_Shadow.Models.Weapons;
using Silent_Shadow.Managers.EntityManager;

namespace Silent_Shadow.Models.AI.Agents
{
	/// <summary>
	/// AI Agent actor
	/// </summary>
	/// 
	/// <remarks>
	/// needs goals and actions to act
	/// needs a Navmesh for Pathfinding (if needed) 
	/// </remarks>
	public abstract class Agent : Entity
	{

		public string Name { get; }
		public Weapon CurrentWeapon { get; set; }

		#region GOAP

		public FiniteStateMashine AIState { get; private set; }
		public WorldStates WorldState { get; set; }
		private int WorldStateChecksum;
		private ISkynet planner;
		protected List<Goal> Goals { get; set; }
		public Goal CurrentGoal { get; set; }
		public List<GAction> Actions { get; set; }
		public GAction CurrentAction { get; set; }
		private Queue<GAction> actionQueue;

		#endregion

		#region Pathfinding

		public List<Node> Path { get; set; }
		public int CurrentWaypointIndex { get; set; }
		public float StoppingDistance { get; set; }
		public float RemainingDistance { get; set; }
		public float TurnSpeed { get; set; }

		#endregion

		#region Detection

		public AlertState AlertState { get; set; }
		protected Vector2[] VisionCone { get; set; }
		protected float _detectionCounter = 0;
		protected float _dementiaCounter = 0;
		protected const float _detectionThreshold = 10f;

		#endregion

		// Standard-Hitbox, kann von Unterklassen angepasst werden
		public override Rectangle Bounds
		{
			get
			{
				if (Sprite == null)
				{
					Debug.WriteLine("Sprite is null. Returning default bounds.");
					return new Rectangle((int)Position.X, (int)Position.Y, 1, 1); // Rückgabe minimaler Bounds
				}

				return new Rectangle(
					(int)(Position.X - (Sprite.Width / 2 * Size)),
					(int)(Position.Y - (Sprite.Height / 2 * Size)),
					(int)(Sprite.Width * Size),
					(int)(Sprite.Height * Size)
				);
			}
		}

		protected Agent(string name, float rotation, float speed, float turnSpeed, float stoppingDistance, List<Goal> goals, List<GAction> actions)
		{
			Debug.Assert(!string.IsNullOrEmpty(name), "Missconfugred Agent: No Name assigned");
			Debug.Assert(rotation >= 0, "Misconfigured Agent: Rotation must be non-negative");
			Debug.Assert(speed >= 0, "Misconfigured Agent: Speed must be non-negative");
			Debug.Assert(turnSpeed >= 0, "Misconfigured Agent: TurnSpeed must be non-negative");
			Debug.Assert(stoppingDistance >= 0, "Misconfigured Agent: StoppingDistance must be non-negative");

			Name = name;
			Rotation = rotation;
			Speed = speed;
			TurnSpeed = turnSpeed;
			StoppingDistance = stoppingDistance;

			AIState = FiniteStateMashine.ANIMATE;
			AlertState = AlertState.IDLE;

			WorldState = new();

			Actions = [];
			Actions.AddRange(actions);
			Goals = [];
			Goals.AddRange(goals);
		}

		#region Movement

		/// <summary>
		/// Finds the nearest node realtive to a given position
		/// </summary>
		/// 
		/// <param name="position">postition</param>
		/// <param name="nodes">Nagigation nodes</param>
		/// 
		/// <returns>Nearest node</returns>
		public static Node FindNearestValidNode(Vector2 position, List<Node> nodes)
		{
			Node nearestNode = null;
			float nearestDistance = float.MaxValue;

			foreach (var node in nodes)
			{
				if (node.Neighbors.Count > 0)
				{
					float distance = Vector2.Distance(position, node.Position);
					if (distance < nearestDistance)
					{
						nearestDistance = distance;
						nearestNode = node;
					}
				}
			}
			return nearestNode;
		}

		/// <summary>
		/// Sets a new path for the AI to follow.
		/// </summary>
		///
		/// <param name="path">List of nodes representing the path.</param>
		public void SetPath(List<Node> path)
		{
			Path = path;
			CurrentWaypointIndex = 0;
		}

		/// <summary>
		/// Move towards a point in the world
		/// </summary>
		/// 
		/// <param name="targetPosition"></param>
		/// <param name="deltaTime">Elapsed game time</param>
		public void Goto(Vector2 targetPosition, float deltaTime)
		{
			Vector2 targetDirection = targetPosition - Position;
			targetDirection.Normalize();

			Vector2 currentDirection = new((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));

			Vector2 newDirection = Vector2.Lerp(currentDirection, targetDirection, TurnSpeed * deltaTime);
			newDirection.Normalize();

			Position += newDirection * Speed * deltaTime;
			Rotation = MathHelpers.ToAngle(newDirection);
		}

		/// <summary>
		/// Move towards a node in the world
		/// </summary>
		/// 
		/// <param name="node">target node</param>
		/// <param name="deltaTime">Elapsed game time</param>
		public void Goto(Node node, float deltaTime)
		{
			Vector2 target = node.Position;
			Goto(target, deltaTime);
		}

		#endregion

		#region GOAP

		/// <summary>
		/// Generates a plan to satify the highest priority goal
		/// </summary>
		private void PlanNextAction()
		{
			// Check if we need to replan due to the current goal's condition.
			if (CurrentGoal != null && CurrentGoal.ReplanRequired(this))
			{
				actionQueue = null;
				CurrentGoal = null;
			}

			// Check if we need to replan due to changed worldstates.
			int currentWordSum = WorldState.States.GetHashCode();
			if (WorldStateChecksum != currentWordSum)
			{
				actionQueue = null;
				CurrentGoal = null;
			}
			else
			{
				WorldStateChecksum = currentWordSum;
			}

			// If there is no plan or no active goal, determine the next highest-priority goal.
			if (planner == null || actionQueue == null)
			{
				planner = SkynetFactory.GetInstance();

				foreach (Goal goal in Goals)
				{
					goal.UpdatePriority(this);
					Debug.WriteLine($"Agent({Name}) - {goal} prio: {goal.Priority}");
				}

				List<Goal> sortedGoals = [.. Goals.OrderByDescending(g => g.Priority)];
				CurrentGoal = sortedGoals.FirstOrDefault();

				if (CurrentGoal != null)
				{
					Debug.WriteLine($"Agent({Name}) - Selected new goal: {CurrentGoal}");
					actionQueue = planner.Plan(this, CurrentGoal.Subgoals, WorldState);

					// Check immediately if the goal is already satisfied (avoid planning unnecessarily).
					if (CurrentGoal.IsSatisfied(this))
					{
						Debug.WriteLine($"Agent({Name}) - Goal {CurrentGoal} is already satisfied.");

						actionQueue = null;
						CurrentGoal = null;
					}
				}
			}

			// Execute the next action if there are actions in the queue.
			if (actionQueue != null && actionQueue.Count > 0)
			{
				CurrentAction = actionQueue.Dequeue();
				if (CurrentAction != null)
				{
					ExecuteAction(CurrentAction);
				}
			}

			// After executing all actions, check if the goal is satisfied.
			if (actionQueue != null && actionQueue.Count == 0)
			{
				Debug.WriteLine($"Agent({Name}) - No actions left");
				if (CurrentGoal != null && CurrentGoal.IsSatisfied(this))
				{
					Debug.WriteLine($"Agent({Name}) - Goal {CurrentGoal} completed successfully.");

					if (CurrentGoal.RemoveAfterCompletion)
					{
						Goals.Remove(CurrentGoal);
					}

					CurrentGoal = null;
				}

				// NOTE: Setting actionQueue to null forces the system to replan if necessary.
				actionQueue = null;
			}
		}

		/// <summary>
		/// Runs an action
		/// </summary>
		///
		/// <param name="action">The action</param>
		private void ExecuteAction(GAction action)
		{
			if (action.CheckProceduralPreconditions(this))
			{
				action.Running = true;
				action.ActivateAction(this);

				switch (action.ActionType)
				{
					case GActionType.ANIMATE_ACTION:
						AIState = FiniteStateMashine.ANIMATE;
						break;

					case GActionType.GOTO_ACTION:
						AIState = FiniteStateMashine.GOTO;
						break;
				}
			}
			else
			{
				actionQueue = null;
			}
		}

		/// <summary>
		/// Stops a running action
		/// </summary>
		private void CompleteAction()
		{
			CurrentAction.Running = false;
			CurrentAction.DeactivateAction(this);
			CurrentAction = null;
		}

		private void HandleAnimateState()
		{
			if (CurrentGoal != null && CurrentGoal.ReplanRequired(this))
			{
				Debug.WriteLine($"Agent({Name}) - ReplanRequired triggered during animation state for goal '{CurrentGoal}'. Abandoning and replanning.");

				CompleteAction();
				CurrentGoal = null;
				actionQueue = null;
				PlanNextAction(); // NOTE: Re-enter planning immediately.

				return;
			}

			if (CurrentAction != null && CurrentAction.Running)
			{
				if (!CurrentAction.UpdateAction(this, Globals.DeltaTime))
				{
					// NOTE: Action is still running.
					return;
				}
				CompleteAction();
			}
			else
			{
				PlanNextAction();
			}
		}

		private void HandleGotoState()
		{
			if (CurrentGoal != null && CurrentGoal.ReplanRequired(this))
			{

				Debug.WriteLine($"Agent({Name}) - ReplanRequired triggered during GOTO state for goal '{CurrentGoal}'. Abandoning and replanning.");

				CompleteAction();
				CurrentGoal = null;
				actionQueue = null;
				PlanNextAction(); // NOTE: Re-enter planning immediately.

				return;
			}

			if (CurrentAction != null && CurrentAction.Running)
			{
				if (!CurrentAction.UpdateAction(this, Globals.DeltaTime))
				{
					return; // NOTE: Action is still running.
				}

				CompleteAction();
				AIState = FiniteStateMashine.ANIMATE;
			}
			else
			{
				AIState = FiniteStateMashine.ANIMATE;
			}
		}

		#endregion

		#region Detection

		public static bool PlayerInVisionCone(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
		{
			return MathHelpers.PointInTriangle(A, B, C, P);
		}

		public static bool CheckForVisualObstacle(Vector2 start, Vector2 end, List<Rectangle> colliders)
		{
			foreach (var collider in colliders)
			{
				Vector2[] corners =
				[
					new Vector2(collider.X, collider.Y), // Top-left
					new Vector2(collider.X + collider.Width, collider.Y), // Top-right
					new Vector2(collider.X + collider.Width, collider.Y + collider.Height), // Bottom-right
					new Vector2(collider.X, collider.Y + collider.Height) // Bottom-left
				];

				for (int i = 0; i < 4; i++)
				{
					Vector2 edgeStart = corners[i];
					Vector2 edgeEnd = corners[(i + 1) % 4];
					if (MathHelpers.LineIntersectsLine(start, end, edgeStart, edgeEnd))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual bool PlayerDetected(float deltaTime)
		{
			// (\(\
			// ( –.–)
			// o_(")(")
			//
			// Not implemeted, overrideable

			return false;
		}

		#endregion

		public override void Update()
		{
			// (\(\
			// ( –.–)
			// o_(")(")
			//
			// Not implemeted, overrideable

			throw new NotImplementedException();
		}

		public override void LateUpdate()
		{
			switch (AIState)
			{
				case FiniteStateMashine.ANIMATE:
					HandleAnimateState();
					break;

				case FiniteStateMashine.GOTO:
					HandleGotoState();
					break;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Sprite, Position, null, Tint, Rotation, SpriteOffset, Size, 0, 0);
		}

#if DEBUG
		public override void DrawDebug(SpriteBatch spriteBatch)
		{
			// Zeichnet die Kollisionsbox in Grün
			spriteBatch.DrawRectangle(Bounds, new Color(0, 255, 0, 100));
		}
#endif

		public override void Die()
		{
			WorldState = null;
			planner = null;
			Goals = null;
			CurrentGoal = null;
			Actions = null;
			CurrentAction = null;
			actionQueue = null;

			base.Die();
		}
	}
}
