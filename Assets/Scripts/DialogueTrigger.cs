using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DlStory text;

    public void Trigger()
    {
        DialogueManager.Inst.StartDialogue(text);
    }
}
