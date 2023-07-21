using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Seb.Meshing;
using TerrainGeneration;
using UnityEngine.UI;
using static Microsoft.MixedReality.GraphicsTools.ProximityLight;
using Unity.VisualScripting;
using System.Linq;

namespace TerrainGeneration {
	public class PlanetaryMeshGenerator : Generator {
		[Header("Settings")]
		public float stepSize;
		public float errorThreshold;
		public float normalsStepSize;
		public float minHeight;

		[Disabled] public int numSpherePointsMax;
		[Disabled] public int numSpherePointsMin;

		public int minResolution = 250;
		public int maxResolution = 1000;
		public int subdivisions = 1;

		public SimpleMeshData[] meshData;
		public Vector3[] cornerPoints;
		[SerializeField, HideInInspector]
		MeshFilter meshFilter;

		[Header("References")]
		public TerrainGeneration.TerrainHeightProcessor heightProcessor;
		public ComputeShader meshNormalsCompute;
		public ComputeShader vertexCompute;
		public Texture2D displaceMap;
		public Material testMat;
		public TerrainGeneration.TerrainHeightSettings heightSettings;
		public Transform meshHolder;

		[Header("Save/Load Settings")]
		public string meshSaveFileName;
		public string outlinesSaveFileName;
		public TextAsset loadFile;


		//public PolygonProcessor polygonProcessor;
		[Disabled] public int totalVertexCount;


		ComputeBuffer spherePointsBuffer;
		ComputeBuffer gridSpherePointsBuffer;
		ComputeBuffer innerVertices2DBuffer;

		const int compute2DVerticesKernel = 0;
		const int assignVertexHeightsKernel = 1;

		// Generation result
		//List<SimpleMeshData> allCombinedMeshes;

		[Range(0, 1)]
		public float longitude = 0;

		protected override void Start() {
			base.Start();
		}

		public override void StartGenerating() {
			NotifyGenerationStarted();

			//allCombinedMeshes = new List<SimpleMeshData>();

			/*if (meshFilter == null) {
				meshFilter = new MeshFilter();
			}*/

			//meshData = IcoSphere.Generate(maxResolution);
			meshData = CubeSphere.GenerateMeshes(maxResolution, subdivisions);
			cornerPoints = CubeSphere.CreateCornerPoints(maxResolution, subdivisions);

			/*if (meshFilter == null) {
				GameObject meshObj = new GameObject("mesh");
				meshObj.transform.parent = transform;

				//meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
				meshFilter = meshObj.AddComponent<MeshFilter>();
			}*/

			//meshFilter.sharedMesh = MeshHelper.CreateMesh(meshData);

			RenderTexture heightMap = heightProcessor.ProcessHeightMap();

			meshNormalsCompute.SetTexture(0, "HeightMap", heightMap);

			//Vector3[] spherePoints = IcoSphere.Generate(maxResolution).vertices;
			//Vector3[] spherePoints = meshData.vertices;
			List<Vector3> spherePoints = new List<Vector3>();
			foreach (SimpleMeshData data in meshData) {
				spherePoints.AddRange(data.vertices);
			}

			spherePointsBuffer = ComputeHelper.CreateStructuredBuffer<Vector3>(spherePoints.ToArray().Length);
			spherePointsBuffer.SetData(spherePoints.ToArray());

			//gridSpherePointsBuffer = ComputeHelper.CreateStructuredBuffer<Vector3>(IcoSphere.Generate(minResolution).vertices);

			// Set other compute data
			innerVertices2DBuffer = ComputeHelper.CreateAppendBuffer<Vector2>(capacity: spherePointsBuffer.count);
			vertexCompute.SetBuffer(compute2DVerticesKernel, "SpherePoints", spherePointsBuffer);
			vertexCompute.SetInt("numSpherePoints", spherePointsBuffer.count);
			vertexCompute.SetBuffer(compute2DVerticesKernel, "InnerVertices2D", innerVertices2DBuffer);
			vertexCompute.SetTexture(compute2DVerticesKernel, "HeightMap", heightMap);
			vertexCompute.SetTexture(assignVertexHeightsKernel, "HeightMap", heightMap);

			vertexCompute.SetFloat("stepSize", stepSize);
			vertexCompute.SetFloat("errorThreshold", errorThreshold);

			totalVertexCount = 0;
			StartCoroutine(GenerateDetail(/*spherePoints.ToArray()*/));
			//GenerateDetail(spherePoints);

		}

