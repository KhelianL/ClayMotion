using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendHandsToShader : MonoBehaviour
{
	Renderer m_ObjectRenderer;

    public GameObject L_index_end;
    public GameObject L_middle_end;
    public GameObject L_pinky_end;
    public GameObject L_ring_end;
    public GameObject L_thumb_end;

    public GameObject R_index_end;
    public GameObject R_middle_end;
    public GameObject R_pinky_end;
    public GameObject R_ring_end;
    public GameObject R_thumb_end;

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_ObjectRenderer.material.SetVector("_posLindex", L_index_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posLmiddle", L_middle_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posLpinky", L_pinky_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posLring", L_ring_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posLthumb", L_thumb_end.transform.position);

        m_ObjectRenderer.material.SetVector("_posRindex", R_index_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posRmiddle", R_middle_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posRpinky", R_pinky_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posRring", R_ring_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posRthumb", R_thumb_end.transform.position);
    }
}
