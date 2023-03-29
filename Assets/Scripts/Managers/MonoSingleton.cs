using UnityEngine;

[DefaultExecutionOrder(-200)]
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Inst { get; private set; }

    protected virtual void Awake()
    {
        if (Inst)
        {
            Destroy(Inst.gameObject);
            Debug.LogWarning("Duplicate " + name + " in scene!");
        }
        Inst = (T)(object)this;
    }
}