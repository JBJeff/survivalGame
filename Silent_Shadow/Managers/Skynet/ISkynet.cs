
using System.Collections.Generic;
using Silent_Shadow.Models.AI;
using Silent_Shadow.Models.AI.Actions;
using Silent_Shadow.Models.AI.Agents;

namespace Silent_Shadow.Managers.Skynet;

public interface ISkynet
{
	public Queue<GAction> Plan(Agent agent, Dictionary<string, int> goal, WorldStates states);
}
