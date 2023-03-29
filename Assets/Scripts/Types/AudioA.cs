using UnityEngine;
using UnityEditor;

[System.Serializable]
public class AudioA
{
    public AudioClip clip;
    public AudioClip[] clipRandom;
    public float volume = 0.5f;
    public float pitch = 1f;
    public float pitchVar = 0f;
    
    public AudioA() => Init(null, 0.5f, 1f, 0f);

    public AudioA(
        AudioClip clip,
        float volume = 0.5f,
        float pitch = 1f,
        float pitchVar = 0f)
    => Init(clip, volume, pitch, pitchVar);

    public AudioA(
        string load,
        float volume = 0.5f,
        float pitch = 1f,
        float pitchVar = 0f)
    => Init(Resources.Load<AudioClip>("Sounds/" + load), volume, pitch, pitchVar);

    private void Init(
        AudioClip clip,
        float volume,
        float pitch,
        float pitchVar)
    {
        this.clip = clip;
        this.volume = volume;
        this.pitch = pitch;
        this.pitchVar = pitchVar;
    }

    public AudioClip GetRandom() => clipRandom[Random.Range(0, clipRandom.Length)];
}

/*
[CustomPropertyDrawer(typeof(AudioA))]
public class AudioADrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
    }
}
*/
