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
            // face arri�re
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
            // face arri�re
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

        //SubdivideMesh(ref vertices, ref triangles, ref normals);
        // SubdivideMesh(ref vertices, ref triangles, ref normals);
        // SubdivideMesh(ref vertices, ref triangles, ref normals);

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

    // Subdivise le maillage en utilisant la méthode de Catmull-Clark
    void SubdivideMesh(ref Vector3[] vertices, ref int[] triangles, ref Vector3[] normals)
    {
        int nbStartTri = triangles.Length;

        int vertNextind = vertices.Length;
        int normNextind = normals.Length;
        int triNextInd = triangles.Length;

        System.Array.Resize(ref vertices, vertices.Length + triangles.Length);
        System.Array.Resize(ref normals, vertices.Length);
        System.Array.Resize(ref triangles, triangles.Length*4);

        // Parcours chaque triangle du maillage
        for (int i = 0; i < nbStartTri; i += 3)
        {
            // Récupère les indices des sommets du triangle
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            // Récupère les sommets du triangle
            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            // Calcule les sommets de chaque côté du triangle
            Vector3 sideVertex1 = (v1 + v2) / 2;
            Vector3 sideVertex2 = (v2 + v3) / 2;
            Vector3 sideVertex3 = (v3 + v1) / 2;

            // Récupère l'index du sommet de côté
            int index1 = vertNextind;
            vertices[vertNextind++] = sideVertex1;

            int index2 = vertNextind;
            vertices[vertNextind++] = sideVertex2;

            int index3 = vertNextind;
            vertices[vertNextind++] = sideVertex3;

            // Remplace le triangle original par 4 triangles plus petits
            triangles[i] = index1;
            triangles[i + 1] = index2;
            triangles[i + 2] = index3;

            triangles[triNextInd++] = i1;
            triangles[triNextInd++] = index1;
            triangles[triNextInd++] = index3;

            triangles[triNextInd++] = index2;
            triangles[triNextInd++] = index1;
            triangles[triNextInd++] = i2;

            triangles[triNextInd++] = index3;
            triangles[triNextInd++] = index2;
            triangles[triNextInd++] = i3;

            normals[index1] = (normals[i1] + normals[i2]) / 2;
            normals[index2] = (normals[i2] + normals[i3]) / 2;
            normals[index3] = (normals[i3] + normals[i1]) / 2;

        }
    }
         public GameObject GenerateTore(int longitudeBands = 30, int latitudeBands = 30,  float R = 1.0f, float r = 0.5f, float radius = 0.5f)
    {
    GameObject res = new GameObject();

    res.AddComponent<MeshRenderer>();
    res.AddComponent<MeshCollider>();

    Vector3[] vertices = new Vector3[(longitudeBands + 1) * (latitudeBands + 1)];
    int[] triangles = new int[longitudeBands * latitudeBands * 6];
    Vector3[] normals = new Vector3[vertices.Length];

    int index = 0;
    for (int lat = 0; lat <= latitudeBands; lat++)
    {
        float φ = lat * Mathf.PI / latitudeBands;

        for (int lon = 0; lon <= longitudeBands; lon++)
        {
            float θ = lon * 2 * Mathf.PI / longitudeBands;

            float x = (R + r * Mathf.Cos(φ)) * Mathf.Cos(θ);
            float y = (R + r * Mathf.Cos(φ)) * Mathf.Sin(θ);
            float z = r * Mathf.Sin(φ);

            vertices[index] = new Vector3(x, y, z);

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

    res.GetComponent<Renderer>().material =Resources.Load("Materials/Standard", typeof(Material)) as Material; ;

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

    public GameObject GenerateCylinder(int segments = 16, float height = 1.0f, float radius = 0.5f)
    {
        GameObject res = new GameObject();

        res.AddComponent<MeshRenderer>();
        CapsuleCollider collider = res.AddComponent<CapsuleCollider>();

        Vector3[] vertices = new Vector3[(segments + 1) * 2 + 2];
        int[] triangles = new int[segments * 12];
        Vector3[] normals = new Vector3[vertices.Length];

        int index = 0;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            vertices[index++] = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            vertices[index++] = new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius);
        }

        vertices[index++] = new Vector3(0f, 0f, 0f);
        vertices[index++] = new Vector3(0f, height, 0f);

        // Reposition centre
        for(int i=0; i< vertices.Length; i++)
        {
            vertices[i] -= new Vector3(0, height / 2.0f, 0);
        }

        index = 0;
        for (int i = 0; i < segments; i++)
        {
            int baseIndex = i * 2;

            triangles[index++] = baseIndex;
            triangles[index++] = baseIndex + 1;
            triangles[index++] = baseIndex + 2;

            triangles[index++] = baseIndex + 2;
            triangles[index++] = baseIndex + 1;
            triangles[index++] = baseIndex + 3;

            triangles[index++] = baseIndex;
            triangles[index++] = baseIndex + 2;
            triangles[index++] = vertices.Length - 2;

            triangles[index++] = baseIndex + 1;
            triangles[index++] = vertices.Length - 1;
            triangles[index++] = baseIndex + 3;
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