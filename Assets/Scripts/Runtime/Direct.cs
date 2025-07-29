public class Direct : AbilityTargetFilter
{
    public int Range { get; private set; }
    
    
    internal Direct(int range)
    {
        Range = range;
    }


    public override bool IsValidTarget(Actor performer, Actor target)
    {
        return (performer.position - target.position).Length() < Range;
    }
}