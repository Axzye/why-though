using UnityEngine;

public class Panel : MonoBehaviour
{
    public bool open;
    private readonly int True = Animator.StringToHash("True"), False = Animator.StringToHash("False");
    protected Animator animate;

    protected virtual void Awake()
    {
        animate = GetComponent<Animator>();
    }

    public void Toggle()
    {
        Set(!open);
    }

    public virtual void Set(bool foo)
    {
        animate.CrossFade(foo? True : False, 0f);
        open = foo;
    } 
}
