
using System.Collections.Generic;
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Models.AI.Actions 
{
	public enum GActionType
	{
		GOTO_ACTION,
		ANIMATE_ACTION
	}

	/// <summary>
	/// Abstackt repre
	/// </summary>
	/// <param name="agent"></param>
	public abstract class GAction()
	{
		public string ActionName { get; set; } = "Action";
		public GActionType ActionType { get; set; } = GActionType.ANIMATE_ACTION;
		public float Cost { get; set; } = 1.0f;
		public float Duration { get; set; } = 0f;
		public Dictionary<string, int> PreConditions { get; set; } = [];
		public Dictionary<string, int> Effects { get; set; } = [];
		public bool Running { get; set; } = false;

		public bool IsAchievableGiven(Dictionary<string, int> conditons)
		{
			foreach (KeyValuePair<string, int> p in PreConditions)
			{
				if (!conditons.ContainsKey(p.Key))
				{
					return false;
				}
			}
			return true;
		}

		public abstract bool CheckProceduralPreconditions(Agent agent);
		public abstract void ActivateAction(Agent agent);
		public abstract bool UpdateAction(Agent agent, float deltaTime);
		public abstract void DeactivateAction(Agent agent);
	}
}
