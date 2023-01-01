using System;
using System.Collections.Generic;
using Leap.InteractionEngine.Examples;
using Leap.Unity.Interaction;
using LeapInternal;
using UnityEngine;

public enum SelectOption
{
    NONE,
    EXTRUDE,
    ROTATION,
    SPHERE,
    CUBE,
    CYLINDER
};


public class SendButtonMenu : MonoBehaviour
{
    public GameObject meshTransformTools;
    public Transform resetTransformTools;
    private GameObject closetsObject;

    public GameObject sphereButton;
    public GameObject cubeButton;
    public GameObject cylinderButton;
    public GameObject rotationButton;
    public GameObject extrudeButton;

    public GameObject sphereObj;
    public GameObject cubeObj;
    public GameObject cylinderObj;
    public GameObject rotationObj;
    public GameObject extrudeObj;
    public GameObject deleteObj;

    private InteractionToggle toggleSphere;
    private InteractionToggle toggleCube;
    private InteractionToggle toggleCylinder;
    private InteractionToggle toggleRotation;
    private InteractionToggle toggleExtrude;

    private Material[] materialsSphere;
    private Material[] materialsCube;
    private Material[] materialsCylinder;
    private Material[] materialsRotation;
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
        toggleRotation = rotationButton.GetComponent<InteractionToggle>();
        toggleExtrude = extrudeButton.GetComponent<InteractionToggle>();

        materialsSphere = sphereObj.GetComponent<Renderer>().materials;
        materialsCube = cubeObj.GetComponent<Renderer>().materials;
        materialsCylinder = cylinderObj.GetComponent<Renderer>().materials;
        materialsRotation = rotationObj.GetComponent<Renderer>().materials;
        materialsExtrude = extrudeObj.GetComponent<Renderer>().materials;
        materialsDelete = deleteObj.GetComponent<Renderer>().materials;

        ToggleOnSphere();
    }

    public void ToggleOnSphere()
    {
        ToggleOffCube();
        ToggleOffCylinder();
        ToggleOffRotation();
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
        ToggleOffRotation();
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
        ToggleOffRotation();
        ToggleOffExtrude();
        SwitchToggle(SelectOption.CYLINDER, true);
    }
    public void ToggleOffCylinder()
    {
        SwitchToggle(SelectOption.CYLINDER, false);
        toggleCylinder.Untoggle();
    }

    public void ToggleOnRotation()
    {
        ToggleOffSphere();
        ToggleOffCube();
        ToggleOffCylinder();
        ToggleOffExtrude();
        SwitchToggle(SelectOption.ROTATION, true);

        meshTransformTools.SetActive(true);

        findClosestObject();
    }
    public void ToggleOffRotation()
    {
        SwitchToggle(SelectOption.ROTATION, false);
        toggleRotation.Untoggle();

        meshTransformTools.SetActive(false);

        if (closetsObject != null)
        {
            closetsObject.GetComponent<Rigidbody>().isKinematic = false;
            closetsObject.GetComponent<InteractionBehaviour>().enabled = true;
            meshTransformTools.GetComponent<TransformTool>().target = resetTransformTools;
            closetsObject = null;
        }
    }

    public void ToggleOnExtrude()
    {
        ToggleOffSphere();
        ToggleOffCube();
        ToggleOffCylinder();
        ToggleOffRotation();
        SwitchToggle(SelectOption.EXTRUDE, true);

        findClosestObject();
    }
    public void ToggleOffExtrude()
    {
        SwitchToggle(SelectOption.EXTRUDE, false);
        toggleExtrude.Untoggle();

        if (closetsObject != null)
        {
            closetsObject.GetComponent<Rigidbody>().isKinematic = false;
            closetsObject.GetComponent<InteractionBehaviour>().enabled = true;
            meshTransformTools.GetComponent<TransformTool>().target = resetTransformTools;
            closetsObject = null;
        }
    }

    public void PressOnDelete()
    {
        materialsDelete[0] = Resources.Load("Materials/GlowRedMat", typeof(Material)) as Material;
        deleteObj.GetComponent<Renderer>().materials = materialsDelete;

        ToggleOffRotation();
        gameObject.GetComponent<MeshGenerator>().deleteObj();
    }
    public void PressOffDelete()
    {
        materialsDelete[0] = Resources.Load("Materials/WhiteMat", typeof(Material)) as Material;
        deleteObj.GetComponent<Renderer>().materials = materialsDelete;
        ToggleOffExtrude();
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
            case SelectOption.ROTATION:
                materialsRotation[0] = Resources.Load((b ? "Materials/GlowBlueMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                rotationObj.GetComponent<Renderer>().materials = materialsRotation;
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

    private void findClosestObject()
    {
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
        if (closetsObject != null)
        {
            closetsObject.GetComponent<Rigidbody>().isKinematic = true;
            closetsObject.transform.position = meshTransformTools.transform.position;
            closetsObject.GetComponent<InteractionBehaviour>().enabled = false;
            meshTransformTools.GetComponent<TransformTool>().target = closetsObject.transform;
        }
        else
        {
            meshTransformTools.GetComponent<TransformTool>().target = resetTransformTools;
        }
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
