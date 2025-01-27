
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Models.AI.Actions
{
	/// <summary>
	/// Abstract represemtation of an attack behavoir
	/// </summary>
	public abstract class Attack : GAction
	{
		protected Attack()
		{
			ActionName = "Attack";
			ActionType = GActionType.ANIMATE_ACTION;
			Duration = 10f;
			Cost = 1;
			PreConditions.Add("TargetInRange", 0);
			Effects.Add("ThreatEliminated", 0);
		}

		public override bool CheckProceduralPreconditions(Agent agent)
		{
			return true;
		}

		public override void ActivateAction(Agent agent) { }

		public override bool UpdateAction(Agent agent, float deltaTime) 
		{
			return true;
		}

		public override void DeactivateAction(Agent agent) {}
	}
}
