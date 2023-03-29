using UnityEngine;

public class Scroll : MonoBehaviour
{
    public Vector2 moveScale;
    public bool looping;
    private float width;
    private Vector3 startPos;

    private Transform _camera;

    private void Start()
    {
        _camera = Camera.main.transform;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
        startPos = transform.position;
        startPos.z = 12f;
        transform.parent = _camera;
    }

    private void Update()
    {
        Vector3 relativePos = _camera.position * -moveScale;
        transform.localPosition = relativePos + startPos;

        if (looping)
        {
            if (transform.localPosition.x > width * 2f) startPos.x -= width * 3f;
            if (transform.localPosition.x < -width * 2f) startPos.x += width * 3f;
        }
    }
}
