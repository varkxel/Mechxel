using System.Runtime.CompilerServices;

namespace Mechxel.Renderer
{
	public enum DepthFormat
	{
		None = 0,
		Bits16 = 16,
		Bits24 = 24,
		Bits32 = 32
	}
	
	public static class DepthFormat_Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Bits(this DepthFormat format) => (int) format;
	}
}