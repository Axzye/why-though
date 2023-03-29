using UnityEngine;

public class WaterRender : MonoBehaviour
{
    private Transform pos;
    private Transform[] t;

    private void Start()
    {
        pos = Camera.main.transform;
        t = transform.GetComponentsInChildren<Transform>();
    }

    private void Update()
    {
        for (int i = 1; i < t.Length; i++)
        {
            transform.position = new(pos.position.x, -7f, -1f);
            t[i].localPosition = new(0f, (pos.position.y + 3f) * i * -0.1f, 0f);
        }
    }
}
