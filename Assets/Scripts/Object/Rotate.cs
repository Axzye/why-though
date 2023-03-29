using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed;

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, 0f, -speed * Time.deltaTime), Space.Self);
    }
}
