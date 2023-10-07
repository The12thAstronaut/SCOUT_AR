using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace TerrainGeneration
{
	public class TerrainHeightProcessor : MonoBehaviour
	{

		[SerializeField] TerrainHeightSettings heightSettings;
		[SerializeField] ComputeShader heightMapCompute;
		[SerializeField] Texture2D[] heightMapTop;
		[SerializeField] Texture2D[] heightMapBottom;
		//[SerializeField] Texture2DArray heightMap;

		public RenderTexture processedHeightMap { get; private set; }

		public RenderTexture ProcessHeightMap()
		{
			const int worldHeightsKernel = 0;

			GraphicsFormat format = GraphicsFormat.R16_UNorm;
			processedHeightMap = ComputeHelper.CreateRenderTexture(heightMapTop[0].width, heightMapTop[0].height, FilterMode.Bilinear, format, "World Heights", useMipMaps: true);
			heightMapCompute.SetTexture(worldHeightsKernel, "RawHeightMapTop1", heightMapTop[0]);
			heightMapCompute.SetTexture(worldHeightsKernel, "RawHeightMapTop2", heightMapTop[1]);
			heightMapCompute.SetTexture(worldHeightsKernel, "RawHeightMapTop3", heightMapTop[2]);
			heightMapCompute.SetTexture(worldHeightsKernel, "RawHeightMapTop4", heightMapTop[3]);
			heightMapCompute.SetTexture(worldHeightsKernel, "RawHeightMapBottom1", heightMapBottom[0]);
			heightMapCompute.SetTexture(worldHeightsKernel, "RawHeightMapBottom2", heightMapBottom[1]);
			heightMapCompute.SetTexture(worldHeightsKernel, "RawHeightMapBottom3", heightMapBottom[2]);
			heightMapCompute.SetTexture(worldHeightsKernel, "RawHeightMapBottom4", heightMapBottom[3]);
			heightMapCompute.SetTexture(worldHeightsKernel, "WorldHeightMap", processedHeightMap);
			heightMapCompute.SetInts("size", heightMapTop[0].width, heightMapTop[0].height);
			ComputeHelper.Dispatch(heightMapCompute, processedHeightMap, kernelIndex: worldHeightsKernel);
			processedHeightMap.GenerateMips();
			return processedHeightMap;
		}

		public void Release()
		{
			ComputeHelper.Release(processedHeightMap);
			//Resources.UnloadAsset(heightMap);
			heightMapCompute = null;
		}

		void OnDestroy()
		{
			Release();
		}
	}
}