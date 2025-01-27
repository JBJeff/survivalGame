
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.Weapons;

namespace Silent_Shadow.Models.AI.Actions
{
	/// <summary>
	/// Attack Threat with a gun (abstract)
	/// </summary>
	public class AttackWithFirearm : Attack
	{
		protected Vector2 playerPosition;

		public AttackWithFirearm()
		{
			ActionName = "Attack_With_Firearm";
			PreConditions.Add("WeaponLoaded", 0);
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			if (agent.CurrentWeapon != null)
			{
				return true;
			}

			if (agent.CurrentWeapon is not Knife)
			{
				return true;
			}

			if (agent.CurrentWeapon.Ammo > 0)
			{
				return true;
			}

			return false;
		}

		protected static bool AimAtTarget(Agent agent, Vector2 targetPosition, float deltaTime)
		{
			const float threshold = 0.04f;

			Vector2 desiredDirection = targetPosition - agent.Position;
			desiredDirection.Normalize();
			
			Vector2 currentDirection = new((float)Math.Cos(agent.Rotation), (float)Math.Sin(agent.Rotation));

			// Smoothly interpolate toward the target
			Vector2 newDirection = Vector2.Lerp(currentDirection, desiredDirection, agent.TurnSpeed * deltaTime);
			newDirection.Normalize();

			float aimDirection = (float)Math.Atan2(newDirection.Y, newDirection.X);
			agent.Rotation = aimDirection;

			float angleDifference = Math.Abs((float)Math.Atan2(desiredDirection.Y, desiredDirection.X) - aimDirection);

			angleDifference = Math.Min(angleDifference, (float)(2 * Math.PI - angleDifference));

			return angleDifference < threshold;
		}

		protected static void Shoot(Agent agent)
		{
			if (agent.CurrentWeapon.Ammo == 0)
			{
				agent.WorldState.RemoveState("WeaponLoaded");
			}
			else
			{
				agent.CurrentWeapon.Fire(agent);
				Debug.WriteLine($"Agent({agent.Name}) - Ammo {agent.CurrentWeapon.Ammo}");
			}
		}

		public override void ActivateAction(Agent agent)
		{
			// NOTE: Abstract, override this
		}

		public override bool UpdateAction(Agent agent, float deltaTime)
		{
			return true;
		}

		public override void DeactivateAction(Agent agent)
		{
			// NOTE: Nothing to reset
			// NOTE: Abstract, override this
		}
	}
}