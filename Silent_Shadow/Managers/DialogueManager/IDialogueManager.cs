using Silent_Shadow.Models.AI.States;

namespace Silent_Shadow.Managers.DialogueManager
{
	public interface IDialogueManager
	{
		/// <summary>
		/// Gets a random bark line based of the Alertstate
		/// </summary>
		/// 
		/// <param name="alertState">The agents Alertstate</param>
		/// 
		/// <returns>A bark line</returns>
		public string GetBark(AlertState alertState);
	}
}

