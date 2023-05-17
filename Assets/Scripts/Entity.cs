using UnityEngine;

public abstract class Entity : MonoBehaviour, ITarget
{
    protected Vector2 startPos;
    [SerializeField]
    protected bool facingRight = true;

    public bool FacingRight { get => facingRight; }

    protected abstract void Awake();

    public abstract bool Damage(DamageInfo inf);

    public abstract void AddStatusEffect(Status status, float time);
}