using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEditor : MonoBehaviour
{
    public HandManager handManager;
    private Mesh mesh;

    private bool isPinching = false;
    private int pinchedVertex = 0;
    
    private float PINCH_DISTANCE_LOW  = 0.03f;
    private float PINCH_DISTANCE_HIGH = 0.05f;

    // Start is called before the first frame update
    void Start()
    { 
        if( this.gameObject.GetComponent<MeshFilter>().mesh != null )
        {
            Debug.Log("Yes, I have a mesh!");
            mesh = GetComponent<MeshFilter>().mesh;
        }
        else
        {
            Debug.Log("Nope");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rightIndex = handManager.R_index_end.transform.position;
        Vector3 rightThumb = handManager.R_thumb_end.transform.position;

        Vector3 rightPinchPos = (rightIndex + rightThumb) / 2;

        float rightDiff = (rightIndex-rightThumb).magnitude;

        if (!isPinching && rightDiff < PINCH_DISTANCE_LOW)
        {
            Debug.Log("START");            
            isPinching = true;

            float minDist = float.MaxValue;

            for(int i = 0; i < mesh.vertices.Length ; i ++)
            {
                float dist = (rightPinchPos-mesh.vertices[i]).magnitude;
                if(dist < minDist){
                    minDist = dist;
                    pinchedVertex = i;
                }
            }
            Debug.Log("pinchedVertex : " + pinchedVertex);
        }

        if(isPinching){
            // Vector3[] tmp = mesh.vertices;
            // tmp[pinchedVertex] = rightPinchPos;
            // mesh.vertices = tmp;
        }
        
        if(isPinching && rightDiff >= PINCH_DISTANCE_HIGH){
            Debug.Log("END");
            isPinching = false;
        }
    }
}
