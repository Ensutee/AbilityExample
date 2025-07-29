using System;
using System.Numerics;

public class Actor
{
    public Vector3 position;
    
    
    public int str = 1;
    public int dex = 2;
    public int con = 3;
    public int wis = 4;
    public int intel = 5;
    public int cha = 6;


    public int GetStat(StatName stat)
    {
        return stat switch
        {
            StatName.Strength => str,
            StatName.Dexterity => dex,
            StatName.Constitution => con,
            StatName.Wisdom => wis,
            StatName.Intelligence => intel,
            StatName.Charisma => cha,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };
    }


    public void Damage(int damage)
    {
    }
}