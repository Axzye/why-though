using UnityEngine;

public class NPC : Interactable
{
    public DlStory text;

    protected override void DoAThing()
    {
        DialogueManager.Inst.StartDialogue(text);
    }
}