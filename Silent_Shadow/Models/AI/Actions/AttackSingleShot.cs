
using System.Diagnostics;
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Models.AI.Actions
{
	/// <summary>
	/// Attack Threat with a gun
	/// </summary>
	public class AttackSingleShot : AttackWithFirearm
	{

		public AttackSingleShot()
		{
			ActionName = "Attack_Single_Shot";
			Cost = 5;
			Duration = 0f;
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			return true;
		}

		public override void ActivateAction(Agent agent)
		{
			playerPosition = agent.WorldState.LastKnownPlayerPosition;
		}

		public override bool UpdateAction(Agent agent, float deltaTime)
		{

			if (AimAtTarget(agent, playerPosition, deltaTime))
			{
				Shoot(agent);
				agent.CurrentWeapon.Update();
			}

			return true;
		}

		public override void DeactivateAction(Agent agent)
		{
			// NOTE: Nothing to reset
			Debug.WriteLine($"Agent({agent.Name}) - deactivating single shot action");
		}
	}
}