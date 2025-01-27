using Silent_Shadow.Models.AI.Goals;
using System.Collections.Generic;
using System.Diagnostics;
using Silent_Shadow.Models.AI.Agents;
using Silent_Shadow.Models.AI.States;

namespace Silent_Shadow.Models.AI.Goals
{
public class AlertNearby : Goal
{

    public AlertNearby() : base(initialPriority: 0, removeAfterCompletion: true) {
			Subgoals = new Dictionary<string, int>
			{
				{ "TargetInView", 0 }
			};
		}

    public override bool IsSatisfied(Agent agent)
    {      
        if (Globals.Game.PlayerAlive)
			{
				agent.WorldState.RemoveState("FoundPlayer");
				return false;
			}

			Priority = 0;
			return true;
    }

    public override bool ReplanRequired(Agent agent)
    {
        // Kein Replan erforderlich, sobald das Ziel begonnen wurde.
        return false;
    }

    public override void UpdatePriority(Agent agent)
    {
        // Beispiel: Dynamische Priorität basierend auf Umgebungsbedingungen.
        if (agent.WorldState.HasState("FoundPlayer"))
        {
            Priority = 10; // Höchste Priorität, wenn der Spieler entdeckt wurde.
        }
        else
        {

            Priority = 0; // Standardpriorität.
        }
    }
}
}
