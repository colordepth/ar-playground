using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

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

        RedGreenBinUI.instance.inventory.color = colors[0];
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
        else if (WheresPUI.instance)
            switch (WheresPUI.instance.mode)
            {
                case WheresPUI.Mode.PLACEOBJECT:
                    UpdatePlacementPose();
                    UpdatePlacementIndicator();
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.layer != 5))
                    {
                        debugText.text += "Inside WheresPUI PlaceObject click\n";
                        objectToPlace = WheresPUI.instance.objects[objectsPlaced];
                        PlaceObject();
                    }
                    break;

                case WheresPUI.Mode.PICKUP:
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.layer != 5))
                    {
                        debugText.text += "Inside WheresPUI Pickup click\n";

                        if (objectsPlaced == 0)
                            break;

                        Pickup();

                        if (objectsPlaced == 0)
                        {
                            WheresPUI.instance.Speak("Congratulations!");
                            WheresPUI.instance.winPanel.SetActive(true);
                        }
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
            debugText.text += "Append Raycast success!\n";
            GameObject obj = hit.collider.gameObject;
            Vector3 direction = hit.normal.normalized;

            System.Random rand = new System.Random();

            direction *= 1f; // hit.collider.bounds.size.x;

            Vector3 newPos = obj.transform.position + direction;

            GameObject obj2 = Instantiate(objectToPlace, newPos, obj.transform.rotation);

            debugText.text += obj.transform.position.ToString() + " " + newPos.ToString() + "\n";

            //if (MinecraftUI.instance.objectColor == Color.yellow)
            //    obj2.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(woodMaterials[2]);
            //else
            {
                // obj.GetComponent<Renderer>().material.mainTexture = null;
                // obj2.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(metalMat);
                obj2.GetComponent<Renderer>().material.color = MinecraftUI.instance.objectColor;
            }

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
            debugText.text += "Pickup Raycast success!\n";
            debugText.text += hit.collider.gameObject.name + "\n";
        }
        else
        {
            debugText.text += "No object\n";
            return;
        }

        if (MinecraftUI.instance)
        {
            debugText.text += "Minecraft pickup\n";
            hit.collider.gameObject.SetActive(false);
        }

        else if (RedGreenBinUI.instance)
        {
            debugText.text += "RedGreen pickup\n";
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
                GameObject bowlClicked = hit.collider.gameObject;

                if (!(Physics.Raycast(new Ray(Camera.current.transform.position, Vector3.down), out hit, Mathf.Infinity)
                    && hit.collider.gameObject.GetComponent<MeshCollider>()))
                {
                    // If bowl is not under the camera
                    return;
                }

                if (objectHeld.GetComponent<Renderer>().material.color == bowlClicked.GetComponent<Renderer>().material.color)
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
        else if (WheresPUI.instance)
        {
            debugText.text += "WheresPUI pickup\n";
            objectHeld = hit.collider.gameObject;
            debugText.text += "Calling check riddle\n";
            // assuming successful match with riddle
            if (checkRiddleMatch(objectHeld))
            {
                objectHeld.SetActive(false);
                debugText.text += "Picked up " + objectHeld.name + "\n";
                WheresPUI.instance.Speak(WheresPUI.instance.objects[objectsPlaced - 1].name);
                // WheresPUI.instance.inventory.color = objectHeld.GetComponent<Renderer>().material.color;
                WheresPUI.instance.successPanel.SetActive(true);
                objectsPlaced--;
            }
            else
            {
                debugText.text += "Bad answer\n";
                WheresPUI.instance.failurePanel.SetActive(true);
            }
        }
        else
            debugText.text += "No instance found\n";
    }

    public void SpeakRiddle()
    {
        if (objectsPlaced > 0)
            WheresPUI.instance.Speak(WheresPUI.instance.riddles[objectsPlaced - 1]);
    }

    private bool checkRiddleMatch(GameObject objectHeld)
    {
        debugText.text += "Inside check riddle\n";
        debugText.text += "Comparing " + objectHeld.name + " - " + WheresPUI.instance.objects[objectsPlaced-1].name + "\n";
        return objectHeld.name.Contains(WheresPUI.instance.objects[objectsPlaced-1].name);
        /*
        switch (objectsPlaced)
        {
            case 1:
                return objectHeld.name.Contains("Cat");
            case 2:
                return objectHeld.name.Contains("Banana");
            case 3:
                return objectHeld.name.Contains("Duck");
            case 4:
                return objectHeld.name.Contains("IceCream");
            default:
                return false;
        }
        */
    }

    private void PlaceObject()
    {
        debugText.text += "Object placed\n";
        GameObject obj = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);

        if (RedGreenBinUI.instance)
        {
            obj.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(metalMat);
            if (RedGreenBinUI.instance.mode == RedGreenBinUI.Mode.PLACEBASKET)
            {
                obj.GetComponent<Renderer>().material.color = colors[bowlsPlaced++];
                
                RedGreenBinUI.instance.inventory.color = colors[bowlsPlaced%colors.Length];
            }
            else if (RedGreenBinUI.instance.mode == RedGreenBinUI.Mode.PLACEOBJECT)
            {
                obj.GetComponent<Renderer>().material.color = colors[(objectsPlaced++) % colors.Length];

                if (objectsPlaced < 8)
                    RedGreenBinUI.instance.inventory.color = colors[objectsPlaced%4];
                else
                    RedGreenBinUI.instance.inventory.color = Color.white;
            }

            if (bowlsPlaced == 4)
            {
                RedGreenBinUI.instance.mode = RedGreenBinUI.Mode.PLACEOBJECT;
                //RedGreenBinUI.instance.placeObjectsPanel.SetActive(true);
            }

            if (objectsPlaced == 8)
            {
                RedGreenBinUI.instance.mode = RedGreenBinUI.Mode.PICKUP;
                placementPoseIsValid = false;
                placementIndicator.SetActive(false);
                //RedGreenBinUI.instance.startGamePanel.SetActive(true);
            }
        }
        else if (WheresPUI.instance)
        {
            objectsPlaced++;
            if (objectsPlaced == 4)
            {
                WheresPUI.instance.mode = WheresPUI.Mode.PICKUP;
                placementPoseIsValid = false;
                placementIndicator.SetActive(false);
                WheresPUI.instance.Speak(WheresPUI.instance.riddles[objectsPlaced-1]);
            }
            obj.transform.rotation = objectToPlace.transform.localRotation * obj.transform.rotation;
        }
        else if (MinecraftUI.instance)
        {
            //if (MinecraftUI.instance.objectColor == Color.yellow)
            //    obj.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(woodMaterials[2]);
            //else
            {
                // obj.GetComponent<Renderer>().material.mainTexture = null;
                // obj.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(metalMat);
                obj.GetComponent<Renderer>().material.color = MinecraftUI.instance.objectColor;
            }
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
