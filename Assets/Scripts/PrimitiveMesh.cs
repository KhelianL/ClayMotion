using UnityEngine;

public class PrimitiveMesh : MonoBehaviour
{
    public GameObject GenerateCube(float size = 0.5f)
    {
        GameObject res = new GameObject();

        res.AddComponent<MeshRenderer>();
        res.AddComponent<BoxCollider>();

        Vector3[] vertices = new Vector3[] {
            // face avant
            new Vector3(-size, -size,  size), // sommet 0
            new Vector3( size, -size,  size), // sommet 1
            new Vector3( size,  size,  size), // sommet 2
            new Vector3(-size,  size,  size), // sommet 3
            // face arrière
            new Vector3(-size, -size, -size), // sommet 4
            new Vector3( size, -size, -size), // sommet 5
            new Vector3( size,  size, -size), // sommet 6
            new Vector3(-size,  size, -size)  // sommet 7
        };

        int[] triangles = new int[] {
            // face avant
            0, 1, 2,
            2, 3, 0,
            // face droite
            1, 5, 6,
            6, 2, 1,
            // face arrière
            5, 4, 7,
            7, 6, 5,
            // face gauche
            4, 0, 3,
            3, 7, 4,
            // face haute
            3, 2, 6,
            6, 7, 3,
            // face basse
            1, 0, 4,
            4, 5, 1
        };

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector3 normal = Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1));

            normals[i1] = normal;
            normals[i2] = normal;
            normals[i3] = normal;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        MeshFilter meshFilter = res.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        res.GetComponent<Renderer>().material = Resources.Load("Materials/Standard", typeof(Material)) as Material; ;

        return res;
    }

    public GameObject GenerateSphere(int longitudeBands = 30, int latitudeBands = 30, float radius = 0.5f)
    {
        GameObject res = new GameObject();

        res.AddComponent<MeshRenderer>();
        res.AddComponent<SphereCollider>();

        Vector3[] vertices = new Vector3[(longitudeBands + 1) * (latitudeBands + 1)];
        int[] triangles = new int[longitudeBands * latitudeBands * 6];
        Vector3[] normals = new Vector3[vertices.Length];

        int index = 0;
        for (int lat = 0; lat <= latitudeBands; lat++)
        {
            float theta = lat * Mathf.PI / latitudeBands;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            for (int lon = 0; lon <= longitudeBands; lon++)
            {
                float phi = lon * 2 * Mathf.PI / longitudeBands;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                float x = cosPhi * sinTheta;
                float y = cosTheta;
                float z = sinPhi * sinTheta;

                vertices[index] = new Vector3(x, y, z) * radius;

                index++;
            }
        }

        index = 0;
        for (int lat = 0; lat < latitudeBands; lat++)
        {
            for (int lon = 0; lon < longitudeBands; lon++)
            {
                int first = (lat * (longitudeBands + 1)) + lon;
                int second = first + longitudeBands + 1;

                triangles[index++] = first + 1;
                triangles[index++] = second;
                triangles[index++] = first;

                triangles[index++] = first + 1;
                triangles[index++] = second + 1;
                triangles[index++] = second;
            }
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector3 normal = Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1));

            normals[i1] = normal;
            normals[i2] = normal;
            normals[i3] = normal;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        MeshFilter meshFilter = res.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        res.GetComponent<Renderer>().material = Resources.Load("Materials/Standard", typeof(Material)) as Material; ;

        return res;
    }
}