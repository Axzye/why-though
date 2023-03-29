using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    public TMP_Text dbText, dbNameText;
    public Image dbImage;
    public Image dbBack;
    public Animator dbAnim;

    public static bool inDlMode, isTyping;
    private float limitInTime;

    private static readonly int
        True = Animator.StringToHash("True"),
        False = Animator.StringToHash("False");

    private Queue<DlPage> pageQueue = new();
    private DlPage pageCurrent;
    private float typeTick;
    private bool skip;
    private float imageSquish;

    private InputMaster input;

    /// <summary>
    /// 0 = def
    /// 1 = talk
    /// 2 = blink / idle
    /// </summary>
    private void SetIcon(int value)
    {
        dbImage.sprite = pageCurrent.speaker.spriteset[(int)value + 3 * (int)pageCurrent.emotion];
    }

    protected override void Awake()
    {
        base.Awake();
        input = new();
    }

    private void OnEnable() => input.Player.Enable();
    private void OnDisable()
    {
        inDlMode = false;
        input.Player.Disable();
    }

    private void Update()
    {
        Utils.TimeDown(ref limitInTime);
        if (inDlMode)
        {
            if (input.Player.Use.triggered)
            {
                if (isTyping)
                {
                    skip = true;
                }
                else
                {
                    StartNextPage();
                }
            }

            if (isTyping)
            {
                typeTick += Time.unscaledDeltaTime;
                if (typeTick % 0.1f < Time.unscaledDeltaTime)
                    AudioManager.PlayAdv(pageCurrent.speaker.sound, Camera.main.transform.position + Vector3.forward);

                if (typeTick % 0.2f < 0.1f)
                {
                    SetIcon(1);
                    dbImage.rectTransform.anchoredPosition = Vector2.down * 0.5f;
                }
                else
                {
                    SetIcon(0);
                    dbImage.rectTransform.anchoredPosition = Vector2.zero;
                }
            }

            imageSquish *= Mathf.Pow(0.8f, Time.unscaledDeltaTime * 60f);
            dbImage.rectTransform.sizeDelta = new(38f / (1f + imageSquish), 38f * (1f + imageSquish));
        }
    }

    public bool StartDialogue(DlStory story)
    {
        if (inDlMode) return false;
        if (limitInTime > 0f) return false;

        Time.timeScale = 0f;
        inDlMode = true;
        skip = false;

        dbAnim.CrossFade(True, 0f);

        foreach (DlPage page in story.pages)
            pageQueue.Enqueue(page);

        StartNextPage();
        return true;
    }

    public void StartNextPage()
    {
        if (pageQueue.TryDequeue(out pageCurrent))
        {
            print("New page");
            StartCoroutine(TypePage());
        }
        else
        {
            print("End of story, so end dialogue");
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        Time.timeScale = 1f;
        inDlMode = false;
        limitInTime = 0.1f;

        dbText.text = "";

        dbAnim.CrossFade(False, 0f);
    }

    IEnumerator TypePage()
    {
        isTyping = true;
        typeTick = 0f;

        dbText.text = "";
        dbNameText.text = pageCurrent.speaker._name;
        dbNameText.color = pageCurrent.speaker.color;
        SetIcon(0);

        imageSquish = 0.1f;

        bool isTag = false;
        foreach (char letter in pageCurrent.text)
        {
            float timeToWait = 0.01f;
            if (skip)
            {
                print("skipped dialogue");
                skip = false;
                dbText.text = pageCurrent.text;
                //AudioManager.PlayAdv(pageCurrent.speaker.sound, Camera.main.transform.position + Vector3.forward);
                break;
            }

            switch (letter)
            {
                case '<':
                    isTag = true;
                    continue;
                case '>':
                    isTag = false;
                    continue;
                case '.' or ';' or '!' or '?':
                    timeToWait = 0.05f;
                    dbText.text += letter;
                    break;
                default:
                    dbText.text += letter;
                    break;
            }

            if (isTag) continue;
            yield return new WaitForSecondsRealtime(timeToWait);
        }

        isTyping = false;
        SetIcon(0);

        yield return null;
    }
}