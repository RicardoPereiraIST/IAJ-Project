using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
	public class DepthLimitedGOAPDecisionMaking
	{
		public const int MAX_DEPTH = 3;
		public int ActionCombinationsProcessedPerFrame { get; set; }
		public float TotalProcessingTime { get; set; }
		public int TotalActionCombinationsProcessed { get; set; }
		public bool InProgress { get; set; }

		public CurrentStateWorldModel InitialWorldModel { get; set; }
		private List<Goal> Goals { get; set; }
		private WorldModel[] Models { get; set; }
		private Action[] ActionPerLevel { get; set; }
		public Action[] BestActionSequence { get; private set; }
		public Action BestAction { get; private set; }
		public float BestDiscontentmentValue { get; private set; }
		private int CurrentDepth {  get; set; }

		public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
		{
			this.ActionCombinationsProcessedPerFrame = 200;
			this.Goals = goals;
			this.InitialWorldModel = currentStateWorldModel;
		}

		public void InitializeDecisionMakingProcess()
		{
			this.InProgress = true;
			this.TotalProcessingTime = 0.0f;
			this.TotalActionCombinationsProcessed = 0;
			this.CurrentDepth = 0;
			this.Models = new WorldModel[MAX_DEPTH + 1];
			this.Models[0] = this.InitialWorldModel;
			this.ActionPerLevel = new Action[MAX_DEPTH];
			this.BestActionSequence = new Action[MAX_DEPTH];
			this.BestAction = null;
			this.BestDiscontentmentValue = float.MaxValue;
			this.InitialWorldModel.Initialize();
		}

		public Action ChooseAction()
		{
			var processedActions = 0;
			float currentValue = 0.0f;
            int CurrentDepth = 0;
            Action action;
            float bestActionDiscontentment = float.MaxValue;

            while (CurrentDepth >= 0)
            {
                if (processedActions > ActionCombinationsProcessedPerFrame)
                {
                    this.InProgress = false;
                    break;
                }

                if (CurrentDepth >= MAX_DEPTH)
                {
                    processedActions++;
                    currentValue = Models[CurrentDepth].CalculateDiscontentment(Goals);

                    if (currentValue < BestDiscontentmentValue)
                    {
                        BestDiscontentmentValue = currentValue;

                        ActionPerLevel = ActionPerLevel.OrderBy(a => a.GetDiscontentment(Goals)).ToArray();
                        BestAction = ActionPerLevel[0];
                        bestActionDiscontentment = ActionPerLevel[0].GetDiscontentment(Goals);

                        for (int i = 0; i < ActionPerLevel.Length; i++)
                        {
                            this.BestActionSequence[i] = ActionPerLevel[i];
                        }
                    }

                    CurrentDepth -= 1;
                    continue;
                }

                action = Models[CurrentDepth].GetNextAction();

                if (action != null && action.CanExecute())
                {
                    Models[CurrentDepth + 1] = Models[CurrentDepth].GenerateChildWorldModel();
                    action.ApplyActionEffects(Models[CurrentDepth + 1]);
                    ActionPerLevel[CurrentDepth] = action;
                    CurrentDepth += 1;
                }
                else
                    CurrentDepth -= 1;
            }

            this.TotalProcessingTime += Time.deltaTime;
            TotalActionCombinationsProcessed += processedActions;
			return this.BestAction;
		}
	}
}
