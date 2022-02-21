using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

using static Mechxel.Noise.Permutation;

namespace Mechxel.Noise
{
	[BurstCompile(
		FloatPrecision.Standard, FloatMode.Fast,
		OptimizeFor = OptimizeFor.Performance
	)]
	public struct Simplex2D : IJobParallelFor
	{
		[ReadOnly]  public double4 bounds;
		[ReadOnly]  public double scale;
		
		[WriteOnly] public NativeArray<double> map;
		[ReadOnly]  public int2 dimensions;
		
		[BurstCompile(FloatPrecision.Standard, FloatMode.Fast)]
		public static int Size
		(
			in double2 origin, in double2 size, double scale,
			out int2 dimensions, out double4 bounds
		)
		{
			bounds = double4(origin - size, origin + size);
			return Size(bounds, scale, out dimensions);
		}
		
		[BurstCompile(FloatPrecision.Standard, FloatMode.Fast)]
		public static int Size(in double4 bounds, double scale, out int2 dimensions)
		{
			double4 scaledBounds = bounds / scale;
			int4 boundsInt = (int4) floor(scaledBounds);
			
			dimensions = boundsInt.zw - boundsInt.xy;
			
			int total = dimensions.x * dimensions.y;
			return total;
		}
		
		[BurstCompile]
		public static void Index2D(in int2 dimensions, int index, out int2 position)
		{
			position = int2
			(
				(index) % dimensions.x,
				(index / dimensions.x) % dimensions.y
			);
		}
		
		public static Simplex2D Construct(in double2 origin, in double2 size, double scale)
		{
			int arraySize = Size(origin, size, scale, out int2 dimensions, out double4 bounds);
			Simplex2D instance = new Simplex2D
			{
				bounds = bounds,
				scale = scale,
				
				dimensions = dimensions,
				map = new NativeArray<double>(arraySize, Allocator.TempJob)
			};
			return instance;
		}
		
		[BurstCompile(
			FloatPrecision.Standard, FloatMode.Fast,
			OptimizeFor = OptimizeFor.Performance
		)]
		public static double Sample(in double2 position)
		{
			double C_x = (3.0 - sqrt(3.0)) / 6.0;
			double C_y = 0.5 * (sqrt(3.0) - 1.0);
			double C_z = -1.0 + 2.0 * C_x;
			const double C_w = 1.0 / 41.0;
			
			double4 C = double4(C_x, C_y, C_z, C_w);
			
			// First corner
			double2 i = floor(position + dot(position, C.yy));
			double2 x0 = position - dot(i, C.xx);
			
			// Other corners
			double2 i1 = (x0.x > x0.y) ? double2(1.0, 0.0) : double2(0.0, 1.0);

			double4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			
			// Permutations
			Mod289(ref i);
			double3 p = i.y + double3(0.0, i1.y, 1.0);
			Permute(ref p);
			p += i.x + double3(0.0, i1.x, 1.0);
			Permute(ref p);

			double3 m = max(0.5 - double3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
			m *= m;
			m *= m;
			
			// Gradients: 41 points uniformly over a line, mapped onto a diamond.
			// The ring size 17 * 17 = 289 is close to a multiple of 41 (41 * 7 = 287)
			
			double3 x = 2.0 * frac(p * C.www) - 1.0;
			double3 h = abs(x) - 0.5;
			double3 ox = floor(x + 0.5);
			double3 a0 = x - ox;
			
			// Normalise gradients implicitly by scaling m
			m *= rsqrt(a0 * a0 + h * h);
			
			// Compute final noise value at P
			double3 g = double3
			(
				// X
				a0.x * x0.x + h.x * x0.y,
				// YZ
				a0.yz * x12.xz + h.yz * x12.yw
			);
			return 130.0 * dot(m, g);
		}
		
		public void Execute(int index)
		{
			Index2D(dimensions, index, out int2 sample);
			double2 position = lerp(bounds.xy, bounds.zw, (double2) sample / (double2) (dimensions - 1));
			map[index] = Sample(position);
		}
	}
}