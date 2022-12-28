using System;
using Leap.Unity.Interaction;
using UnityEngine;

public enum SelectMesh
{
    NONE,
    SPHERE,
    CUBE,
    CYLINDER
};


public class SendButtonMenu : MonoBehaviour
{
    public GameObject SphereButton;
    public GameObject CubeButton;
    public GameObject CylinderButton;

    public GameObject SphereObj;
    public GameObject CubeObj;
    public GameObject CylinderObj;

    private InteractionToggle toggleSphere;
    private InteractionToggle toggleCube;
    private InteractionToggle toggleCylinder;

    private Material[] materialsSphere;
    private Material[] materialsCube;
    private Material[] materialsCylinder;

    protected SelectMesh chooseMesh;

    void Start()
    {
        chooseMesh = SelectMesh.SPHERE;

        toggleSphere = SphereButton.GetComponent<InteractionToggle>();
        toggleCube = CubeButton.GetComponent<InteractionToggle>();
        toggleCylinder = CylinderButton.GetComponent<InteractionToggle>();

        materialsSphere = SphereObj.GetComponent<Renderer>().materials;
        materialsCube = CubeObj.GetComponent<Renderer>().materials;
        materialsCylinder = CylinderObj.GetComponent<Renderer>().materials;

        ToggleOnSphere();
    }

    public void ToggleOnSphere()
    {
        ToggleOffCube();
        ToggleOffCylinder();
        SwitchToggle(SelectMesh.SPHERE, true);
    }
    public void ToggleOffSphere()
    {
        SwitchToggle(SelectMesh.SPHERE, false);
        toggleSphere.Untoggle();
    }
    public void ToggleOnCube()
    {
        ToggleOffSphere();
        ToggleOffCylinder();
        SwitchToggle(SelectMesh.CUBE, true);
    }
    public void ToggleOffCube()
    {
        SwitchToggle(SelectMesh.CUBE, false);
        toggleCube.Untoggle();
    }
    public void ToggleOnCylinder()
    {
        ToggleOffSphere();
        ToggleOffCube();
        SwitchToggle(SelectMesh.CYLINDER, true);
    }
    public void ToggleOffCylinder()
    {
        SwitchToggle(SelectMesh.CYLINDER, false);
        toggleCylinder.Untoggle();
    }

    private void SwitchToggle(SelectMesh call, bool b)
    {
        switch (call)
        {
            case SelectMesh.SPHERE:
                materialsSphere[0] = Resources.Load((b ? "Materials/GlowRedMat" : "Materials/GreyMat"), typeof(Material)) as Material;
                SphereObj.GetComponent<Renderer>().materials = materialsSphere;
                break;
            case SelectMesh.CUBE:
                materialsCube[0] = Resources.Load((b ? "Materials/GlowRedMat" : "Materials/GreyMat"), typeof(Material)) as Material;
                CubeObj.GetComponent<Renderer>().materials = materialsCube;
                break;
            case SelectMesh.CYLINDER:
                materialsCylinder[0] = Resources.Load((b ? "Materials/GlowRedMat" : "Materials/GreyMat"), typeof(Material)) as Material;
                CylinderObj.GetComponent<Renderer>().materials = materialsCylinder;
                break;
            default:
                break;
        }
        chooseMesh = (b ? call : SelectMesh.NONE);

    }
    public SelectMesh ChooseMesh
    {
        get { return chooseMesh; }
    }
}
