using System.Collections;
using GoogleARCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedPoster : MonoBehaviour
{
    private bool isWindowCreated;
    private bool moreWorldCreation;
    private bool crateRender;
    private bool loading;
    private GameObject window;
    private GameObject currentMoreWorld;
    private Vector2 touchStart;
    private Vector2 touchEnd;
    private Touch touch;
    private List<AugmentedImage> trackedMarkers = new List<AugmentedImage>();
    public GameObject Window;
    public GameObject Barrel;
    public GameObject Crate;
    public AugmentedImageDatabase AugmentedImageDatabase;
    public Text MarkerCount;
    public Text WindowCreated;
    public static float AnimSpeedFactor;
    void Start()
    {
        currentMoreWorld = null;
        AnimSpeedFactor = 1;
    }
    void renderBarrel()
    {
        currentMoreWorld = Instantiate (Barrel) as GameObject;
        currentMoreWorld.transform.position = window.transform.position + new Vector3(0.07f, -0.23f, 2.85f);
        Invoke("renderNothing", 3);
    }
    void renderNothing()
    {
        if (currentMoreWorld)
            Destroy(currentMoreWorld);
        if (crateRender)
            Invoke("renderCrate", 3);
        else
            Invoke("renderBarrel", 3);
        crateRender = !crateRender;
    }
    void renderCrate()
    {
        currentMoreWorld = Instantiate (Crate) as GameObject;
        currentMoreWorld.transform.position = window.transform.position + new Vector3(0.37f, -0.23f, 2.85f);
        Invoke("renderNothing", 3);
    }
    void renderMoreWorld()
    {
        loading = true;
        if (crateRender)
        {
            currentMoreWorld = Instantiate (Crate) as GameObject;
            currentMoreWorld.transform.position = window.transform.position + new Vector3(0.37f, -0.23f, 2.85f);
            WindowCreated.text = "CRATE";
        }
        else
        {
            currentMoreWorld = Instantiate(Barrel) as GameObject;
            currentMoreWorld.transform.position = window.transform.position + new Vector3(0.07f, -0.23f, 2.85f);
            WindowCreated.text = "BARREL";
        }
        crateRender = !crateRender;
        Destroy(currentMoreWorld, 3);
        Invoke("loadingReset", 6);
    }
    void loadingReset()
    {
        loading = false;
    }
    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }
        Session.GetTrackables<AugmentedImage>(trackedMarkers, TrackableQueryFilter.Updated);
        foreach(AugmentedImage marker in trackedMarkers)
        {
            if (!isWindowCreated && marker.Name == "WindowMarker")
            {
                window = Instantiate (Window) as GameObject;
                window.transform.position = marker.CenterPose.position + new Vector3(-0.22f, -0.5f, 0);
                isWindowCreated = true;
                WindowCreated.text = "TRUE";
            }
            else if (isWindowCreated && !loading && marker.Name == "MoreWorld" && marker.TrackingMethod == 0)
            {
                renderMoreWorld();
            }
            else
            {

            }
        }
        MarkerCount.text = "DETECTED MARKERS : " + trackedMarkers.Count.ToString();
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                touchEnd = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEnd = touch.position;
                float verticalSwipeDistance = Mathf.Abs(touchStart.y - touchEnd.y);
                float horizontalSwipeDistance = Mathf.Abs(touchStart.x - touchEnd.x);
                if (verticalSwipeDistance >= 20 || horizontalSwipeDistance >= 20)
                {
                    if (verticalSwipeDistance >= horizontalSwipeDistance)
                    {
                        if (window != null)
                        {
                            Transform planes = window.transform.GetChild(0);
                            if (touchStart.y > touchEnd.y)
                            {
                                foreach (Transform plane in planes)
                                {
                                    if (plane.GetComponent<MaterialAnimator>() != null)
                                    {
                                        plane.GetComponent<MaterialAnimator>().Closer();
                                    }
                                }
                                WindowCreated.text = "DOWN";
                            }
                            else
                            {
                                foreach (Transform plane in planes)
                                {
                                    if (plane.GetComponent<MaterialAnimator>() != null)
                                    {
                                        plane.GetComponent<MaterialAnimator>().Farther();
                                    }
                                }
                                WindowCreated.text = "UP";
                            }
                        }
                    }
                    else
                    {
                        if (touchStart.x > touchEnd.x)
                        {
                            AnimSpeedFactor = Mathf.Clamp(AnimSpeedFactor - 0.5f, 0.5f, 1.5f);
                            WindowCreated.text = "LEFT";
                        }
                        else
                        {
                            AnimSpeedFactor = Mathf.Clamp(AnimSpeedFactor + 0.5f, 0.5f, 1.5f);
                            WindowCreated.text = "RIGHT";
                        }
                    }
                }
            }
        }
        for (int i = 0; i < trackedMarkers.Count; i++)
            trackedMarkers.RemoveAt(i);
    }
}
