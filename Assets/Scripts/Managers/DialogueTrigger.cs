using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DlStory text;

    public virtual void Start()
    {
    }

    public void Trigger()
    {
        DialogueManager.Inst.StartDialogue(text);
    }
}
