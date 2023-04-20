using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    public AudioClip defMusic;
    public List<Vector2> spawnPos;

    private void Awake()
    {     
        spawnPos = new();
        GameObject[] foundPos = GameObject.FindGameObjectsWithTag("SpawnPos");
        if (foundPos.Length == 0) Debug.LogError("No spawn positions found in level");

        foreach(GameObject pos in foundPos)
        {
            spawnPos.Add(pos.transform.position);
            Destroy(pos);
        }
    }
}
