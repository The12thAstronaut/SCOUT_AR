using Seb.Meshing;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class MapLoader : MonoBehaviour {

	public Material mat;
	public bool useStaticBatching;
	public bool loadOnStart;
	public bool useStencil;
	public bool loaded { get; private set; }
	public bool generated { get; private set; }
	public float sizeMult = 0.01f;
	[Range(1, 2000)] public float heightMult = 1.0f;
	public float mapSize = 5.0f;
	public Transform parentTransform;
	public GridDistanceIndicator gridDistanceIndicator;
	public TextMeshProUGUI zoomLevelIndicator;
	public TelemetryManager positionInfo;
	public Vector3 worldPosition { get; set; }

	private Vector3[] mapWindowCorners = new Vector3[4];
	private Vector3 centerPoint = Vector3.zero;
	private Vector3[] meshCenters;
	private int[] keys;
	private int count = 0;
	private float totalVec = 0;
	private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
	private float[] zoomPos;
	private List<float> meshHighestMagnitude = new List<float>();
	private bool isLoading = false;

	[SerializeField] float[] zoomRanges = { 50f, 100f, 200f, 500f, 1000f, 5000f, 10000f, 20000f, 50000f, 100000f, 200000f, 400000f };
	[Range(1, 12)]public int zoomLevel = 1;

	/*public ComputeShader mapCompute;

	ComputeBuffer meshPointsBuffer;
	ComputeBuffer innerVerticesBuffer;
	ComputeBuffer innerVerticesIndexBuffer;
	ComputeBuffer changeValueBuffer;*/

	//const int calculateSubregionKernel = 0;


	[Header("Generator Settings")]
	public float normalsStepSize;
	public float minHeight;

	[Disabled] public int numSpherePointsMax;
	[Disabled] public int numSpherePointsMin;

	public int minResolution = 250;
	public int maxResolution = 1000;
	public int subdivisions = 1;

	public Vector3[] cornerPoints;

	[Header("References")]
	public TerrainGeneration.TerrainHeightProcessor heightProcessor;
	public ComputeShader meshNormalsCompute;
	public ComputeShader vertexCompute;
	public TerrainGeneration.TerrainHeightSettings heightSettings;

	[Disabled] public int totalVertexCount;

	ComputeBuffer spherePointsBuffer;

	const int assignVertexHeightsKernel = 1;

	private int mapLayerMask = 1 << 3;
	private MarkerManager markerManager;

	void Start() {
		zoomPos = new float[zoomRanges.Length];
		loaded = false;
		generated = false;
		Physics.IgnoreLayerCollision(0, 3);

		markerManager = GameObject.Find("MarkerManager").GetComponent<MarkerManager>();

		if (useStencil) {
			mat = new Material(mat);
			mat.SetFloat("_StencilComparison", 4);
		}
		
		if (loadOnStart) {
			Load();
		}
		
	}

	void Update()
    {
        
    }

	private void OnValidate() {
		zoomLevelIndicator.text = "Zoom Scale: " + zoomLevel;

		numSpherePointsMax = Mathf.RoundToInt(6 * Mathf.RoundToInt(Mathf.Pow(subdivisions, 2)) * Mathf.Pow(maxResolution, 2));
		numSpherePointsMin = Mathf.RoundToInt(6 * Mathf.RoundToInt(Mathf.Pow(subdivisions, 2)) * Mathf.Pow(minResolution, 2));
	}

	public async void Load() {
		if (loaded || isLoading) return;

		isLoading = true;

		//await Task.Yield();
		//await Task.Yield();
		//await Task.Yield();
		//await Task.Yield();

		meshCenters = new Vector3[6 * subdivisions * subdivisions];

		LoadData();

		loaded = true;
		isLoading = false;
		UpdateMapRenderer(maxResolution);
	}

	public void Reload() {
		if (loaded == true) {
			foreach (Transform mapTile in parentTransform.GetComponentInChildren<Transform>()) {
				Destroy(mapTile.gameObject);
			}
			loaded = false;
		}
		Load();
	}

	async void UpdateMapRenderer(int resolution) {
		generated = false;
		try { gridDistanceIndicator.UpdateIndicator(); } catch { }

		await Task.Yield();
		await Task.Yield();
		await Task.Yield();
		await Task.Yield();
		await Task.Yield();
		await Task.Yield();
		await Task.Yield();
		await Task.Yield();
		await Task.Yield();
		await Task.Yield();
		await Task.Yield();
		await Task.Yield();

		try { zoomPos[zoomLevel - 1] = zoomPos[zoomLevel - 2]; } catch {  }

		transform.parent.GetComponent<RectTransform>().GetWorldCorners(mapWindowCorners);

		for (int i = 0; i < 4; i++) {
			int x = 0;
			while (!Physics.Raycast(mapWindowCorners[i], transform.parent.forward, 10000f, mapLayerMask)) {
				if (x++ > 3) return;
				SimpleMeshData meshData = new SimpleMeshData("temp");
				await Task.Run(() => meshData = CubeSphere.GenerateMesh(resolution, subdivisions, keys[count++]));
				//meshData = AssignMeshHeights(meshData);
				meshRenderers.Add(CreateMapMesh(meshData));

				await Task.Run(() => meshHighestMagnitude.Add(CalculateHighestVector(meshData)));


				float magnitude = 0f;
				foreach (float mag in meshHighestMagnitude) {
					if (mag > magnitude) magnitude = mag;
				}

				zoomPos[zoomLevel - 1] = magnitude;
				transform.GetChild(0).localPosition = new Vector3(0, 0, zoomPos[zoomLevel - 1] > 1 ? zoomPos[zoomLevel - 1] : 1);

				await Task.Yield();
				await Task.Yield();
				await Task.Yield();
				await Task.Yield();
				transform.parent.GetComponent<RectTransform>().GetWorldCorners(mapWindowCorners);
			}
		}

		FixDistance();

		gridDistanceIndicator.UpdateIndicator();
		//isZooming = false;

		RaycastHit hit;
		Physics.Raycast(transform.parent.transform.position, transform.parent.transform.forward, out hit, 10000f, mapLayerMask);
		worldPosition = transform.parent.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).transform.InverseTransformPoint(hit.point);
		generated = true;
	}

	private void FixDistance() {
		RaycastHit[] hits = new RaycastHit[5];
		Physics.Raycast(mapWindowCorners[0], transform.parent.forward, out hits[0], 10000f, mapLayerMask);
		Physics.Raycast(mapWindowCorners[1], transform.parent.forward, out hits[1], 10000f, mapLayerMask);
		Physics.Raycast(mapWindowCorners[2], transform.parent.forward, out hits[2], 10000f, mapLayerMask);
		Physics.Raycast(mapWindowCorners[3], transform.parent.forward, out hits[3], 10000f, mapLayerMask);
		Physics.Raycast(transform.position, transform.parent.forward, out hits[4], 10000f, mapLayerMask);

		// Raycasts in opposite direction in case the map mesh is in front of the panel
		for (int x = 0; x < hits.Length; x++) {
			if (x < 4 && hits[x].point == Vector3.zero) {
				Physics.Raycast(mapWindowCorners[x], -transform.parent.forward, out hits[x], 10000f, mapLayerMask);
			}
			if (x == 4 && hits[x].point == Vector3.zero) {
				Physics.Raycast(transform.position, -transform.parent.forward, out hits[x], 10000f, mapLayerMask);
			}
		}

		float dist = float.MaxValue;
		int index = 0;
		for (int x = 0; x < hits.Length; x++) {
			if (hits[x].point != Vector3.zero && hits[x].distance < dist) {
				dist = hits[x].distance;
				index = x;
			}
		}

		zoomPos[zoomLevel - 1] = parentTransform.GetChild(1).InverseTransformPoint(hits[index].point).magnitude;
		transform.GetChild(0).localPosition = new Vector3(0, 0, zoomPos[zoomLevel - 1] > 1 ? zoomPos[zoomLevel - 1] : 1);
	}

	private void LoadData() {
		transform.GetChild(0).localPosition = Vector3.zero;
		transform.GetChild(0).localRotation = Quaternion.identity;

		float scale = mapSize * 2000000f / zoomRanges[zoomLevel - 1];
		transform.localScale = new Vector3(scale, scale, scale);
		parentTransform.GetChild(0).localScale = new Vector3(1 / scale, 1 / scale, 1 / scale);

		cornerPoints = CubeSphere.CreateCornerPoints(maxResolution, subdivisions);
		for (int i = 0; i < cornerPoints.Length / 4; i++) {
			meshCenters[i] = CubeSphere.CubePointToSpherePoint((cornerPoints[i * 4] + cornerPoints[i * 4 + 1] + cornerPoints[i * 4 + 2] + cornerPoints[i * 4 + 3]) / 4);
		}

		//RenderTexture heightMap = heightProcessor.ProcessHeightMap();

		//meshNormalsCompute.SetTexture(0, "HeightMap", heightMap);
		//vertexCompute.SetTexture(assignVertexHeightsKernel, "HeightMap", heightMap);

		totalVertexCount = 0;

		UpdatePosition();

		InvokeRepeating("UpdatePosition", 5.0f, 5.0f); // Starts in 5 seconds, repeats every 5 seconds

		transform.parent.GetComponent<RectTransform>().GetWorldCorners(mapWindowCorners);
	}

	void UpdatePosition() {

		transform.GetChild(0).localRotation = Quaternion.Euler(-positionInfo.longitudeLatitude.latitude, 0, 0);
		transform.GetChild(0).GetChild(0).localRotation = Quaternion.Euler(0, positionInfo.longitudeLatitude.longitude, 0);
		centerPoint = GeoMaths.CoordinateToPoint(new Coordinate(Mathf.Deg2Rad * positionInfo.longitudeLatitude.longitude, Mathf.Deg2Rad * positionInfo.longitudeLatitude.latitude));

		float[] distances = new float[cornerPoints.Length / 4];

		for (int i = 0; i < distances.Length; i++) {
			distances[i] = Vector3.Distance(centerPoint, meshCenters[i]);
		}
		float min = float.PositiveInfinity;
		for (int i = 0; i < distances.Length; i++) {
			if (distances[i] < min) {
				min = distances[i];
			}
		}

		keys = new int[distances.Length];
		for (int i = 0; i < keys.Length; i++) {
			keys[i] = i;
		}
		Array.Sort(distances, keys);

		RaycastHit hit;
		Physics.Raycast(transform.parent.transform.position, transform.parent.transform.forward, out hit, 100f, mapLayerMask);
		try { worldPosition = transform.parent.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).transform.InverseTransformPoint(hit.point); } catch { }
	}

	float CalculateHighestVector(SimpleMeshData meshData) {
		float highest = 0f;
		foreach (Vector3 vec in meshData.vertices) {
			if (vec.magnitude > highest) highest = vec.magnitude;
		}
		return highest;
	}

	MeshRenderer CreateMapMesh(SimpleMeshData meshData) {
		MeshRenderer meshRenderer = MeshHelper.CreateRendererObject(meshData.name, meshData, mat, parent: parentTransform, gameObject.layer).renderer;

		if (useStaticBatching) {
			meshRenderer.gameObject.isStatic = true;
		}

		meshRenderer.transform.localScale = new Vector3(sizeMult, sizeMult, sizeMult);
		meshRenderer.transform.localPosition = Vector3.zero;
		meshRenderer.transform.localRotation = Quaternion.identity;

		meshRenderer.gameObject.AddComponent<MeshCollider>().sharedMesh = meshRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;

		return meshRenderer;
	}

	public void ZoomIn() {
		if (zoomLevel == 1 || !generated) return;
		zoomLevel--;
		transform.GetChild(0).localPosition = new Vector3(0, 0, zoomPos[zoomLevel - 1] > 1 ? zoomPos[zoomLevel - 1] : 1);

		UpdateMapSize();

		gridDistanceIndicator.UpdateIndicator();

		//Debug.Log((zoomLevel + 3) / 4);
		RemoveFarMeshes((zoomLevel + 3) / 4);
	}

	public void ZoomOut() {
		if (zoomLevel == zoomRanges.Length || !generated) return;
		zoomLevel++;

		UpdateMapSize();
		zoomLevelIndicator.text = "Zoom Scale: " + zoomLevel;

		if (zoomLevel >= 11) {
			RemoveFarMeshes(-1);
			UpdateMapRenderer(minResolution);
		} else {
			UpdateMapRenderer(maxResolution);
		}
		markerManager.UpdateGroupings();
	}

	private void UpdateMapSize() {
		float scale = mapSize * 2000000f / zoomRanges[zoomLevel - 1];

		transform.localScale = new Vector3(scale, scale, scale);

		UpdateMarkerSize();

		zoomLevelIndicator.text = "Zoom Scale: " + zoomLevel;
	}

	public void UpdateMarkerSize() {
		for (int i = 0; i < parentTransform.GetChild(0).childCount; i++) {
			parentTransform.GetChild(0).GetChild(i).localScale = Vector3.one * (zoomRanges[zoomLevel - 1] / zoomRanges[0]);
		}
	}

	private SimpleMeshData AssignMeshHeights(SimpleMeshData mesh) {

		/*spherePointsBuffer = ComputeHelper.CreateStructuredBuffer<Vector3>(mesh.vertices.Length);
		spherePointsBuffer.SetData(mesh.vertices);

		vertexCompute.SetInt("numSpherePoints", spherePointsBuffer.count);

		// Assign heights to vertices:
		// At this stage, vertices are all points on unit sphere.
		// After this they will have magntitude (1 + h) where h is the corresponding value in the heightmap [0, 1]

		ComputeBuffer vertexBuffer = ComputeHelper.CreateStructuredBuffer(mesh.vertices);
		vertexCompute.SetBuffer(assignVertexHeightsKernel, "Vertices", vertexBuffer);
		vertexCompute.SetInt("numVertices", vertexBuffer.count);*/

		// Run the compute shader to assign heights, and then fetch the results
		//ComputeHelper.Dispatch(vertexCompute, vertexBuffer.count, kernelIndex: assignVertexHeightsKernel);
		Vector3[] vertices = mesh.vertices;//ComputeHelper.ReadDataFromBuffer<Vector3>(vertexBuffer, isAppendBuffer: false);

		// Modify heights based on world radius and height multiplier
		for (int i = 0; i < vertices.Length; i++) {
			float heightT = vertices[i].magnitude - 1; // vertex magnitude is calculated in compute shader as 1 + heightT
			float height = Mathf.Max(minHeight, heightSettings.heightMultiplier * heightT);
			vertices[i] = vertices[i].normalized * (heightSettings.worldRadius + height);
		}

		// Normals
		/*meshNormalsCompute.SetBuffer(0, "Vertices", vertexBuffer);
		meshNormalsCompute.SetInt("numVerts", vertexBuffer.count);
		meshNormalsCompute.SetFloat("stepSize", normalsStepSize);
		meshNormalsCompute.SetFloat("worldRadius", heightSettings.worldRadius);
		meshNormalsCompute.SetFloat("heightMultiplier", heightSettings.heightMultiplier);
		ComputeBuffer normalsBuffer = ComputeHelper.CreateStructuredBuffer<Vector3>(vertexBuffer.count);
		meshNormalsCompute.SetBuffer(0, "Normals", normalsBuffer);
		//ComputeHelper.Dispatch(meshNormalsCompute, vertexBuffer.count);*/
		Vector3[] normals = mesh.normals;//ComputeHelper.ReadDataFromBuffer<Vector3>(normalsBuffer, false);

		//Release
		//ComputeHelper.Release(vertexBuffer, normalsBuffer, spherePointsBuffer);
		totalVertexCount += vertices.Length;

		SimpleMeshData sectorMeshData = new SimpleMeshData(vertices, mesh.triangles, normals);

		return sectorMeshData;
	}

	private void RemoveFarMeshes(int retentionVar) {
		float meshDiameter = 2 * MathF.PI / (6.0f * subdivisions);
		float retentionDistance = zoomRanges[zoomLevel - 1] / positionInfo.moonBaseRadius + meshDiameter * retentionVar;

		for (int i = meshRenderers.Count - 1; i >= 0; i--) {
			if (Vector3.Distance(centerPoint, meshCenters[keys[i]]) > retentionDistance) {
				Destroy(meshRenderers[i].gameObject);
				totalVec -= meshHighestMagnitude[i];
				count--;
			}
		}
		meshRenderers.RemoveRange(count, meshRenderers.Count - count);
		meshHighestMagnitude.RemoveRange(count, meshRenderers.Count - count);
	}

	void OnDestroy() {
		Release();
	}

	void Release() {
		ComputeHelper.Release(spherePointsBuffer);
	}

	/*public SimpleMeshData BytesToRegionMesh(byte[] bytes, Vector3 center, float range) {

		MemoryStream stream = new MemoryStream(bytes);
		using (BinaryReader reader = new BinaryReader(stream)) {
			int numBytes = reader.ReadInt32();
			Debug.Assert(numBytes == bytes.Length, "Wrong number of bytes in mesh data");
			// Read name
			int nameByteCount = reader.ReadInt32();
			byte[] nameBytes = reader.ReadBytes(nameByteCount);
			string name = System.Text.Encoding.Unicode.GetString(nameBytes);

			Vector3[] allVertices = MeshSerializer.ReadVector3Array(reader);
			int[] allTriangles = MeshSerializer.ReadIntArray(reader);
			Vector3[] allNormals = MeshSerializer.ReadVector3Array(reader);
			Vector4[] allTexCoords = MeshSerializer.ReadVector4Array(reader);

			//List<Vector3> vertices = new List<Vector3>();
			//List<int> verticesIndex = new List<int>();
			List<int> triangles = new List<int>();
			//List<Vector3> normals = new List<Vector3>();
			List<Vector3> texCoords = new List<Vector3>();
			Dictionary<int, int> change = new Dictionary<int, int>();

			/*for (int i = 0; i < allVertices.Length; i++) {
				if ((allVertices[i].normalized - center).x * (allVertices[i].normalized - center).x <= range * range && (allVertices[i].normalized - center).y * (allVertices[i].normalized - center).y <= range * range && (allVertices[i].normalized - center).z * (allVertices[i].normalized - center).z <= range * range) {
					vertices.Add(allVertices[i]);
					verticesIndex.Add(i);
					normals.Add(allVertices[i]);
					change.Add(i, vertices.Count - i - 1);
					//texCoords.Add(allTexCoords[i]);
				}
			}

			meshPointsBuffer = ComputeHelper.CreateStructuredBuffer<Vector3>(allVertices.Length);
			meshPointsBuffer.SetData(allVertices);
			//meshPointsBuffer = ComputeHelper.CreateStructuredBuffer<Vector3>(allVertices);
			mapCompute.SetBuffer(calculateSubregionKernel, "MeshPoints", meshPointsBuffer);
			mapCompute.SetInt("numMeshPoints", meshPointsBuffer.count);
			innerVerticesBuffer = ComputeHelper.CreateAppendBuffer<Vector3>(capacity: meshPointsBuffer.count);
			mapCompute.SetBuffer(calculateSubregionKernel, "InnerVertices", innerVerticesBuffer);
			innerVerticesIndexBuffer = ComputeHelper.CreateAppendBuffer<int>(capacity: meshPointsBuffer.count);
			mapCompute.SetBuffer(calculateSubregionKernel, "InnerVerticesIndex", innerVerticesIndexBuffer);
			changeValueBuffer = ComputeHelper.CreateAppendBuffer<int>(capacity: meshPointsBuffer.count);
			mapCompute.SetBuffer(calculateSubregionKernel, "ChangeValue", changeValueBuffer);
			mapCompute.SetFloat("range", range);
			//mapCompute.SetInt("numInnerVertices", 0);
			mapCompute.SetVector("center", center);

			ComputeHelper.ResetAppendBuffer(innerVerticesBuffer);

			Debug.Log(0);
			ComputeBuffer numInnerVertices = ComputeHelper.CreateStructuredBuffer<int>(1);
			mapCompute.SetBuffer(calculateSubregionKernel, "numInnerVertices", numInnerVertices);
			// Run the compute shader to calculate inner area, and then fetch the results
			ComputeHelper.Dispatch(mapCompute, meshPointsBuffer.count, kernelIndex: calculateSubregionKernel);
			Vector3[] vertices = ComputeHelper.ReadDataFromBuffer<Vector3>(innerVerticesBuffer, isAppendBuffer: true);
			int[] verticesIndex = ComputeHelper.ReadDataFromBuffer<int>(innerVerticesIndexBuffer, isAppendBuffer: true);
			int[] changeValue = ComputeHelper.ReadDataFromBuffer<int>(changeValueBuffer, isAppendBuffer: true);

			Debug.Log(1);

			for (int i = 0; i < verticesIndex.Length; i++) {
				change.Add(verticesIndex[i], changeValue[i]);
			}
			Debug.Log(2);

			Debug.Log(changeValue[0]);
			Debug.Log(changeValue[1]);
			Debug.Log(changeValue[changeValue.Length - 1]);
			Debug.Log(3);

			for (int i = 0; i < allTriangles.Length; i += 3) {
				if (verticesIndex.Contains(allTriangles[i]) && verticesIndex.Contains(allTriangles[i + 1]) && verticesIndex.Contains(allTriangles[i + 2])) {
					triangles.Add(allTriangles[i] + change[allTriangles[i]]);
					triangles.Add(allTriangles[i + 1] + change[allTriangles[i + 1]]);
					triangles.Add(allTriangles[i + 2] + change[allTriangles[i + 2]]);
				}
			}

			// Return
			//SimpleMeshData meshData = new SimpleMeshData(vertices, triangles.ToArray(), vertices);
			SimpleMeshData meshData = new SimpleMeshData(allVertices, allTriangles, allNormals);
			meshData.name = name;

			ComputeHelper.Release(meshPointsBuffer, innerVerticesBuffer, innerVerticesIndexBuffer, changeValueBuffer);
			return meshData;
		}
	}*/
}
