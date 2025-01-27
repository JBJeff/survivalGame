
using System;
using System.Collections.Generic;
using Silent_Shadow.Models.AI.States;
using Newtonsoft.Json;
using MonoGame.Extended.Content;
using System.IO;

namespace Silent_Shadow.Managers.DialogueManager
{

	public class DialogueData
	{
		public List<Guard> Guards { get; set; }
	}

	public class Guard
	{
		public string Trigger { get; set; }
		public List<string> Lines { get; set; }
	}

	public class DialogueManager : IDialogueManager
	{
		private readonly Dictionary<string, List<string>> dialogue;

		public DialogueManager()
		{
			string json;

			using (Stream stream = Globals.Content.OpenStream("Strings/barks.json"))
			using (StreamReader reader = new(stream))
			{
				json = reader.ReadToEnd();
			}

			// Deserialize the JSON into DialogueData
			var dialogueData = JsonConvert.DeserializeObject<DialogueData>(json);
			dialogue = [];

			foreach (var guard in dialogueData.Guards)
			{
				dialogue[guard.Trigger] = guard.Lines;
			}
		}

		public string GetBark(AlertState alertState)
		{
			
			Random random = new Random();
			List<string> lines;

			switch (alertState)
			{
				case AlertState.IDLE:
					lines = dialogue["idle"];
					return lines[random.Next(lines.Count)];

				case AlertState.COUTIOUS:
					lines = dialogue["cautious"];
					return lines[random.Next(lines.Count)];

				case AlertState.ALERT:
					lines = dialogue["alert"];
					return lines[random.Next(lines.Count)];

				case AlertState.COMBAT:
					lines = dialogue["combat"];
					return lines[random.Next(lines.Count)];

				default:
					throw new InvalidOperationException($"Alertstate {alertState} does not exist");
			}
		}
	}
}
