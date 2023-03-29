using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFade : MonoBehaviour
{
    private Light2D light2D;

    private void Start() => light2D = GetComponent<Light2D>();

    private void FixedUpdate()
    {
        light2D.intensity *= 0.9f;
        if (light2D.intensity < 0.1f) light2D.enabled = false;
    }
}
