using Seb.Meshing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Mesh;

public class LodMeshLoader : MonoBehaviour
{
	public enum Mode { Single, Tiled };
	public Mode mode;
	public TextAsset meshFileHighRes;
	public TextAsset meshFileLowRes;

	public Material mat;
	public Material lowResMat;
	public bool useStaticBatching;
	public bool loadOnStart;
	private bool loaded;
	public float sizeMult = 0.01f;

	public SimpleLodSystem lodSystem;
	public Transform parentTransform;

	void Start()
	{
		if (loadOnStart)
		{
			Load();
			loaded = true;
		}
	}

	public void Load() {
		if (loaded == true) return;
		loaded = true;
		MeshRenderer highResRenderers;
		MeshRenderer lowResRenderers;
		switch (mode) {
			case Mode.Tiled:
				CreateRenderers(mat);
				break;
			case Mode.Single:
				highResRenderers = CreateRenderer(meshFileHighRes, mat);
				lowResRenderers = CreateRenderer(meshFileLowRes, lowResMat);

				lodSystem.AddLOD(highResRenderers, lowResRenderers);
				break;
		}

		//Debug.Assert(highResRenderers.Length == lowResRenderers.Length, "Mismatch in number of high and low res meshes");



	}

	public void Reload() {
		if (loaded == true) {
			Destroy(transform.GetChild(0).GetChild(0).GetChild(0));
			loaded = false;
		}
		Load();
	}

	MeshRenderer CreateRenderer(TextAsset loadFile, Material material)
	{
		SimpleMeshData meshData = MeshSerializer.BytesToMesh(loadFile.bytes);
		MeshRenderer meshRenderer = MeshHelper.CreateRendererObject(meshData.name, meshData, material, parent: parentTransform, gameObject.layer).renderer;

		if (useStaticBatching) {
			meshRenderer.gameObject.isStatic = true;
		}

		meshRenderer.gameObject.AddComponent<MeshCollider>().sharedMesh = meshRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
		meshRenderer.transform.localScale = new Vector3(sizeMult, sizeMult, sizeMult);
		meshRenderer.transform.localPosition = Vector3.zero;

		return meshRenderer;
	}

	async void CreateRenderers(Material material) {
		string path = FileHelper.MakePath("Assets", "Data", "Map Terrain");
		string[] files = Directory.GetFiles(path, "*.bytes");

		foreach (string file in files) {
			SimpleMeshData meshData = new SimpleMeshData("temp");
			await Task.Run(() => meshData = MeshSerializer.BytesToMesh(File.ReadAllBytes(file)));
			MeshRenderer meshRenderer = MeshHelper.CreateRendererObject(meshData.name, meshData, material, parent: parentTransform, gameObject.layer).renderer;

			if (useStaticBatching) {
				meshRenderer.gameObject.isStatic = true;
			}

			meshRenderer.gameObject.AddComponent<MeshCollider>().sharedMesh = meshRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
			meshRenderer.transform.localScale = new Vector3(sizeMult, sizeMult, sizeMult);
			meshRenderer.transform.localPosition = Vector3.zero;
		}

		//return meshRenderer;
	}
}
