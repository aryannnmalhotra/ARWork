using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAnimator : MonoBehaviour
{
    private float closest;
    private float farthest;
    public float DistancingFactor;
    public Material SurfaceTexture;
    public Vector2 Speed;
    void Start()
    {
        closest = transform.localPosition.x;
        farthest = closest - (2 * DistancingFactor);
    }
    public void Farther()
    {
        transform.localPosition = new Vector3((Mathf.Clamp(transform.localPosition.x - DistancingFactor, farthest, closest)), transform.localPosition.y, transform.localPosition.z);
    }
    public void Closer()
    {
        transform.localPosition = new Vector3((Mathf.Clamp(transform.localPosition.x + DistancingFactor, farthest, closest)), transform.localPosition.y, transform.localPosition.z);
    }
    void Update()
    {
        SurfaceTexture.mainTextureOffset += Speed * AnimatedPoster.AnimSpeedFactor * Time.deltaTime;
    }
}
