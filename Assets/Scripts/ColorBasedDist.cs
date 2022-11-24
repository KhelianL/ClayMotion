using UnityEngine;
using Leap.Unity.HandsModule;

public class ColorBasedDist : MonoBehaviour
{
    public Transform obj;

    private float MinDistance;
    private float MaxDistance;
    private float distance;
    private Material m;

    // Start is called before the first frame update
    void Start()
    {
        float distActive = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3.0f;
        MinDistance = distActive;
        MaxDistance = distActive + 0.2f;
        m = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, obj.transform.position);
        Color c = m.color;
        c.r = Mathf.InverseLerp(MaxDistance, MinDistance, distance);
        m.color = c;
        GetComponent<MeshRenderer>().material = m;
    }
}