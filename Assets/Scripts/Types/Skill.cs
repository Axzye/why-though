using System;
using UnityEngine;

[Serializable]
public class Skill
{
    public string _name;
    public Sprite icon;
    public float cooldown;
    public int cost;
    [NonSerialized] public float time;
}