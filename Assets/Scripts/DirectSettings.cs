using System;
using UnityEngine;


namespace PortaPlay.TurnBasedTactics.Core.ScriptableObjects.TargetFilters
{
	[Serializable, ClassLabel("Direct")]
	public class DirectSettings : AbilityFilterSettings
	{
		[SerializeField]
		private int range = 0;


		public DirectSettings() { }

		public DirectSettings(int range)
		{
			this.range = range;
		}
		
		
		public override AbilityTargetFilter CreateAbilityTargetFilter()
		{
			return new Direct(range);
		}
	}
}