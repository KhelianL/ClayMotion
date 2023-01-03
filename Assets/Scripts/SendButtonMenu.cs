using System.Collections.Generic;
using Leap.InteractionEngine.Examples;
using Leap.Unity.Interaction;
using UnityEngine;

public enum SelectOption
{
    NONE,
    EXTRUDE,
    ROTATION,
    SMOOTH,
    SPHERE,
    CUBE,
    CYLINDER
};


public class SendButtonMenu : MonoBehaviour
{
    public GameObject meshTransformTools;
    public Transform resetTransformTools;
    private GameObject closetsObject;
    private bool passSmooth = false;

    public GameObject sphereButton;
    public GameObject cubeButton;
    public GameObject cylinderButton;
    public GameObject rotationButton;
    public GameObject smoothButton;
    public GameObject extrudeButton;

    public GameObject sphereObj;
    public GameObject cubeObj;
    public GameObject cylinderObj;
    public GameObject rotationObj;
    public GameObject extrudeObj;
    public GameObject smoothObj;
    public GameObject deleteObj;

    private InteractionToggle toggleSphere;
    private InteractionToggle toggleCube;
    private InteractionToggle toggleCylinder;
    private InteractionToggle toggleRotation;
    private InteractionToggle toggleSmooth;
    private InteractionToggle toggleExtrude;

    private Material[] materialsSphere;
    private Material[] materialsCube;
    private Material[] materialsCylinder;
    private Material[] materialsRotation;
    private Material[] materialsExtrude;
    private Material[] materialsSmooth;
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
        toggleSmooth = smoothButton.GetComponent<InteractionToggle>();
        toggleExtrude = extrudeButton.GetComponent<InteractionToggle>();

        materialsSphere = sphereObj.GetComponent<Renderer>().materials;
        materialsCube = cubeObj.GetComponent<Renderer>().materials;
        materialsCylinder = cylinderObj.GetComponent<Renderer>().materials;
        materialsRotation = rotationObj.GetComponent<Renderer>().materials;
        materialsExtrude = extrudeObj.GetComponent<Renderer>().materials;
        materialsSmooth = smoothObj.GetComponent<Renderer>().materials;
        materialsDelete = deleteObj.GetComponent<Renderer>().materials;

        ToggleOnSphere();
    }

    public void ToggleOnSphere()
    {
        ToggleOffCube();
        ToggleOffCylinder();
        ToggleOffRotation();
        ToggleOffExtrude();
        ToggleOffSmooth();

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
        ToggleOffSmooth();

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
        ToggleOffSmooth();

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
        ToggleOffSmooth();


        findClosestObject();
        if (closetsObject != null)
        {
            SwitchToggle(SelectOption.ROTATION, true);
            meshTransformTools.SetActive(true);
            meshTransformTools.GetComponent<TransformTool>().target = closetsObject.transform;
            meshTransformTools.transform.position = closetsObject.transform.position;
            meshTransformTools.transform.localScale = (closetsObject.transform.localScale.magnitude < 0.15f ? new Vector3(0.6f, 0.6f, 0.6f) : closetsObject.transform.localScale * 4.0f);
        }
    }
    public void ToggleOffRotation()
    {
        SwitchToggle(SelectOption.ROTATION, false);
        toggleRotation.Untoggle();

        meshTransformTools.SetActive(false);
        meshTransformTools.GetComponent<TransformTool>().target = resetTransformTools;

        if (closetsObject != null)
        {
            closetsObject = null;
        }
    }

    public void ToggleOnExtrude()
    {
        ToggleOffSphere();
        ToggleOffCube();
        ToggleOffCylinder();
        ToggleOffRotation();
        ToggleOffSmooth();

        findClosestObject();
        if (closetsObject != null)
        {
            SwitchToggle(SelectOption.EXTRUDE, true);
            closetsObject.GetComponent<InteractionBehaviour>().enabled = false;
        }
    }
    public void ToggleOffExtrude()
    {
        SwitchToggle(SelectOption.EXTRUDE, false);
        toggleExtrude.Untoggle();

        if (closetsObject != null)
        {
            closetsObject.GetComponent<InteractionBehaviour>().enabled = true;
            closetsObject = null;
        }

        GameObject go = gameObject.GetComponent<MeshGenerator>().TmpGo;
        if(go != null)
        {
            Destroy(go);
        }
    }

    public void ToggleOnSmooth()
    {
        ToggleOffSphere();
        ToggleOffCube();
        ToggleOffCylinder();
        ToggleOffRotation();
        ToggleOffExtrude();

        passSmooth = true;

        findClosestObject();
        if (closetsObject != null)
        {
            SwitchToggle(SelectOption.SMOOTH, true);
        }
    }
    public void ToggleOffSmooth()
    {
        SwitchToggle(SelectOption.SMOOTH, false);
        toggleSmooth.Untoggle();

        passSmooth = false;

        GameObject go = gameObject.GetComponent<MeshGenerator>().TmpGo;
        if (go != null)
        {
            Destroy(go);
        }

        if (closetsObject != null)
        {
            closetsObject = null;
        }
    }


    public void PressOnDelete()
    {
        materialsDelete[0] = Resources.Load("Materials/GlowRedMat", typeof(Material)) as Material;
        deleteObj.GetComponent<Renderer>().materials = materialsDelete;

        ToggleOffRotation();
        ToggleOffExtrude();
        ToggleOffSmooth();

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
            case SelectOption.ROTATION:
                materialsRotation[0] = Resources.Load((b ? "Materials/GlowBlueMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                rotationObj.GetComponent<Renderer>().materials = materialsRotation;
                break;
            case SelectOption.EXTRUDE:
                materialsExtrude[0] = Resources.Load((b ? "Materials/GlowBlueMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                extrudeObj.GetComponent<Renderer>().materials = materialsExtrude;
                break;
            case SelectOption.SMOOTH:
                materialsSmooth[0] = Resources.Load((b ? "Materials/GlowBlueMat" : "Materials/WhiteMat"), typeof(Material)) as Material;
                smoothObj.GetComponent<Renderer>().materials = materialsSmooth;
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
    }

    public SelectOption ChooseMesh
    {
        get { return chooseMesh; }
    }

    public GameObject ClosetsObject
    {
        get { return closetsObject; }
    }

    public bool PassSmooth
    {
        get { return passSmooth; }
        set { passSmooth = value; }
    }
}
