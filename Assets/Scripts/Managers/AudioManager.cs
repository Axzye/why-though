using System.Collections;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    private AudioSource[] aS;
    private AudioLowPassFilter aL;
    private float vVal = 0.5f, vCurrent;
    private float lVal = 1f, lCurrent;

    protected override void Awake()
    {
        base.Awake();
        aS = GetComponentsInChildren<AudioSource>();
        aL = GetComponentInChildren<AudioLowPassFilter>();
    }

    private void OnEnable() => MainManager.OnLoad += Load;
    private void OnDisable() => MainManager.OnLoad -= Load;

    private void Load(Level l, int i)
    {
        // TEMP
        aS[1].clip = l.defMusic;
        aS[1].Play();
    }

    void Update()
    {
        float vTarget = 0.5f, lTarget = 1f;
        if (GameManager.paused)
        {
            vTarget = 0.4f;
            lTarget = 0.4f;
        }

        vVal = Mathf.SmoothDamp(vVal, vTarget, ref vCurrent,
            0.25f, Mathf.Infinity, Time.unscaledDeltaTime);
        lVal = Mathf.SmoothDamp(lVal, lTarget, ref lCurrent,
            0.25f, Mathf.Infinity, Time.unscaledDeltaTime);

        aL.cutoffFrequency = Utils.Pow(lVal, 2) * 22000f;
        aS[1].volume = vVal;
    }

    /// <summary>
    /// Use PlayAdv for pitch and position.
    /// </summary>
    public static void Play(in AudioA audio)
    {
        Inst.aS[0].PlayOneShot(
            audio.clipRandom != null && audio.clipRandom.Length > 0 ?
            audio.GetRandom()
            : audio.clip,
            audio.volume);
    }

    public static void PlayAdv(in AudioA audio, Vector2 pos)
    {
        Inst.StartCoroutine(StartAudio(audio, pos));
    }

    private static IEnumerator StartAudio(AudioA audio, Vector2 pos)
    {
        AudioClip play =
            !(audio.clipRandom == null || audio.clipRandom.Length == 0) ?
            audio.GetRandom() : audio.clip;

        GameObject gameObject = new("Audio");
        gameObject.transform.position = pos;

        AudioSource asNew = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        asNew.clip = play;
        asNew.volume = audio.volume;
        asNew.spatialBlend = 1f;
        asNew.pitch = audio.pitch
            + Random.Range(-audio.pitchVar, audio.pitchVar);
        asNew.rolloffMode = AudioRolloffMode.Linear;

        asNew.Play();

        yield return new WaitForSecondsRealtime(play.length / audio.pitch);

        Destroy(gameObject);

        yield return null;
    }
}
