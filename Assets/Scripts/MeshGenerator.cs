using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int sizeX = 10;
    public int sizeY = 10;
    public int sizeZ = 10;
    public float spacing = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[sizeX * sizeY * sizeZ];


        for (var i = 0; i < sizeX; i++)
        {
            for (var j = 0; j < sizeY; j++)
            {
                for (var k = 0; k < sizeZ; k++)
                {
                    if((i==0 || i==sizeX-1) || (j==0 || j==sizeY-1) || (k==0 || k == sizeZ - 1))
                    {
                        vertices[i + sizeX * (j + sizeY * k)] = new Vector3(i * spacing, j * spacing, k * spacing);

                    }
                }
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if(vertices == null)
        {
            return;
        }
        for(int i =0 ; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
