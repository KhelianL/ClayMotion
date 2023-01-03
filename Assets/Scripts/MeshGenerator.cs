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
    private float PINCH_DISTANCE_HIGH = 0.05f;

    private bool isPinching = false;

    private Vector3[] tmpVertices;
    private Vector3 PinchStart;
    private float MODIFY_DISTANCE = 0.15f;

    void Update()
    {
        SelectOption selectMesh = gameObject.GetComponent<SendButtonMenu>().ChooseMesh;

        if (selectMesh == SelectOption.SPHERE || selectMesh == SelectOption.CUBE || selectMesh == SelectOption.CYLINDER)
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
                switch (selectMesh)
                {
                    case SelectOption.CUBE:
                        tmpGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        break;
                    case SelectOption.SPHERE:
                        tmpGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        break;
                    case SelectOption.CYLINDER:
                        tmpGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        break;
                    default:
                        tmpGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        break;
                }
            }

            // While Pinch
            if (isPinching)
            {
                if (tmpGo != null)
                {
                    tmpGo.transform.position = (leftPinchPos + rightPinchPos) / 2;
                    tmpGo.transform.localScale = new Vector3(size, size, size);
                    tmpGo.transform.forward = leftPinchPos - rightPinchPos;

                }
            }

            // End Pinch
            if (isPinching && leftDiff >= PINCH_DISTANCE_HIGH && rightDiff >= PINCH_DISTANCE_HIGH)
            {
                isPinching = false;
                Destroy(tmpGo);

                GameObject go;
                switch (selectMesh)
                {
                    case SelectOption.CUBE:
                        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        break;
                    case SelectOption.SPHERE:
                        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        break;
                    case SelectOption.CYLINDER:
                        go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        break;
                    default:
                        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        break;
                }

                go.transform.position = (leftPinchPos + rightPinchPos) / 2;
                go.transform.localScale = new Vector3(size, size, size);
                go.transform.forward = leftPinchPos - rightPinchPos;

                go.AddComponent<Rigidbody>();
                go.GetComponent<MeshRenderer>().material = Resources.Load("Materials/ClayMat", typeof(Material)) as Material;
                SendHandsToShader sd = go.AddComponent<SendHandsToShader>();
                sd.handManager = handManager;
                go.AddComponent<InteractionBehaviour>();
                go.GetComponent<Rigidbody>().isKinematic = true;

                listObj.Add(go);
            }
        }
        else if (selectMesh == SelectOption.EXTRUDE)
        {
            GameObject go = gameObject.GetComponent<SendButtonMenu>().ClosetsObject;
            if (go != null)
            {
                if (go.GetComponent<MeshFilter>().mesh != null)
                {
                    Mesh mesh = go.GetComponent<MeshFilter>().mesh;
                    Transform m_transform = go.GetComponent<Transform>();

                    Vector3 rightIndex = handManager.R_index_end.transform.position;
                    Vector3 rightThumb = handManager.R_thumb_end.transform.position;
                    Vector3 rightPinchPos = (rightIndex + rightThumb) / 2;
                    float rightDiff = (rightIndex - rightThumb).magnitude;

                    // Start Pinch
                    if (!isPinching && rightDiff < PINCH_DISTANCE_LOW)
                    {
                        isPinching = true;
                        tmpVertices = mesh.vertices;
                        PinchStart = rightPinchPos;

                        tmpGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Material transparentMat = Resources.Load("Materials/TransparentMat", typeof(Material)) as Material;
                        tmpGo.GetComponent<Renderer>().material = transparentMat;
                    }

                    // While Pinch
                    if (isPinching)
                    {
                        if (tmpGo != null)
                        {
                            tmpGo.transform.position = rightPinchPos;
                            tmpGo.transform.localScale = new Vector3(MODIFY_DISTANCE * 2, MODIFY_DISTANCE * 2, MODIFY_DISTANCE * 2);
                        }
                    }

                    // End Pinch
                    if (isPinching && rightDiff >= PINCH_DISTANCE_HIGH)
                    {
                        isPinching = false;
                        Destroy(tmpGo);

                        for (int i = 0; i < tmpVertices.Length; i++)
                        {
                            Vector3 V = m_transform.TransformPoint(tmpVertices[i]); // Mesh point in world pos
                            if ((PinchStart - V).magnitude < MODIFY_DISTANCE)
                            {
                                float modifyCoef = 1 - ((PinchStart - V).magnitude / MODIFY_DISTANCE);
                                Vector3 targetDirection = rightPinchPos - V;
                                V += targetDirection * modifyCoef;
                                tmpVertices[i] = m_transform.InverseTransformPoint(V);
                            }
                        }
                        mesh.vertices = tmpVertices;

                    }
                }
            }
        }
    }

    public void deleteObj()
    {
        for (int i = 0; i < listObj.Count; i++)
        {
            Destroy(listObj[i]);
        }
    }

    public List<GameObject> ListObj
    {
        get { return listObj; }
    }

    public GameObject TmpGo
    {
        get { return tmpGo; }
    }
}
