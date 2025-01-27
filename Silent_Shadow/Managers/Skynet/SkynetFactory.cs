
namespace Silent_Shadow.Managers.Skynet
{
	public static class SkynetFactory
	{
		public static ISkynet GetInstance()
		{
			return new Skynet();
		}
	}
}
