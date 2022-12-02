using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPass : MonoBehaviour
{
	Renderer m_ObjectRenderer;
	public Transform target; 
	
    // Start is called before the first frame update
    void Start()
    {
        m_ObjectRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_ObjectRenderer.material.SetVector("_posTarget", target.position);
    }
}
