using System;
using System.Collections.Generic;
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
    public GameObject meshContainer;
    private GameObject closetsObject;

    public GameObject sphereButton;
    public GameObject cubeButton;
    public GameObject cylinderButton;
    public GameObject extrudeButton;

    public GameObject sphereObj;
    public GameObject cubeObj;
    public GameObject cylinderObj;
    public GameObject extrudeObj;
    public GameObject deleteObj;

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
        closetsObject = null;

        toggleSphere = sphereButton.GetComponent<InteractionToggle>();
        toggleCube = cubeButton.GetComponent<InteractionToggle>();
        toggleCylinder = cylinderButton.GetComponent<InteractionToggle>();
        toggleExtrude = extrudeButton.GetComponent<InteractionToggle>();

        materialsSphere = sphereObj.GetComponent<Renderer>().materials;
        materialsCube = cubeObj.GetComponent<Renderer>().materials;
        materialsCylinder = cylinderObj.GetComponent<Renderer>().materials;
        materialsExtrude = extrudeObj.GetComponent<Renderer>().materials;
        materialsDelete = deleteObj.GetComponent<Renderer>().materials;

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

        meshContainer.SetActive(true);

        closetsObject = null;
        float oldDistance = 9999;
        List<GameObject> NearGameobjects = gameObject.GetComponent<MeshGenerator>().ListObj;

        foreach (GameObject g in NearGameobjects)
        {
            if (g != null)
            {
                float dist = Vector3.Distance(this.gameObject.transform.position, g.transform.position);
                if (dist < oldDistance)
                {
                    closetsObject = g;
                    oldDistance = dist;
                }
            }
        }

        closetsObject.GetComponent<Rigidbody>().isKinematic = true;
        closetsObject.transform.position = meshContainer.transform.position;
        closetsObject.GetComponent<InteractionBehaviour>().enabled = false;
    }
    public void ToggleOffExtrude()
    {
        SwitchToggle(SelectOption.EXTRUDE, false);
        toggleExtrude.Untoggle();

        meshContainer.SetActive(false);
        if (closetsObject != null)
        {
            closetsObject.GetComponent<Rigidbody>().isKinematic = false;
            closetsObject.GetComponent<InteractionBehaviour>().enabled = true;
        }
        closetsObject = null;
    }

    public void PressOnDelete()
    {
        materialsDelete[0] = Resources.Load("Materials/GlowRedMat", typeof(Material)) as Material;
        deleteObj.GetComponent<Renderer>().materials = materialsDelete;
        gameObject.GetComponent<MeshGenerator>().deleteObj();
    }
    public void PressOffDelete()
    {
        materialsDelete[0] = Resources.Load("Materials/WhiteMat", typeof(Material)) as Material;
        deleteObj.GetComponent<Renderer>().materials = materialsDelete;
    }

    private void SwitchToggle(SelectOption call, bool b)
    {
        switch (call)
        {
            case SelectOption.SPHERE:
                materialsSphere[0] = Resources.Load((b ? "Materials/GlowGreenMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                sphereObj.GetComponent<Renderer>().materials = materialsSphere;
                break;
            case SelectOption.CUBE:
                materialsCube[0] = Resources.Load((b ? "Materials/GlowGreenMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                cubeObj.GetComponent<Renderer>().materials = materialsCube;
                break;
            case SelectOption.CYLINDER:
                materialsCylinder[0] = Resources.Load((b ? "Materials/GlowGreenMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                cylinderObj.GetComponent<Renderer>().materials = materialsCylinder;
                break;
            case SelectOption.EXTRUDE:
                materialsExtrude[0] = Resources.Load((b ? "Materials/GlowBlueMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                extrudeObj.GetComponent<Renderer>().materials = materialsExtrude;
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

    public GameObject ClosetsObject
    {
        get { return closetsObject; }
    }
}
