using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject placementIndicator;

    public GameObject objectCube;
    public GameObject objectCylinder;
    public GameObject objectCone;
    public GameObject objectBasket;

    public List<Material> woodMaterials;
    public Material metalMat;

    private GameObject objectToPlace;
    private Color[] colors = new Color[] { Color.blue, Color.red, Color.green, Color.yellow };
    private int bowlsPlaced = 0;
    private int objectsPlaced = 0;

    private GameObject objectHeld = null;

    public Text debugText;
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private TrackableType planeDetectionMode = TrackableType.PlaneWithinBounds | TrackableType.PlaneWithinPolygon;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();
    }

    void Update()
    {
        if (MinecraftUI.instance)
            switch (MinecraftUI.instance.mode)
            {
                case MinecraftUI.Mode.PLACE:
                    UpdatePlacementPose();
                    UpdatePlacementIndicator();

                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.layer != 5))
                    {
                        switch (MinecraftUI.instance.objectType)
                        {
                            case MinecraftUI.Object.CUBE:
                                objectToPlace = objectCube;
                                break;
                            case MinecraftUI.Object.CONE:
                                objectToPlace = objectCone;
                                break;
                            case MinecraftUI.Object.CYLINDER:
                                objectToPlace = objectCylinder;
                                break;
                            default:
                                break;
                        }
                        // Try to append object. If it fails, place a new object on AR Plane
                        if (AppendCube() == false && placementPoseIsValid)
                            PlaceObject();
                    }

                    break;

                case MinecraftUI.Mode.PICKUP:
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.layer != 5))
                    {
                        Pickup();
                    }
                    break;

                default:
                    break;
            }
        else if (RedGreenBinUI.instance)
            switch (RedGreenBinUI.instance.mode)
            {
                case RedGreenBinUI.Mode.PLACEBASKET:
                    UpdatePlacementPose();
                    UpdatePlacementIndicator();
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.layer != 5))
                    {
                        objectToPlace = objectBasket;
                        PlaceObject();
                    }
                    break;

                case RedGreenBinUI.Mode.PLACEOBJECT:
                    UpdatePlacementPose();
                    UpdatePlacementIndicator();
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.layer != 5))
                    {
                        if (objectsPlaced < 4)
                            objectToPlace = objectCube;
                        else
                            objectToPlace = objectCone;

                        PlaceObject();
                    }
                    break;

                case RedGreenBinUI.Mode.PICKUP:
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.layer != 5))
                    {
                        Pickup();
                        if (objectsPlaced == 0)
                            RedGreenBinUI.instance.winPanel.SetActive(true);
                    }
                    break;

                default:
                    break;
            }
    }

    private bool AppendCube()
    {
        Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            debugText.text += "Raycast success!\n";
            GameObject obj = hit.collider.gameObject;
            Vector3 direction = hit.normal.normalized;

            System.Random rand = new System.Random();

            direction *= 1f; // hit.collider.bounds.size.x;

            Vector3 newPos = obj.transform.position + direction;

            GameObject obj2 = Instantiate(objectToPlace, newPos, obj.transform.rotation);

            debugText.text += obj.transform.position.ToString() + " " + newPos.ToString() + "\n";

            if (obj2.GetComponent<BoxCollider>() != null)     // if of type box, randomize wooden material
                obj2.GetComponent<Renderer>().material = woodMaterials[rand.Next(0, 3)];

            return true;
        }
        return false;
    }

    private void Pickup()
    {
        Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            debugText.text += "Raycast success!\n";
            debugText.text += hit.collider.gameObject.name + "\n";
        }

        if (MinecraftUI.instance)
        {
            hit.collider.gameObject.SetActive(false);
        }

        else if (RedGreenBinUI.instance)
        {
            if ((hit.collider.gameObject.GetComponent<BoxCollider>() || hit.collider.gameObject.GetComponent<SphereCollider>()) && objectHeld == null)
            {
                // Clicked on a non-bowl object
                objectHeld = hit.collider.gameObject;
                objectHeld.SetActive(false);
                debugText.text += "Picked up " + objectHeld.GetComponent<Renderer>().material.color.ToString() + "\n";
                RedGreenBinUI.instance.inventory.color = objectHeld.GetComponent<Renderer>().material.color;
            }
            else if (hit.collider.gameObject.GetComponent<MeshCollider>())
            {
                debugText.text += "Clicked on bowl " + hit.collider.gameObject.GetComponent<Renderer>().material.color.ToString() + "\n";
                if (objectHeld.GetComponent<Renderer>().material.color == hit.collider.gameObject.GetComponent<Renderer>().material.color)
                {
                    // if color of object and bowl match
                    debugText.text += "Colors match!\n";
                    objectHeld.SetActive(true);
                    objectHeld.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    objectHeld.gameObject.transform.position = Camera.current.transform.position;
                    objectHeld = null;
                    RedGreenBinUI.instance.inventory.color = Color.white;
                    objectsPlaced--;
                    RedGreenBinUI.instance.successPanel.SetActive(true);
                }
                else
                    RedGreenBinUI.instance.failurePanel.SetActive(true);
            }
            else
                debugText.text += "Clicked on weird object!\n";
        }
    }

    private void PlaceObject()
    {
        debugText.text += "Object placed\n";
        GameObject obj = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        if (RedGreenBinUI.instance)
        {
            obj.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(metalMat);
            if (RedGreenBinUI.instance.mode == RedGreenBinUI.Mode.PLACEBASKET)
                obj.GetComponent<Renderer>().material.color = colors[bowlsPlaced++];
            else if (RedGreenBinUI.instance.mode == RedGreenBinUI.Mode.PLACEOBJECT)
                obj.GetComponent<Renderer>().material.color = colors[(objectsPlaced++)%4];

            if (bowlsPlaced == 4)
                RedGreenBinUI.instance.mode = RedGreenBinUI.Mode.PLACEOBJECT;

            if (objectsPlaced == 8)
                RedGreenBinUI.instance.mode = RedGreenBinUI.Mode.PICKUP;
        }
    }

    private void UpdatePlacementIndicator()
    {
        placementIndicator.SetActive(placementPoseIsValid);

        if (placementPoseIsValid)
        {
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
    }

    private void UpdatePlacementPose()
    {
        if (Camera.current == null)
            return;

        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        Ray ray = Camera.current.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        // If there is a object in line of sight, cant place another object.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject obj = hit.collider.gameObject;

            if (obj.name.StartsWith("GameCube") || obj.name.StartsWith("GameSphere") || obj.name.StartsWith("GameCylinder"))
            {
                placementPoseIsValid = false;
                return;
            }
        }

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        
        raycastManager.Raycast(screenCenter, hits, planeDetectionMode);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