		SimpleMeshData GenerateSector(int meshIndex) {
			
			ComputeHelper.ResetAppendBuffer(innerVertices2DBuffer);
			/*
			// Create bounding box of polygon's sphere points
			Bounds3D sphereBounds = new Bounds3D();
			foreach (Vector3 spherePoint in meshData[meshIndex].vertices) {
				sphereBounds.GrowToInclude(spherePoint);
			}

			// Upload polygon data
			vertexCompute.SetVector("polygonCentre3D", sphereBounds.Centre);
			vertexCompute.SetVector("polygonHalfSize3D", sphereBounds.HalfSize);

			//ComputeBuffer polygonBuffer = ComputeHelper.CreateStructuredBuffer<Vector2>(coordinatePath.NumPoints);
			//polygonBuffer.SetData(coordinatePath.GetPointsAsVector2());
			//vertexCompute.SetBuffer(compute2DVerticesKernel, "Polygon2D", polygonBuffer);
			//vertexCompute.SetInt("numPolygonPoints", polygonBuffer.count);

			// Run compute shader to find points inside polygon to use as vertices, and fetch results
			// Run for detail
			vertexCompute.SetFloat("errorThreshold", errorThreshold);
			vertexCompute.SetBuffer(compute2DVerticesKernel, "SpherePoints", spherePointsBuffer);
			vertexCompute.SetInt("numSpherePoints", spherePointsBuffer.count);
			ComputeHelper.Dispatch(vertexCompute, spherePointsBuffer.count, kernelIndex: compute2DVerticesKernel);

			// Run for base grid
			vertexCompute.SetFloat("errorThreshold", 0);
			vertexCompute.SetBuffer(compute2DVerticesKernel, "SpherePoints", gridSpherePointsBuffer);
			vertexCompute.SetInt("numSpherePoints", gridSpherePointsBuffer.count);
			ComputeHelper.Dispatch(vertexCompute, gridSpherePointsBuffer.count, kernelIndex: compute2DVerticesKernel);
			*/
			//Coordinate[] innerPoints = ComputeHelper.ReadDataFromBuffer<Coordinate>(innerVertices2DBuffer, isAppendBuffer: true);
			/*Coordinate[] innerPoints = new Coordinate[meshData[meshIndex].vertices.Length];
			for (int i = 0; i < innerPoints.Length; i++) {
				innerPoints[i] = GeoMaths.PointToCoordinate(meshData[meshIndex].vertices[i]);
			}

			foreach (Vector3 point in meshData[meshIndex].vertices) {
				Debug.Log(point);
			}
			foreach (Coordinate coord in innerPoints) {
				Debug.Log(coord);
			}
			//Debug.Log(innerPoints.Length + " " + meshData[meshIndex].vertices.Length);

			// ---- Triangulate ----
			//int[] triangles = TerrainGeneration.Triangulator.Triangulate(Path.GetPointsAsVector2(innerPoints), false);
			//Debug.Log("Triangulated");*/

			// Assign heights to vertices:
			// At this stage, vertices are all points on unit sphere.
			// After this they will have magntitude (1 + h) where h is the corresponding value in the heightmap [0, 1]
			
			ComputeBuffer vertexBuffer = ComputeHelper.CreateStructuredBuffer(meshData[meshIndex].vertices/*spherePoints*/);
			vertexCompute.SetBuffer(assignVertexHeightsKernel, "Vertices", vertexBuffer);
			vertexCompute.SetInt("numVertices", vertexBuffer.count);

			// Run the compute shader to assign heights, and then fetch the results
			ComputeHelper.Dispatch(vertexCompute, vertexBuffer.count, kernelIndex: assignVertexHeightsKernel);
			Vector3[] vertices = ComputeHelper.ReadDataFromBuffer<Vector3>(vertexBuffer, isAppendBuffer: false);

			// Modify heights based on world radius and height multiplier
			for (int i = 0; i < vertices.Length; i++) {
				float heightT = vertices[i].magnitude - 1; // vertex magnitude is calculated in compute shader as 1 + heightT
				float height = Mathf.Max(minHeight, heightSettings.heightMultiplier * heightT);
				vertices[i] = vertices[i].normalized * (heightSettings.worldRadius + height);
			}

			// Normals
			meshNormalsCompute.SetBuffer(0, "Vertices", vertexBuffer);
			meshNormalsCompute.SetInt("numVerts", vertexBuffer.count);
			meshNormalsCompute.SetFloat("stepSize", normalsStepSize);
			meshNormalsCompute.SetFloat("worldRadius", heightSettings.worldRadius);
			meshNormalsCompute.SetFloat("heightMultiplier", heightSettings.heightMultiplier);
			ComputeBuffer normalsBuffer = ComputeHelper.CreateStructuredBuffer<Vector3>(vertexBuffer.count);
			meshNormalsCompute.SetBuffer(0, "Normals", normalsBuffer);
			ComputeHelper.Dispatch(meshNormalsCompute, vertexBuffer.count);
			Vector3[] normals = ComputeHelper.ReadDataFromBuffer<Vector3>(normalsBuffer, false);

			//Release
			ComputeHelper.Release(vertexBuffer, normalsBuffer);
			totalVertexCount += vertices.Length;

			SimpleMeshData sectorMeshData = new SimpleMeshData(vertices, meshData[meshIndex].triangles, normals);

			return sectorMeshData;
		}

