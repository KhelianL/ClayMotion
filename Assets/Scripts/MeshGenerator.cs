using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using System.Net;
using System;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public HandManager handManager;

    private List<GameObject> listObj = new List<GameObject>();

    private GameObject tmpGo;

    private float PINCH_DISTANCE_LOW = 0.02f;
    private float PINCH_DISTANCE_HIGH = 0.03f;

    private bool isPinching = false;

    private Vector3[] tmpVertices;
    private Vector3[] startVertices;
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
                        tmpGo = gameObject.GetComponent<PrimitiveMesh>().GenerateCube();
                        break;
                    case SelectOption.SPHERE:
                        tmpGo = gameObject.GetComponent<PrimitiveMesh>().GenerateSphere();
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
                        go = gameObject.GetComponent<PrimitiveMesh>().GenerateCube();
                        break;
                    case SelectOption.SPHERE:
                        go = gameObject.GetComponent<PrimitiveMesh>().GenerateSphere();
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
                    Vector3 rightIndex = handManager.R_index_end.transform.position;
                    Vector3 rightThumb = handManager.R_thumb_end.transform.position;
                    Vector3 rightPinchPos = (rightIndex + rightThumb) / 2;
                    float rightDiff = (rightIndex - rightThumb).magnitude;

                    // Start Pinch
                    if (!isPinching && rightDiff < PINCH_DISTANCE_LOW)
                    {
                        isPinching = true;

                        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
                        startVertices = mesh.vertices;
                        System.Array.Resize(ref tmpVertices, mesh.vertices.Length);


                        PinchStart = rightPinchPos;

                        tmpGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        tmpGo.transform.localScale = new Vector3(MODIFY_DISTANCE * 2, MODIFY_DISTANCE * 2, MODIFY_DISTANCE * 2);
                        Material transparentMat = Resources.Load("Materials/TransparentMat", typeof(Material)) as Material;
                        tmpGo.GetComponent<Renderer>().material = transparentMat;
                        Destroy(tmpGo.GetComponent<SphereCollider>());
                    }

                    // While Pinch
                    if (isPinching)
                    {
                        if (tmpGo != null)
                        {
                            tmpGo.transform.position = rightPinchPos;

                            Transform m_transform = go.GetComponent<Transform>();

                            for (int i = 0; i < tmpVertices.Length; i++)
                            {
                                Vector3 V = m_transform.TransformPoint(startVertices[i]); // Mesh point in world pos

                                if ((PinchStart - V).magnitude < MODIFY_DISTANCE)
                                {
                                    float modifyCoef = 1 - ((PinchStart - V).magnitude / MODIFY_DISTANCE);
                                    Vector3 targetDirection = rightPinchPos - V;
                                    V += targetDirection * modifyCoef;
                                }
                                tmpVertices[i] = m_transform.InverseTransformPoint(V);
                            }
                            go.GetComponent<MeshFilter>().mesh.vertices = tmpVertices;
                        }
                    }

                    // End Pinch
                    if (isPinching && rightDiff >= PINCH_DISTANCE_HIGH)
                    {
                        isPinching = false;
                        Destroy(tmpGo);
                    }
                }
            }
        }
        else if (selectMesh == SelectOption.SMOOTH)
        {
            GameObject go = gameObject.GetComponent<SendButtonMenu>().ClosetsObject;
            if (go != null)
            {
                if (go.GetComponent<MeshFilter>().mesh != null)
                {
                    if (gameObject.GetComponent<SendButtonMenu>().PassSmooth)
                    {
                        gameObject.GetComponent<SendButtonMenu>().PassSmooth = false;
                        tmpGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        tmpGo.transform.localScale = new Vector3(MODIFY_DISTANCE * 2, MODIFY_DISTANCE * 2, MODIFY_DISTANCE * 2);
                        tmpGo.GetComponent<Renderer>().material = Resources.Load("Materials/TransparentMat", typeof(Material)) as Material;
                        Destroy(tmpGo.GetComponent<SphereCollider>());
                    }
                    Vector3 rightIndex = handManager.R_index_end.transform.position;
                    Vector3 rightThumb = handManager.R_thumb_end.transform.position;
                    Vector3 rightPinchPos = (rightIndex + rightThumb) / 2;
                    float rightDiff = (rightIndex - rightThumb).magnitude;

                    tmpGo.transform.position = rightPinchPos;

                    // Start Pinch
                    if (!isPinching && rightDiff < PINCH_DISTANCE_LOW)
                    {
                        tmpGo.GetComponent<Renderer>().material = Resources.Load("Materials/TransparentMat2", typeof(Material)) as Material;
                        isPinching = true;

                        PinchStart = rightPinchPos;
                    }

                    // End Pinch
                    if (isPinching && rightDiff >= PINCH_DISTANCE_HIGH)
                    {
                        isPinching = false;

                        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
                        Transform m_transform = go.GetComponent<Transform>();

                        Vector3[] resVertices = new Vector3[mesh.vertices.Length];                        
                        Vector3[] tmpVertices = mesh.vertices;

                        int[] tmpTriangles = mesh.triangles;

                        List<List<int>> oneRing = CollectOneRing(tmpVertices, tmpTriangles);

                        
                        for (int i = 0; i < tmpVertices.Length; i++)
                        {
                            Vector3 V = m_transform.TransformPoint(tmpVertices[i]); // Mesh point in world pos

                            if ((PinchStart - V).magnitude < MODIFY_DISTANCE)
                            {
                               resVertices[i] = SmoothVertex(i, 0.5f, oneRing, tmpVertices);
                            }else{
                                resVertices[i] = tmpVertices[i];
                            }
                        }

                        go.GetComponent<MeshFilter>().mesh.vertices = resVertices;
                        tmpGo.GetComponent<Renderer>().material = Resources.Load("Materials/TransparentMat", typeof(Material)) as Material;
                    }
                }
            }
        }    }


    public static Vector3 SmoothVertex(int vertInd, float coef, List<List<int>> oneRing, Vector3[] vertices) {
        // Récupérer la liste des 1-voisins du sommet
        List<int> oneRingVerts = oneRing[vertInd];
        
        // Initialiser la position smoothée du sommet à sa propre position
        Vector3 smoothPos = vertices[vertInd];
        
        // Pour chaque 1-voisin du sommet, ajouter sa position au total
        Debug.Log("OneRingSize = ");
        Debug.Log(oneRingVerts.Count);

        foreach (int oneRingVert in oneRingVerts) {
            smoothPos += vertices[oneRingVert];
        }
        
        // Diviser le total par le nombre de sommets adjacents + 1 pour obtenir la moyenne
        smoothPos /= (oneRingVerts.Count + 1);
        
        // Appliquer le coefficient de lissage
        smoothPos = Vector3.Lerp(vertices[vertInd], smoothPos, coef);
        
        return smoothPos;
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

    public static List<List<int>> CollectOneRing(Vector3[] vertices, int[] triangles)
    {
        // Cr�er un dictionnaire pour stocker les 1-voisinages de chaque sommet
        Dictionary<int, HashSet<int>> oneRing = new Dictionary<int, HashSet<int>>();

        // Pour chaque triangle, ajouter les sommets adjacents au 1-voisinage de chaque sommet
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int a = triangles[i];
            int b = triangles[i + 1];
            int c = triangles[i + 2];

            if (!oneRing.ContainsKey(a))
            {
                oneRing[a] = new HashSet<int>();
            }
            if (!oneRing.ContainsKey(b))
            {
                oneRing[b] = new HashSet<int>();
            }
            if (!oneRing.ContainsKey(c))
            {
                oneRing[c] = new HashSet<int>();
            }

            oneRing[a].Add(b);
            oneRing[a].Add(c);
            oneRing[b].Add(a);
            oneRing[b].Add(c);
            oneRing[c].Add(a);
            oneRing[c].Add(b);
        }

        // Conversion
        List<List<int>> oneRingList = new List<List<int>>();
        foreach (int key in oneRing.Keys)
        {
            oneRingList.Add(oneRing[key].ToList());
        }

        return oneRingList;
    }
}