using System;
using UnityEngine;

[Serializable]
public class Ally
{
    public Mb id;
    public Skill[] skills;

    public Sprite gunA, gunB;

    public AudioA fireClip, relStartClip, relEndClip, switchToClip;

    public RuntimeAnimatorController animation;

    public Stat clip; // ok
    public float fireCooldown, relCooldown;
    public bool reloadAll;

    public bool reloading;
    public float reloadTime;

    public float flashAmt, recAmt;

    public GameObject projectile;
    public DamageInfo infBase;

    public float[] se;
}
