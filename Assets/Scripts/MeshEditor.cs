using System.Drawing;
using UnityEngine;

public class MeshEditor : MonoBehaviour
{
    public HandManager handManager;
    private Mesh mesh;

    private bool isPinching = false;

    private Vector3[] tmpVertices;
    private Vector3 PinchStart;
    private GameObject tmpGo;

    private float MODIFY_DISTANCE = 0.3f;
    private float PINCH_DISTANCE_LOW = 0.03f;
    private float PINCH_DISTANCE_HIGH = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<MeshFilter>().mesh != null)
        {
            Debug.Log("Yes, I have a mesh!");
            mesh = GetComponent<MeshFilter>().mesh;
        }
        else
        {
            Debug.Log("Mesh missing!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rightIndex = handManager.R_index_end.transform.position;
        Vector3 rightThumb = handManager.R_thumb_end.transform.position;
        Vector3 rightPinchPos = (rightIndex + rightThumb) / 2;
        float rightDiff = (rightIndex - rightThumb).magnitude;

        // Start Pinch
        if (!isPinching && rightDiff < PINCH_DISTANCE_LOW)
        {
            Debug.Log("START");
            isPinching = true;
            tmpVertices = mesh.vertices;
            PinchStart = rightPinchPos;

            tmpGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }

        // While Pinch
        if (isPinching)
        {
            tmpGo.transform.position = rightPinchPos;
            tmpGo.transform.localScale = new Vector3(MODIFY_DISTANCE, MODIFY_DISTANCE, MODIFY_DISTANCE);
        }

        // End Pinch
        if (isPinching && rightDiff >= PINCH_DISTANCE_HIGH)
        {
            Debug.Log("END");
            isPinching = false;
            Destroy(tmpGo);

            float powerDist = (PinchStart - rightPinchPos).magnitude; // Start - End
            for (int i = 0; i < tmpVertices.Length; i++)
            {
                Vector3 V = transform.TransformPoint(tmpVertices[i]); // Mesh point in world pos
                if ((PinchStart - V).magnitude < MODIFY_DISTANCE)
                {
                    float distPoint = (rightPinchPos - V).magnitude;
                    Vector3 targetDirection = (rightPinchPos - V).normalized;
                    V += targetDirection * (distPoint / powerDist);
                    tmpVertices[i] = transform.InverseTransformPoint(V);
                }
            }
            mesh.vertices = tmpVertices;
        }
    }
}
