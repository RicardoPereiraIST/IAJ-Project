using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetManaPotion : WalkToTargetAndExecuteAction
    {
        public GetManaPotion(AutonomousCharacter character, GameObject target) : base("GetManaPotion",character,target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Mana < 10;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            return mana < 10;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetManaPotion(this.Target);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            worldModel.SetProperty(Properties.MANA, 10);
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float H()
        {
            float heuristic = this.Character.GameManager.characterData.XP;
            return heuristic;
        }

        public override float H(WorldModel model)
        {
            float heuristic = 0.0f;

            if ((bool)model.GetProperty("Orc1") || (bool)model.GetProperty("Orc2"))
                heuristic = (10 - (int)model.GetProperty(Properties.MANA))/2;

            return heuristic;
        }

    }
}
