using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    private bool isActive;
    private Image thisImage;
    private float fillAmount;
    private float duration = 3;
    private Transform currentObj;
    public GameObject BouncyBall;
    public Camera Cam;
    private void Start()
    {
        thisImage = GetComponent<Image>();
    }
    void OnEnable()
    {
        fillAmount = 0;
        thisImage.fillAmount = fillAmount;
    }
    void Update()
    {
        if (isActive)
        {
            if (fillAmount != 1)
            {
                fillAmount = Mathf.Clamp(fillAmount + Time.deltaTime / duration, 0, 1);
                thisImage.fillAmount = fillAmount;
            }
        }
        Ray ray = new Ray(Cam.transform.position, Cam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit something");
            if (!hit.transform.gameObject.GetComponent<Plane>().BouncyCreated)
            {
                if (currentObj != hit.transform)
                {
                    Debug.Log("New tile");
                    currentObj = hit.transform;
                    fillAmount = 0;
                    thisImage.fillAmount = 0;
                    thisImage.enabled = true;
                    isActive = true;
                }
                else
                {
                    Debug.Log("Same tile");
                    if (fillAmount >= 1)
                    {
                        Debug.Log("Bounce time");
                        var go = Instantiate(BouncyBall) as GameObject; 
                        go.transform.position = hit.transform.position + new Vector3(0, 0.2f, 0);
                        go.transform.parent = hit.transform.parent;
                        hit.transform.gameObject.GetComponent<Plane>().BouncyCreated = true;
                        currentObj = null;
                        thisImage.enabled = false;
                        isActive = false;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Didn't hit anything");
            thisImage.enabled = false;
            isActive = false;
            currentObj = null;
        }
    }
}
