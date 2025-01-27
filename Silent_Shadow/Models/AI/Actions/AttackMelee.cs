
using System;
using Microsoft.Xna.Framework;
using Silent_Shadow.Managers.EntityManager;
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Models.AI.Actions
{
	/// <summary>
	/// Attack Threat with a melee attack
	/// </summary>
	public class AttackMelee : Attack
	{
		public AttackMelee()
		{
			ActionName = "Attack_Melee";
			Cost = 10;
			PreConditions.Remove("TargetInRange");
			PreConditions.Add("TargetInMeleeRange", 0);
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			return true;
		}

		public override void ActivateAction(Agent agent)
		{
			IEntityManager entityManager = EntityManagerFactory.GetInstance();

			// Richtung und Dreieckspunkte berechnen
			Vector2 direction = MathHelpers.GetDirectionVector(agent.Rotation, Direction.Left);
			Vector2[] bounds = MathHelpers.GetTriangle(agent.Position, direction, 40f, 1f);

			// Aktivator (dieser Agent) wird an die Methode übergeben
			entityManager.CheckForEntityInTriangle(agent, agent.Position, bounds[0], bounds[1]);
			Console.WriteLine("Falcon Punch");
		}

		public override bool UpdateAction(Agent agent, float deltaTime)
		{
			return true;
		}

		public override void DeactivateAction(Agent agent) 
		{
			// NOTE: Nothing to reset
		}
	}
}