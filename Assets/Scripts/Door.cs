public class Door : Interactable
{
    public int toLoad;

    protected override void DoAThing()
    {
        StartCoroutine(MainManager.Inst.LoadLevel(toLoad));
    }
}