		IEnumerator GenerateDetail(/*Vector3[] spherePoints*/) {

			for (int meshIndex = 0; meshIndex < meshData.Length; meshIndex++) {  // Cube Sphere Stuff
				SimpleMeshData sectorMesh = GenerateSector(meshIndex);
				meshData[meshIndex] = sectorMesh;
				SpawnMesh(sectorMesh, meshIndex);

				yield return null;
			}

			//polygonProcessor.Init(heightMap, coastline.Read());

			
			//Coordinate[] innerPoints = GeoMaths.PointToCoordinate(spherePoints);


			//ComputeHelper.Release(polygonBuffer);

			// Reproject points to be less distorted for triangulation
			/*var processedPolygon = polygonProcessor.ProcessPolygon(polygon, innerPoints);
			if (!processedPolygon.IsValid) {
				Debug.Log("Skip");
				return;
			}*/

			// ---- Triangulate ----
			//int[] triangles = TerrainGeneration.Triangulator.Triangulate(Path.GetPointsAsVector2(innerPoints), false);
			//Debug.Log("Triangulated");
			/*int[] triangles = meshData.triangles;
			/*List<int> triangles = new List<int>();
			foreach (SimpleMeshData data in meshData) {
				triangles.AddRange(data.triangles);
			}*/

			// Assign heights to vertices:
			// At this stage, vertices are all points on unit sphere.
			// After this they will have magntitude (1 + h) where h is the corresponding value in the heightmap [0, 1]
			/*ComputeBuffer vertexBuffer = ComputeHelper.CreateStructuredBuffer(spherePoints);
			vertexCompute.SetBuffer(assignVertexHeightsKernel, "Vertices", vertexBuffer);
			vertexCompute.SetInt("numVertices", vertexBuffer.count);

			// Run the compute shader to assign heights, and then fetch the results
			ComputeHelper.Dispatch(vertexCompute, vertexBuffer.count, kernelIndex: assignVertexHeightsKernel);
			Vector3[] vertices = ComputeHelper.ReadDataFromBuffer<Vector3>(vertexBuffer, isAppendBuffer: false);


			// Modify heights based on world radius and height multiplier
			for (int i = 0; i < vertices.Length; i++) {
				float heightT = vertices[i].magnitude - 1; // vertex magnitude is calculated in compute shader as 1 + heightT
				float height = heightSettings.heightMultiplier * heightT;
				vertices[i] = vertices[i].normalized * (heightSettings.worldRadius + height);
			}
			Debug.Log("Height set");

			// Normals
			meshNormalsCompute.SetBuffer(0, "Vertices", vertexBuffer);
			meshNormalsCompute.SetInt("numVerts", vertexBuffer.count);
			meshNormalsCompute.SetFloat("stepSize", normalsStepSize);
			meshNormalsCompute.SetFloat("worldRadius", heightSettings.worldRadius);
			meshNormalsCompute.SetFloat("heightMultiplier", heightSettings.heightMultiplier);
			ComputeBuffer normalsBuffer = ComputeHelper.CreateStructuredBuffer<Vector3>(vertexBuffer.count);
			meshNormalsCompute.SetBuffer(0, "Normals", normalsBuffer);
			ComputeHelper.Dispatch(meshNormalsCompute, vertexBuffer.count);
			Vector3[] normals = ComputeHelper.ReadDataFromBuffer<Vector3>(normalsBuffer, false);

			//Release
			ComputeHelper.Release(vertexBuffer, normalsBuffer);
			totalVertexCount = vertices.Length;

			/*int[] tri = triangles.ToArray();
			int verIndex = 0;
			int triIndex = 0;
			int normIndex = 0;
			for (int i = 0; i < meshData.Length; i++) {
				if (i == 0) {
					meshData[0] = new SimpleMeshData(vertices[0..meshData[0].vertices.Length], tri[0..meshData[0].triangles.Length], normals[0..meshData[0].normals.Length]);
				} else {
					meshData[i] = new SimpleMeshData(vertices[verIndex..(verIndex+meshData[i].vertices.Length)], tri[triIndex..(triIndex+meshData[i].triangles.Length)], normals[normIndex..(normIndex+meshData[i].normals.Length)]);
				}
				verIndex += meshData[i].vertices.Length;
				triIndex += meshData[i].triangles.Length;
				normIndex += meshData[i].normals.Length;
				//meshData[i] = new SimpleMeshData(vertices[i * meshData[i-1].vertices.Length..i * meshData[i - 1].vertices.Length + meshData[i].vertices.Length], triangles.ToArray(), normals);
			}*/
			/*meshData = new SimpleMeshData(vertices, triangles, normals);
			

			SpawnMesh(meshData, 0);*/

			Debug.Log($"Generation Complete.");
			Release();
			NotifyGenerationComplete();
			Save();

			//yield return null;
		}

