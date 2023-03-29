using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : Panel
{
    private static AudioA clip;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        clip = new("transition");
    }

    public override void Set(bool foo)
    {
        if (foo) AudioManager.Play(clip);
        base.Set(foo);
    }
}
