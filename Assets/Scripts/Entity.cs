using UnityEngine;

public abstract class Entity : MonoBehaviour, ITarget
{
    protected Vector2 startPos;
    public bool facing = true;


    protected virtual void Awake()
    {
        startPos = transform.position;
    }

    public abstract bool Damage(DamageInfo inf);

    public abstract void AddStatusEffect(Status status, float time);
}