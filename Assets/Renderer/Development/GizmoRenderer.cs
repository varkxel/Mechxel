using UnityEditor;
using UnityEngine.Rendering;

namespace Mechxel.Renderer.Development
{
	internal static class GizmoRenderer
	{
		#if !UNITY_EDITOR
		
		internal static void DrawGizmos(ref Context context) {}
		
		#else
		
		internal static void DrawGizmos(ref Context context)
		{
			if(!Handles.ShouldRenderGizmos()) return;
			
			context.SRPContext.DrawGizmos(context.camera, GizmoSubset.PreImageEffects);
			context.SRPContext.DrawGizmos(context.camera, GizmoSubset.PostImageEffects);
			
			context.SRPContext.Submit();
		}
		
		#endif
	}
}