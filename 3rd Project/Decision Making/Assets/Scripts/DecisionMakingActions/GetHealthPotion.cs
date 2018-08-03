using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
	public class GetHealthPotion : WalkToTargetAndExecuteAction
	{
        private int hpCHange;

        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion",character,target)
		{
            hpCHange = character.GameManager.characterData.MaxHP - character.GameManager.characterData.HP;
        }

		public override bool CanExecute()
		{
			if (!base.CanExecute()) return false;
			return this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP;
		}

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change += -this.hpCHange;

            return change;
        }

        public override bool CanExecute(WorldModel worldModel)
		{
            if (!base.CanExecute(worldModel)) return false;

            try
            {
                int hp = (int)worldModel.GetProperty(Properties.HP);
                return hp < (int)worldModel.GetProperty(Properties.MAXHP);
            }
            catch
            {
                return false;
            }
        }


		public override void Execute()
		{
			base.Execute();
			this.Character.GameManager.GetHealthPotion(this.Target);
		}

		public override void ApplyActionEffects(WorldModel worldModel)
		{
			base.ApplyActionEffects(worldModel);
			worldModel.SetProperty(Properties.HP, Properties.MAXHP);
			//disables the target object so that it can't be reused again
			worldModel.SetProperty(this.Target.name, false);
		}

        public override float H()
        {
            float heuristic = this.Character.GameManager.characterData.MaxHP - this.Character.GameManager.characterData.HP;
            return heuristic;
        }

        public override float H(WorldModel model)
        {
            try
            {
                float heuristic = ((int)model.GetProperty(Properties.MAXHP) - (int)model.GetProperty(Properties.HP))/2;
                return heuristic;
            }
            catch { return H(); }
        }
    }
}