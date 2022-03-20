using Unity.Entities;

namespace Mechxel.Generator
{
	public struct VoxelChunk : IComponentData
	{
		public DynamicBuffer<ushort> voxels;
	}
}