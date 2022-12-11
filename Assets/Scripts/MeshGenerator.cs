using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public HandManager handManager;

    private List<GameObject> listObj = new List<GameObject>();

    private GameObject tmpGo;

    private float PINCH_DISTANCE_LOW = 0.03f;
    private float PINCH_DISTANCE_HIGH = 0.04f;

    private bool isPinching = false;

    void Update()
    {
        Vector3 rightIndex = handManager.R_index_end.transform.position;
        Vector3 rightThumb = handManager.R_thumb_end.transform.position;
        Vector3 leftIndex = handManager.L_index_end.transform.position;
        Vector3 leftThumb = handManager.L_thumb_end.transform.position;

        Vector3 rightPinchPos = (rightIndex + rightThumb) / 2;
        Vector3 leftPinchPos = (leftIndex + leftThumb) / 2;

        float rightDiff = (rightIndex - rightThumb).magnitude;
        float leftDiff = (leftIndex - leftThumb).magnitude;

        float size = (leftPinchPos - rightPinchPos).magnitude;

        // Start Pinch
        if (!isPinching && leftDiff < PINCH_DISTANCE_LOW && rightDiff < PINCH_DISTANCE_LOW)
        {
            isPinching = true;
            tmpGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }

        // While Pinch
        if (isPinching)
        {
            tmpGo.transform.position = (leftPinchPos + rightPinchPos) / 2;
            tmpGo.transform.localScale = new Vector3(size, size, size);
            tmpGo.transform.forward = leftPinchPos - rightPinchPos;
        }

        // End Pinch
        if (isPinching && leftDiff >= PINCH_DISTANCE_HIGH && rightDiff >= PINCH_DISTANCE_HIGH)
        {
            isPinching = false;
            Destroy(tmpGo);

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = (leftPinchPos + rightPinchPos) / 2;
            go.transform.localScale = new Vector3(size, size, size);
            go.transform.forward = leftPinchPos - rightPinchPos;

            go.AddComponent<Rigidbody>();
            go.GetComponent<MeshRenderer>().material = Resources.Load("Materials/ClayMat", typeof(Material)) as Material;
            SendHandsToShader sd = go.AddComponent<SendHandsToShader>();
            sd.handManager = handManager;
            go.AddComponent<InteractionBehaviour>();

            listObj.Add(go);
        }
    }

    public void deleteObj()
    {
        for (int i = 0; i < listObj.Count; i++)
        {
            Destroy(listObj[i]);
        }
    }
}
