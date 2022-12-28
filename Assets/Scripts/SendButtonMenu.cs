using System;
using Leap.Unity.Interaction;
using UnityEngine;

public enum SelectOption
{
    NONE,
    EXTRUDE,
    SPHERE,
    CUBE,
    CYLINDER
};


public class SendButtonMenu : MonoBehaviour
{
    public GameObject SphereButton;
    public GameObject CubeButton;
    public GameObject CylinderButton;
    public GameObject ExtrudeButton;

    public GameObject SphereObj;
    public GameObject CubeObj;
    public GameObject CylinderObj;
    public GameObject ExtrudeObj;
    public GameObject DeleteObj;

    private InteractionToggle toggleSphere;
    private InteractionToggle toggleCube;
    private InteractionToggle toggleCylinder;
    private InteractionToggle toggleExtrude;

    private Material[] materialsSphere;
    private Material[] materialsCube;
    private Material[] materialsCylinder;
    private Material[] materialsExtrude;
    private Material[] materialsDelete;

    protected SelectOption chooseMesh;

    void Start()
    {
        chooseMesh = SelectOption.SPHERE;

        toggleSphere = SphereButton.GetComponent<InteractionToggle>();
        toggleCube = CubeButton.GetComponent<InteractionToggle>();
        toggleCylinder = CylinderButton.GetComponent<InteractionToggle>();
        toggleExtrude = ExtrudeButton.GetComponent<InteractionToggle>();

        materialsSphere = SphereObj.GetComponent<Renderer>().materials;
        materialsCube = CubeObj.GetComponent<Renderer>().materials;
        materialsCylinder = CylinderObj.GetComponent<Renderer>().materials;
        materialsExtrude = ExtrudeObj.GetComponent<Renderer>().materials;
        materialsDelete = DeleteObj.GetComponent<Renderer>().materials;

        ToggleOnSphere();
    }

    public void ToggleOnSphere()
    {
        ToggleOffCube();
        ToggleOffCylinder();
        ToggleOffExtrude();
        SwitchToggle(SelectOption.SPHERE, true);
    }
    public void ToggleOffSphere()
    {
        SwitchToggle(SelectOption.SPHERE, false);
        toggleSphere.Untoggle();
    }
    public void ToggleOnCube()
    {
        ToggleOffSphere();
        ToggleOffCylinder();
        ToggleOffExtrude();
        SwitchToggle(SelectOption.CUBE, true);
    }
    public void ToggleOffCube()
    {
        SwitchToggle(SelectOption.CUBE, false);
        toggleCube.Untoggle();
    }
    public void ToggleOnCylinder()
    {
        ToggleOffSphere();
        ToggleOffCube();
        ToggleOffExtrude();
        SwitchToggle(SelectOption.CYLINDER, true);
    }
    public void ToggleOffCylinder()
    {
        SwitchToggle(SelectOption.CYLINDER, false);
        toggleCylinder.Untoggle();
    }


    public void ToggleOnExtrude()
    {
        ToggleOffSphere();
        ToggleOffCube();
        ToggleOffCylinder();
        SwitchToggle(SelectOption.EXTRUDE, true);
    }
    public void ToggleOffExtrude()
    {
        SwitchToggle(SelectOption.EXTRUDE, false);
        toggleExtrude.Untoggle();
    }

    public void PressOnDelete()
    {
        materialsDelete[0] = Resources.Load("Materials/GlowRedMat", typeof(Material)) as Material;
        DeleteObj.GetComponent<Renderer>().materials = materialsDelete;
        gameObject.GetComponent<MeshGenerator>().deleteObj();
    }
    public void PressOffDelete()
    {
        materialsDelete[0] = Resources.Load("Materials/WhiteMat", typeof(Material)) as Material;
        DeleteObj.GetComponent<Renderer>().materials = materialsDelete;
    }

    private void SwitchToggle(SelectOption call, bool b)
    {
        switch (call)
        {
            case SelectOption.SPHERE:
                materialsSphere[0] = Resources.Load((b ? "Materials/GlowGreenMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                SphereObj.GetComponent<Renderer>().materials = materialsSphere;
                break;
            case SelectOption.CUBE:
                materialsCube[0] = Resources.Load((b ? "Materials/GlowGreenMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                CubeObj.GetComponent<Renderer>().materials = materialsCube;
                break;
            case SelectOption.CYLINDER:
                materialsCylinder[0] = Resources.Load((b ? "Materials/GlowGreenMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                CylinderObj.GetComponent<Renderer>().materials = materialsCylinder;
                break;
            case SelectOption.EXTRUDE:
                materialsExtrude[0] = Resources.Load((b ? "Materials/GlowBlueMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                ExtrudeObj.GetComponent<Renderer>().materials = materialsExtrude;
                break;
            default:
                break;
        }
        chooseMesh = (b ? call : SelectOption.NONE);

    }
    public SelectOption ChooseMesh
    {
        get { return chooseMesh; }
    }
}
