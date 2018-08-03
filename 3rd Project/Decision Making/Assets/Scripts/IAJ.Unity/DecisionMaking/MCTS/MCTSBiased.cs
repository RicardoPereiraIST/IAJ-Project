using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameManager;
using System.Collections.Specialized;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiased : MCTS
    {
        public MCTSBiased(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel) { }

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            WorldModel model = initialPlayoutState.GenerateChildWorldModel();
            List<GOB.Action> actions;
            List<GOB.Action> executableActions = new List<GOB.Action>();
            GOB.Action nextAction = null;
            Reward reward = new Reward();
            double heuristicValue;
            double accumulatedHeuristicValue;
            double bestValue, minValue;
            SortedDictionary<double, GOB.Action> heuristicList = new SortedDictionary<double, GOB.Action>();
            actions = model.GetActions();

            while (!model.IsTerminal())
            {
                heuristicList.Clear();
                executableActions.Clear();
                heuristicValue = 0;
                accumulatedHeuristicValue = 0;

                bestValue = -1;
                minValue = float.MaxValue;

                if (actions.Count == 0) break;

                foreach (GOB.Action action in actions)
                {
                    if (action.CanExecute(model))
                    {
                        accumulatedHeuristicValue += Math.Pow(Math.E, action.H(model));
                        executableActions.Add(action);
                    }
                }

                foreach(GOB.Action action in executableActions)
                {
                    heuristicValue = Math.Pow(Math.E, action.H(model)) / accumulatedHeuristicValue;

                    if(!heuristicList.ContainsKey(heuristicValue))
                        heuristicList.Add(heuristicValue, action);

                    if (heuristicValue > bestValue)
                    {
                        bestValue = heuristicValue;
                    }
                    if(heuristicValue < minValue)
                    {
                        minValue = heuristicValue;
                    }
                }
                
                double randomNumber = GetRandomNumber(minValue, bestValue);

                foreach (KeyValuePair<double, GOB.Action> actionHeuristic in heuristicList)
                {
                    if (actionHeuristic.Key >= randomNumber)
                    {
                        nextAction = actionHeuristic.Value;
                        break;
                    }
                }

                if (nextAction == null) break;

                nextAction.ApplyActionEffects(model);
                model.CalculateNextPlayer();
            }

            reward.PlayerID = model.GetNextPlayer();
            reward.Value = model.GetScore();
            return reward;
        }

        private double GetRandomNumber(double minimum, double maximum)
        {
            return RandomGenerator.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
