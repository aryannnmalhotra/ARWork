using System.Collections;
using GoogleARCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ARController : MonoBehaviour
{
    public GameObject Tile;
    public Camera FirstPersonCamera;
    public Text PlaneCount;
    List<DetectedPlane> allPlanes = new List<DetectedPlane>(); 
    void Start()
    {
        
    }
    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }
        Session.GetTrackables<DetectedPlane>(allPlanes, TrackableQueryFilter.All);
        PlaneCount.text = "Planes : " + allPlanes.Count.ToString();
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;
        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            if ((hit.Trackable is DetectedPlane) && Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) > 0)
            {
                var gameObject = Instantiate(Tile, hit.Pose.position, hit.Pose.rotation);
                Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
                gameObject.transform.parent = anchor.transform;
            }
        }
    }
}
