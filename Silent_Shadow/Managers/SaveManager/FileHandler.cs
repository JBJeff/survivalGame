using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silent_Shadow._Managers.SaveManager
{
	//für das lesen und schreiben der Dateien zuständig
	public static class FileHandler
	{
		public static void WriteToFile(string path, string data)
		{
			File.WriteAllText(path, data);
		}

		public static string ReadFromFile(string path)
		{
			return File.Exists(path) ? File.ReadAllText(path) : null;
		}
	}
}
