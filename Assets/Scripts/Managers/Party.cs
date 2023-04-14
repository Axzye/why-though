using System.Collections.Generic;

public class Party : MonoSingleton<Party>
{
    public Ally CurrentAlly => allies[(int)CurrentID];
    public Mb CurrentID { get; set; }
    public List<Ally> allies;

    public AllyData[] loadAllies; // TEMP

    public Stat hp, tp;
    public int berries;

    private void Start()
    {
        // TEMP
        hp.Add(99);
        tp.Add(99);
        allies = new();
        foreach (AllyData data in loadAllies)
        {
            Ally set = Instantiate(data).stats;
            set.reloadTime = 0f;
            set.clip.Add(99);
            set.se = new float[System.Enum.GetValues(typeof(Status)).Length];
            allies.Add(set);
        }
    }

    private void FixedUpdate()
    {
        foreach (Ally set in allies)
        {
            foreach (Skill skill in set.skills)
                Utils.TimeDown(ref skill.time);

            for (int i = 0; i < set.se.Length; i++)
                Utils.TimeDown(ref set.se[i]);
        }
    }
}
