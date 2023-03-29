using System;
using UnityEngine;

[Serializable]
public struct DamageInfo
{
    public int damage;
    public float knockback;
    public bool alwaysKnockback;
    public Vector2 addForce;
    public float iFrames;
    public bool crit;
    public bool ignoreIFrames;
    public bool noKill;
}
