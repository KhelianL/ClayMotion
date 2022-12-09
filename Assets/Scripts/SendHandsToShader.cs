using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendHandsToShader : MonoBehaviour
{
    public HandManager handManager;
	Renderer m_ObjectRenderer;

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_ObjectRenderer.material.SetVector("_posLindex", handManager.L_index_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posLmiddle", handManager.L_middle_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posLpinky", handManager.L_pinky_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posLring", handManager.L_ring_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posLthumb", handManager.L_thumb_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posRindex", handManager.R_index_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posRmiddle", handManager.R_middle_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posRpinky", handManager.R_pinky_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posRring", handManager.R_ring_end.transform.position);
        m_ObjectRenderer.material.SetVector("_posRthumb", handManager.R_thumb_end.transform.position);
    }
}
