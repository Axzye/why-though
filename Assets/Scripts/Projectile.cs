using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public bool friendly;
    public bool enemy;
    public DamageInfo inf;
    public float maxTime, time;
    public float speed;

    protected abstract void Die(Vector2? pos = null);
}