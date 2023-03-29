using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SiblingRuleTile : RuleTile
{
    public enum SiblingGroup
    {
        Terrain
    }
    public SiblingGroup group;

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        switch (neighbor)
        {
            case 1:
                {
                    return other is SiblingRuleTile
                        && (other as SiblingRuleTile).group == this.group;
                }
            case 2:
                {
                    return !(other is SiblingRuleTile
                        && (other as SiblingRuleTile).group == this.group);
                }
        }

        return base.RuleMatch(neighbor, other);
    }
}