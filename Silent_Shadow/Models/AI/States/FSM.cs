
namespace Silent_Shadow.Models.AI.States
{
	/// <summary>
	/// Finite State Maschine
	/// </summary>
	/// 
	/// <author>Jonas Schwindr</author>
	/// <version>1.0</version>
	public enum FiniteStateMashine
	{
		/// <summary>
		/// Agent is moving to a location.
		/// </summary>
		GOTO,

		/// <summary>
		/// Agent is playing an animation.
		/// </summary>
		///
		/// <remarks>
		/// Doing any action is seen as  "playing an Animation" for sake of simplicity
		/// </remarks> 
		ANIMATE
	}
}