		void SpawnMeshes() {
			//RenderObject mesh = MeshHelper.CreateRendererObject(meshData2.name, meshData2, parent: meshHolder, material: testMat);
			/*for (int i = 0; i < meshData.Length; i++) {
				RenderObject mesh = MeshHelper.CreateRendererObject(meshData[i].name, meshData[i], parent: meshHolder, material: testMat);
				transform.GetChild(0).GetChild(i).AddComponent<MeshCollider>().sharedMesh = mesh.filter.sharedMesh;
			}*/
		}

		void SpawnMesh(SimpleMeshData spawnMesh, int meshIndex) {
			/*for (int i = 0; i < meshData.vertices.Length; i++) {
				float heightT = meshData.vertices[i].magnitude - 1; // vertex magnitude is calculated in compute shader as 1 + heightT
				float height = Mathf.Max(minHeight, heightSettings.heightMultiplier * heightT);
				Debug.Log(height);
				meshData.vertices[i] = meshData.vertices[i].normalized * (heightSettings.worldRadius + height);
			}*/
			//RenderObject mesh = MeshHelper.CreateRendererObject(meshData2.name, meshData2, parent: meshHolder, material: testMat);

			RenderObject mesh = MeshHelper.CreateRendererObject(spawnMesh.name, spawnMesh, parent: meshHolder, material: testMat);
			transform.GetChild(0).GetChild(meshIndex).AddComponent<MeshCollider>().sharedMesh = mesh.filter.sharedMesh;
			mesh.gameObject.SetActive(false);
		}

		Vector3[] ConvertToSpherePoints(Coordinate[] coords) {
			Vector3[] spherePoints = new Vector3[coords.Length];
			for (int i = 0; i < coords.Length; i++) {
				spherePoints[i] = GeoMaths.CoordinateToPoint(coords[i]);
			}
			return spherePoints;
		}

		public override void Save() {
			for (int i = 0; i < meshData.Length; i++) {
				meshData[i].Optimize();
				byte[] bytes = MeshSerializer.MeshToBytes(meshData[i]);
				/*Vector3 total = Vector3.zero;
				foreach (Vector3 vertex in data.vertices) {
					total += vertex;
				}
				Coordinate center = GeoMaths.PointToCoordinate((total / data.vertices.Length).normalized);*/
				//FileHelper.SaveBytesToFile(SavePath, meshSaveFileName + "_" + center.longitude + "_" + center.latitude, bytes, log: true);
				FileHelper.SaveBytesToFile(SavePath, meshSaveFileName + "_" + cornerPoints[i * 4] + "_" + cornerPoints[i * 4 + 1] + "_" + cornerPoints[i * 4 + 2] + "_" + cornerPoints[i * 4 + 3], bytes, log: true);
			}
			//meshData.Optimize();
			Debug.Log("Save Complete");
		}

		public override void Load() {
			//var info = MeshLoader.Load(loadFile, testMat, transform, useStaticBatching: false);
		}


		void OnDestroy() {
			Release();
		}

		void Release() {
			ComputeHelper.Release(spherePointsBuffer, innerVertices2DBuffer, gridSpherePointsBuffer);
		}

		void OnValidate() {
			numSpherePointsMax = Mathf.RoundToInt(6 * Mathf.RoundToInt(Mathf.Pow(subdivisions, 2)) * Mathf.Pow(maxResolution, 2));
			numSpherePointsMin = Mathf.RoundToInt(6 * Mathf.RoundToInt(Mathf.Pow(subdivisions, 2)) * Mathf.Pow(minResolution, 2));
			//numSpherePointsMax = IcoSphere.NumVerticesFromResolution(maxResolution);
			//numSpherePointsMin = IcoSphere.NumVerticesFromResolution(minResolution);
		}
	}

}