using UnityEngine;

public abstract class Entity : MonoBehaviour, ITarget
{
    protected Vector2 startPos;
    public bool facingRight = true;

    protected abstract void Awake();

    public abstract bool Damage(DamageInfo inf);

    public abstract void AddStatusEffect(Status status, float time);
}