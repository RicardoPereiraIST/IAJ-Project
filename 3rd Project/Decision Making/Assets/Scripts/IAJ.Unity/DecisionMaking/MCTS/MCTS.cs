using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }


        private int CurrentIterations { get; set; }
        private int CurrentIterationsInFrame { get; set; }
        private int CurrentDepth { get; set; }

        private CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        private MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }        

        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 100;
            this.MaxIterationsProcessedPerFrame = 10;
            this.RandomGenerator = new System.Random(Guid.NewGuid().GetHashCode());
        }


        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.CurrentIterationsInFrame = 0;
            this.TotalProcessingTime = 0.0f;
            this.CurrentStateWorldModel.Initialize();
            this.InitialNode = new MCTSNode(this.CurrentStateWorldModel)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<GOB.Action>();
        }

        public GOB.Action Run()
        {
            Reward reward;
            this.CurrentIterationsInFrame = 0;
            MCTSNode initialNode = this.InitialNode, selectedNode;

            while(CurrentIterations < MaxIterations && CurrentIterationsInFrame < MaxIterationsProcessedPerFrame)
            {
                selectedNode = Selection(initialNode);
                reward = Playout(selectedNode.State);
                Backpropagate(selectedNode, reward);
                CurrentIterations++;
                CurrentIterationsInFrame++;
            }

            if (CurrentIterations >= MaxIterations)
            {
                InProgress = false;
            }

            TotalProcessingTime += Time.deltaTime;
            BestActionSequence.Clear();
            BestFirstChild = BestChild(initialNode);

            if (BestFirstChild == null) return null;

            MCTSNode child = BestFirstChild;
            
            while (child != null)
            {
                BestActionSequence.Add(child.Action);
                child = BestChild(child);
            }

            return BestFirstChild.Action;
        }

        private MCTSNode Selection(MCTSNode initialNode)
        {
            GOB.Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild;

            while (!currentNode.State.IsTerminal())
            {
                nextAction = currentNode.State.GetNextAction();

                if (nextAction != null)
                {
                    return Expand(currentNode, nextAction);
                }
                else
                {
                    bestChild = BestUCTChild(currentNode);
                    currentNode = bestChild;
                }
            }

            return currentNode;
        }

        protected virtual Reward Playout(WorldModel initialPlayoutState)
        {
            GOB.Action action;
            WorldModel model = initialPlayoutState.GenerateChildWorldModel();
            GOB.Action[] actions;
            Reward reward = new Reward();

            while (!model.IsTerminal())
            {
                actions = model.GetExecutableActions();

                if (actions.Length == 0) break;

                action = actions[RandomGenerator.Next(0, actions.Length)];
                action.ApplyActionEffects(model);
                model.CalculateNextPlayer();
            }

            reward.PlayerID = model.GetNextPlayer();
            reward.Value = model.GetScore();
            return reward;
        }

        private void Backpropagate(MCTSNode node, Reward reward)
        {
            while(node != null)
            {
                node.N++;
                node.Q += reward.Value;
                node = node.Parent;
            }
        }

        private MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
            WorldModel state = parent.State.GenerateChildWorldModel();
            MCTSNode child = new MCTSNode(state);
            child.Parent = parent;
            action.ApplyActionEffects(state);
            child.State.CalculateNextPlayer();
            child.Action = action;
            parent.ChildNodes.Add(child);

            return child;
        }

        //gets the best child of a node, using the UCT formula
        private MCTSNode BestUCTChild(MCTSNode node)
        {
            return AbstractBestChild(node, 0);
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        private MCTSNode BestChild(MCTSNode node)
        {
            return AbstractBestChild(node, 1);
        }

        private MCTSNode AbstractBestChild(MCTSNode node, int mode)
        {
            List<MCTSNode> children = node.ChildNodes;
            float ui;
            double uct, bestValue = -1;
            MCTSNode bestNode = null;

            foreach (MCTSNode child in children)
            {
                ui = child.Q / child.N;
                if (mode == 0)
                    uct = ui + C * Math.Sqrt(Math.Log(child.Parent.N) / child.N);
                else
                    uct = ui;
                if (uct > bestValue)
                {
                    bestValue = uct;
                    bestNode = child;
                }
            }

            return bestNode;
        }
    }
}
