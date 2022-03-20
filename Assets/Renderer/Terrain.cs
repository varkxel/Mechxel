using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public static class Terrain
	{
		public const string LightingPassTag_Name = "MechxelTerrain";
		public static readonly ShaderTagId LightingPassTag = new ShaderTagId(LightingPassTag_Name);

		public const string GBuffer0_Name = "GBuffer0";
	}
}