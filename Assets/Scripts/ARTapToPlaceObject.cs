using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject placementIndicator;
    public GamePiece objectToPlace;
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
        switch (ModeHandler.instance.mode)
        {
            case ModeHandler.Mode.PLACE:
                UpdatePlacementPose();
                UpdatePlacementIndicator();

                if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    PlaceObject();
                }

                break;

            case ModeHandler.Mode.PICKUP:
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    Pickup();
                }
                break;

            case ModeHandler.Mode.APPEND:
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    AppendCube();
                }
                break;

            default:
                break;
        }

    }

    private void AppendCube()
    {
        Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            debugText.text += "Raycast success!\n";
            GameObject obj = hit.collider.gameObject;
            Vector3 direction = hit.normal.normalized;

            System.Random rand = new System.Random();

            direction *= 0.9f;
            direction *= hit.collider.bounds.size.x;

            Vector3 newPos = obj.transform.position + direction;

            GamePiece obj2 = Instantiate(objectToPlace, newPos, obj.transform.rotation);

            debugText.text += obj.transform.position.ToString() + " " + newPos.ToString() + "\n";
        }
    }

    private void Pickup()
    {
        Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            debugText.text += "Raycast success!\n";
            hit.collider.gameObject.SetActive(false);
        }
    }

    private void PlaceObject()
    {
        debugText.text += "Object placed\n";
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
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
