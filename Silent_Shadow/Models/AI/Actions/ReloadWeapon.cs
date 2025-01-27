
using System;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.Weapons;

namespace Silent_Shadow.Models.AI.Actions
{
	/// <summary>
	/// Attack Threat with a gun (abstract)
	/// </summary>
	public class ReloadWeapon : GAction
	{
		public ReloadWeapon()
		{
			ActionName = "RELOAD_WEAPON";
			ActionType = GActionType.ANIMATE_ACTION;
			Duration = 10f;
			Cost = 1;
			//PreConditions.Add("TargetInRange", 0);
			Effects.Add("WeaponLoaded", 0);
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			if (agent.CurrentWeapon == null)
			{
				return false;
			}

			return true;
		}

		public override void ActivateAction(Agent agent)
		{
			//agent.CurrentWeapon.Ammo = agent.CurrentWeapon.MaxAmmo;
			agent.CurrentWeapon.Reload();
			Console.WriteLine("Reload");
		}

		public override bool UpdateAction(Agent agent, float deltaTime)
		{
			agent.CurrentWeapon.Update();

			if (agent.CurrentWeapon.Ammo == agent.CurrentWeapon.MaxAmmo)
			{
				return true;
			}
			
			return false;
		}

		public override void DeactivateAction(Agent agent)
		{
			agent.WorldState.SetState("WeaponLoaded", 0);
		}
	}
}