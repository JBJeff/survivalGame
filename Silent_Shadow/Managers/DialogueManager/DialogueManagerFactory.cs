using System;

namespace Silent_Shadow.Managers.DialogueManager
{
	public static class DialogueManagerFactory
	{
		/// <summary>
		/// Creates an instance of a <see cref="IDialogueManager"/>.
		/// </summary>
		/// 
		/// <returns>An instance of <see cref="IDialogueManager"/>.</returns>
		public static IDialogueManager GetInstance()
		{
			return new DialogueManager();
		}
	}
}


