public abstract class AbilityEffect
{
    protected int targetGroupIndex { get; private set; }

	
    public AbilityEffect(int targetGroupIndex)
    {
        this.targetGroupIndex = targetGroupIndex;
    }


    public abstract void Execute(Actor performer, Actor[] targets);
}