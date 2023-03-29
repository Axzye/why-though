using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject inst;
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            GameManager.Spawn(
                inst,
                transform.position);
        }
    }
}
