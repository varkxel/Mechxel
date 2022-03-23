using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public abstract class RenderPass
	{
		public readonly string LightingPassTag_Name;
		public readonly ShaderTagId LightingPassTag;
		
		protected RenderPass(string passTag)
		{
			LightingPassTag_Name = passTag;
			LightingPassTag = new ShaderTagId(LightingPassTag_Name);
		}
		
		public abstract void Initialise();
		public abstract void Render(ref Context context);
	}
}