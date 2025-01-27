
namespace Silent_Shadow.Models.AI.States
{
	/// <summary>
	/// AI Alert States
	/// </summary>
	/// 
	/// <author>Jonas Schwindr</author>
	/// <version>1.0</version>
	public enum AlertState
	{
		/// <summary>
		/// Default state
		/// </summary>
		/// 
		/// <remarks>
		/// The AI should never revert to this state.
		/// Use CAUTIOUS instead.
		/// </remarks>
		IDLE,

		/// <summary>
		/// "Calm" State.
		/// AI knows about the players presence, but has given up the search.
		/// </summary>
		COUTIOUS,

		/// <summary>
		/// AI is alerted and actively searching.
		/// </summary>
		ALERT,

		/// <summary>
		/// AI is in actice combat.
		/// </summary>
		COMBAT
	}
}
