using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public HandManager handManager;

    // List created obj + add component (physic / shader)
    private GameObject[] listObj;

    private Vector3 tmp_pos;
    private Vector3 tmp_scale;
    private Vector3 tmp_rotate;

    private float PINCH_DISTANCE_LOW  = 0.03f;
    private float PINCH_DISTANCE_HIGH = 0.05f;

    // private float SCALE_PCT = 1.0f;

    private bool isPinching = false;

    void Update()
    {
        Vector3 rightIndex = handManager.R_index_end.transform.position;
        Vector3 leftIndex = handManager.L_index_end.transform.position;

        Vector3 rightThumb = handManager.R_thumb_end.transform.position;
        Vector3 leftThumb = handManager.L_thumb_end.transform.position;

        Vector3 leftPinchPos  = (leftIndex  + leftThumb) / 2;
        Vector3 rightPinchPos = (rightIndex + rightThumb) / 2;

        float rightDiff = (rightIndex-rightThumb).magnitude;
        float leftDiff = (leftIndex-leftThumb).magnitude;

        if (!isPinching && leftDiff < PINCH_DISTANCE_LOW && rightDiff < PINCH_DISTANCE_LOW)
        {
            Debug.Log("START");            
            isPinching = true;
        }

        if(isPinching){
            // Faire une anim du précube
        }
        
        if(isPinching && leftDiff >= PINCH_DISTANCE_HIGH && rightDiff >= PINCH_DISTANCE_HIGH){
            Debug.Log("END");
            isPinching = false;

            float size = (leftPinchPos - rightPinchPos).magnitude;

            tmp_scale = new Vector3(size,size,size);
            tmp_pos = (leftPinchPos + rightPinchPos)/2;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = tmp_pos;
            cube.transform.localScale = tmp_scale;
            cube.transform.forward = leftPinchPos - rightPinchPos;
            Debug.Log("position : " + tmp_pos);
            Debug.Log("localScale : " + tmp_scale);
        }
    }
}
