using System;
using UnityEngine;

[Serializable]
public struct Stat : IFormattable
{
    [SerializeField]
    private int value, max;
    public int GetMax => max;

    public readonly float Percent
        => (float)value / max;
    public readonly bool Full
        => value == max;

    public void Set(int set)
    {
        if (set > max)
        {
            value = max;
        }
        else if (set < 0)
            value = 0;
        else
            value = set;
    }

    /// <summary>
    /// Adds to stat value. Specify a value to fill that amount.
    /// </summary>
    public void Add(int add) => Set(value + add);

    public string ToString(string format, IFormatProvider provider = default)
        => value.ToString(format, provider);

    public static implicit operator int(Stat val) => val.value;
}
