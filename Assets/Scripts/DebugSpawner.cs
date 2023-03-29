using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpawner : MonoBehaviour
{
    public KeyCode key;
    public GameObject spawn;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            GameObject newObj = GameManager.Spawn(spawn, Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f);
            newObj.SetActive(true);
        }

    }
}
