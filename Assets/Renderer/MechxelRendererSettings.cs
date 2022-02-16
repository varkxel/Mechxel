using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Mechxel.Renderer
{
	[CreateAssetMenu(menuName = "Mechxel/Renderer Asset", fileName = Name)]
	public class MechxelRendererSettings : RenderPipelineAsset
	{
		private const string Name = "Mechxel Renderer";
		
		public readonly UnityEvent OnUpdated = new UnityEvent();
		
		[SerializeField]
		private DefaultFormat _graphicsFormat = DefaultFormat.HDR;
		public DefaultFormat graphicsFormat
		{
			get => _graphicsFormat;
			set
			{
				_graphicsFormat = value;
				OnUpdated.Invoke();
			}
		}
		
		protected override RenderPipeline CreatePipeline()
		{
			return new MechxelRenderer(this);
		}
	}
}