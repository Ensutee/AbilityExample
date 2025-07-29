using System;


[Serializable]
public class Damage : AbilityEffect
{
	public enum Types
	{
		Melee,
		Ranged,
		Critical,
		Other
	}

	public enum Operations
	{
		None,
		Plus,
		Minus,
		MultipliedBy,
		DividedBy
	}

	public int Amount { get; private set; }
	public Types Type { get; private set; }
	public StatName Stat { get; private set; }
	public Operations Operation { get; private set; }
	

	public Damage(int targetGroupIndex, int amount, Types type)
		: base(targetGroupIndex)
	{
		Amount = amount;
		Type = type;
	}

	public Damage(int targetGroupIndex, int amount, Types type, StatName stat, Operations operation)
		: base(targetGroupIndex)
	{
		Amount = amount;
		Type = type;
		Stat = stat;
		Operation = operation;
	}


	public override void Execute(Actor performer, Actor[] targets)
	{
		int damage = Amount;
		damage = ApplyStatOperation(damage, performer);

		foreach (Actor target in targets)
		{
			target.Damage(damage);
		}
	}
	
	
	private int ApplyStatOperation(int damage, Actor performer)
	{
		return Operation switch
		{
			Operations.None => damage,
			Operations.Plus => performer.GetStat(Stat) + Amount,
			Operations.Minus => performer.GetStat(Stat) - Amount,
			Operations.MultipliedBy => performer.GetStat(Stat) * Amount,
			Operations.DividedBy => performer.GetStat(Stat) / Amount,
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}