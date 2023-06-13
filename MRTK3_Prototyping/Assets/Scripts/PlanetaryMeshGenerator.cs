using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
public class PlanetaryMeshGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*MeshData CreateFace(Vector3 normal, int resolution) {
        Vector3 axisA = new Vector3(normal.y, normal.z, normal.x);
        Vector3 axisB = Vector3.Cross(normal, axisA);
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {

                int vertexIndex = x + y * resolution;
                Vector2 t = new Vector2(x, y) / (resolution - 1f);
                Vector3 point = normal + axisA * (2 * t.x - 1) + axisB * (2 * t.y - 1);
                vertices[vertexIndex] = point;

                if (x != resolution - 1 && y != resolution - 1) {
                    triangles[triIndex + 0] = vertexIndex;
					triangles[triIndex + 1] = vertexIndex + resolution + 1;
					triangles[triIndex + 2] = vertexIndex + resolution;
					triangles[triIndex + 3] = vertexIndex;
					triangles[triIndex + 4] = vertexIndex + 1;
					triangles[triIndex + 5] = vertexIndex + resolution + 1;
                    triIndex += 6;
				}
            }
        }

        return new MeshData(vertices, triangles);
    }

    MeshData[] GenerateFaces(int resolution) {
        MeshData[] allMeshData = new MeshData[6];
        Vector3[] faceNormals = {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        for (int i = 0; i < faceNormals.Length; i++) {
            allMeshData[i] = CreateFace(faceNormals[i], resolution);
        }

        return allMeshData;
    }*/
}
