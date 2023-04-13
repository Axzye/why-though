using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected InputMaster input;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        input = new();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
