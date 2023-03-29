using UnityEngine;
using UnityEngine.EventSystems;

public class Utils : MonoBehaviour
{
    public static Color
        clearW = new(1f, 1f, 1f, 0f);

    /// <summary>
    /// Counts down a value, will tick to 0 before returning false
    /// </summary>
    /// <param name="deltaTime">Default: fixedDeltaTime</param>
    /// <returns>true if val is above 0, false otherwise</returns> 
    public static bool TimeDown(ref float val) => TimeDown(ref val, Time.deltaTime);
    public static bool TimeDown(ref float val, float increment)
    {
        if (val > 0f)
        {
            val -= increment;
            if (val < 0f)
                val = 0f;
            return true;
        }
        val = 0f;
        return false;
    }

    /// <summary>
    /// Counts up a value, will tick to 0 before returning false
    /// </summary>
    /// <returns>true if val is at target, false otherwise</returns>
    public static bool TimeDownTick(ref float val, float target = 0f) => TimeDownTick(ref val, target, Time.deltaTime);

    public static bool TimeDownTick(ref float val, float target, float deltaTime)
    {
        if (val == target) return false;

        if (val > target && !(val - deltaTime == target))
            val -= deltaTime;
        else if (val < target)
        {
            val = target;
            return true;
        }
        return false;
    }

    /// <summary>
    /// duh
    /// </summary>
    public static bool TimeUp(ref float val, float to = 0f) => TimeUp(ref val, to, Time.deltaTime);
    public static bool TimeUp(ref float val, float to, float deltaTime)
    {
        if (val < to)
        {
            val += deltaTime;
            if (val > to)
                val = to;
            return true;
        }
        val = to;
        return false;
    }

    public static Vector2 ExplosionForce(Vector2 pos, Vector2 targPos)
    {
        Vector2 explosionDir = targPos - pos;
        float explosionDistance = explosionDir.magnitude;
        explosionDir /= explosionDistance;

        return Mathf.Lerp(3f, 1.5f, explosionDistance * 0.5f) * explosionDir;
    }

    // i don't understand this but it works
    public static float Pow(float num, int pow)
    {
        if (pow < 0) return 0f;
        float result = 1f;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
                result *= num;
            pow >>= 1;
            num *= num;
        }
        return result;
    }
}

// thanks mar
public class Ref<T> { public T value; }