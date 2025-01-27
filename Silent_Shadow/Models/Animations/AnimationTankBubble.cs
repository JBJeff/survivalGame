
using System.Collections.Generic;

namespace Silent_Shadow.Models.Animations
{
	public class AnimationTankBubble
	{
		private readonly Dictionary<object, Animation> _anims = [];
		private object _lastKey;

		public void AddAnimation(object key, Animation animation)
		{
			_anims.Add(key, animation);
			_lastKey ??= key;
		}
	}
}
