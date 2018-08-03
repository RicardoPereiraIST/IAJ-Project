using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class PickUpChest : WalkToTargetAndExecuteAction
    {

        public PickUpChest(AutonomousCharacter character, GameObject target) : base("PickUpChest",character,target)
        {
        }


        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.GET_RICH_GOAL) change -= 5.0f;
            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return true;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return true;
        }


        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.PickUpChest(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL, goalValue - 5.0f);

            var money = (int)worldModel.GetProperty(Properties.MONEY);
            worldModel.SetProperty(Properties.MONEY, money + 5);

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }


        public override float H()
        {
            float heuristic = (1000 / (this.Target.transform.position - this.Character.transform.position).sqrMagnitude);
            return heuristic;
        }

        public override float H(WorldModel model)
        {
            int HP = (int)model.GetProperty(Properties.HP);
            float heuristic = 0.0f;

            Vector3 targetPos = Target.transform.position;

            List<GameObject> enemies = this.Character.GameManager.enemies;
            foreach (GameObject enemy in enemies)
            {
                if ((targetPos - enemy.transform.position).sqrMagnitude < 500)
                {
                    bool enemyEnable = (bool)model.GetProperty(enemy.name);

                    if (enemyEnable)
                    {
                        if (enemy.tag.Equals("Skeleton"))
                        {
                            if (HP <= 5)
                                heuristic -= 1;
                            else heuristic += 1;
                        }
                        else if (enemy.tag.Equals("Orc"))
                        {
                            if (HP <= 10)
                                heuristic -= 2;
                            else heuristic += 2;
                        }
                        else if (enemy.tag.Equals("Dragon"))
                        {
                            if (HP <= 20)
                                heuristic -= 3;
                            else heuristic += 3;
                        }
                    }
                    else
                    {
                        heuristic += 5;
                    }
                    break;
                }
            }

            return heuristic;
        }
    }
}
