using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ButtonJuice : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private RectTransform rect;
    private bool hover, down;
    private float scale, scaleVel;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    
    // Update is called once per frame
    void Update()
    {
        float target = 0f;
        if (down) target = -0.08f;
        else if (hover) target = 0.1f;

        // ??
        scaleVel *= Mathf.Pow(0.8f, Time.unscaledDeltaTime * 60f);
        scaleVel += (target - scale) * Time.unscaledDeltaTime * 60f;
        scale += scaleVel * Mathf.Min(Time.unscaledDeltaTime, 0.5f) * 4f;
        scale = Mathf.Clamp(scale, -0.2f, 0.2f);
        rect.localScale = Vector2.one * (scale + 1f);
        rect.rotation = Quaternion.AngleAxis(Mathf.Sin(Time.unscaledTime * Mathf.PI * 0.6f) * scale * 20f, Vector3.forward);
    }

    public void OnPointerEnter(PointerEventData _) => hover = true;
    public void OnPointerExit(PointerEventData _) => hover = false;
    public void OnPointerDown(PointerEventData _) => down = true;
    public void OnPointerUp(PointerEventData _) => down = false;
}
