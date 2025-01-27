
using System.Collections.Generic;
using System.Text;

namespace Silent_Shadow
{
	public static class DebugUtils
	{
		public static string DictionaryToString<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
		{
			StringBuilder sb = new();

			_ = sb.Append('{');
			
			foreach (var kvp in dictionary)
			{
				sb.AppendFormat("[{0}: {1}], ", kvp.Key, kvp.Value);
			}

			if (dictionary.Count > 0)
			{
				sb.Length -= 2; // Remove trailing ", "
			}

			_ = sb.Append('}');

			return sb.ToString();
		}
	}
}
