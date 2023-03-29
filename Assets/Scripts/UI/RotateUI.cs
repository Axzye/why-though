using UnityEngine;

public class RotateUI : MonoBehaviour
{
    public float speed;
    public RectTransform lockRotate;
    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rt.Rotate(new Vector3(0f, 0f, -speed * Time.unscaledDeltaTime), Space.Self);
        if (lockRotate)
            lockRotate.rotation = Quaternion.identity;
    }
}
