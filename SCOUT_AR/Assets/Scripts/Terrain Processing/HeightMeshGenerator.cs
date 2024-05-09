using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Seb.Meshing;
using TerrainGeneration;
using Unity.VisualScripting;

namespace TerrainGeneration {
	public class HeightMeshGenerator : Generator {

		public Vector2 size = new Vector2(1, 1);
		public float heightFactor = 1f;
		public float[] heights { private set; get; }

		[HideInInspector]
		public SimpleMeshData meshData;

		[Header("References")]
		public ComputeShader vertexCompute;

		[SerializeField] Texture2D heightMap;
		public Material meshMaterial;
		public Transform meshParent;

		[Disabled] public int totalVertexCount;

		const int assignVertexHeightsKernel = 0;

		protected override void Start() {
			base.Start();
		}

		public override void StartGenerating() {
			NotifyGenerationStarted();

			meshData = GeneratePlane(size, heightMap.width, heightMap.height);

			vertexCompute.SetTexture(assignVertexHeightsKernel, "HeightMap", heightMap);

			totalVertexCount = 0;
			StartCoroutine(GenerateDetail(meshData.vertices));
		}

		SimpleMeshData GeneratePlane(Vector2 size, int width, int height) {
			List<Vector3> vertices = new List<Vector3>();
			float xPerStep = size.x / width;
			float yPerStep = size.y / height;

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++)
					vertices.Add(new Vector3(x * xPerStep, 0, y * yPerStep));
			}

			List<int> triangles = new List<int>();

			for (int y = 0; y < height-1; y++) {
				for (int x = 0; x < width-1; x++) {
					int i = (y * height) + x;
					triangles.Add(i);
					triangles.Add(i+height);
					triangles.Add(i + height + 1);

					triangles.Add(i);
					triangles.Add(i + height + 1);
					triangles.Add(i + 1);
				}
			}

			return new SimpleMeshData(vertices.ToArray(), triangles.ToArray());
		}

		IEnumerator GenerateDetail(Vector3[] planePoints) {

			// Assign heights to vertices:
			ComputeBuffer vertexBuffer = ComputeHelper.CreateStructuredBuffer(planePoints);
			ComputeBuffer heightsBuffer = ComputeHelper.CreateStructuredBuffer<float>(planePoints.Length);
			vertexCompute.SetBuffer(assignVertexHeightsKernel, "Vertices", vertexBuffer);
			vertexCompute.SetBuffer(assignVertexHeightsKernel, "Heights", heightsBuffer);
			vertexCompute.SetInt("numVertices", vertexBuffer.count);
			vertexCompute.SetFloats("size", new float[2] { size.x, size.y });
			vertexCompute.SetFloat("heightFactor", heightFactor);

			// Run the compute shader to assign heights, and then fetch the results
			ComputeHelper.Dispatch(vertexCompute, vertexBuffer.count, kernelIndex: assignVertexHeightsKernel);
			Vector3[] vertices = ComputeHelper.ReadDataFromBuffer<Vector3>(vertexBuffer, isAppendBuffer: false);
			heights = ComputeHelper.ReadDataFromBuffer<float>(heightsBuffer, isAppendBuffer: false);

			ComputeHelper.Release(vertexBuffer);
			totalVertexCount = vertices.Length;

			meshData = new SimpleMeshData(vertices, meshData.triangles);

			SpawnMesh(meshData);

			//Debug.Log($"Generation Complete.");
			NotifyGenerationComplete();

			yield return null;
		}

		void SpawnMesh(SimpleMeshData spawnMesh) {
			RenderObject mesh = MeshHelper.CreateRendererObject(spawnMesh.name, spawnMesh, parent: meshParent, material: meshMaterial);
			mesh.gameObject.transform.localScale = Vector3.one;
			
			Vector3[] vertices = mesh.filter.mesh.vertices;
			Vector2[] uvs = new Vector2[vertices.Length];
			for (int i = 0; i < vertices.Length; i++) {
				uvs[i] = new Vector2(vertices[i].x / size.x, vertices[i].z / size.y);
			}

			mesh.filter.mesh.uv = uvs;
			mesh.filter.mesh.RecalculateTangents();
			mesh.filter.mesh.RecalculateBounds();
			mesh.filter.mesh.RecalculateNormals();

		}

		public override void Save() {
			
		}

		public override void Load() {
			
		}
	}

}