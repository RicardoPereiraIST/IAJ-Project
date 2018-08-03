using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using RAIN.Navigation.NavMesh;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.GoalBounding
{
    public class GoalBoundingPathfinding : NodeArrayAStarPathFinding
    {
        public GoalBoundingTable GoalBoundingTable { get; protected set;}

        public GoalBoundingPathfinding(NavMeshPathGraph graph, IHeuristic heuristic, GoalBoundingTable goalBoundsTable) : base(graph, heuristic)
        {
            this.GoalBoundingTable = goalBoundsTable;
        }

        public override void InitializePathfindingSearch(Vector3 startPosition, Vector3 goalPosition)
        {
            this.DiscardedEdges = 0;
			this.TotalEdges = 0;
            base.InitializePathfindingSearch(startPosition, goalPosition);
        }

        protected override void ProcessChildNode(NodeRecord parentNode, NavigationGraphEdge connectionEdge, int connectionIndex)
        {
            TotalEdges++;
            NodeGoalBounds goalBound = GoalBoundingTable.table[parentNode.node.NodeIndex];

            if (goalBound != null && connectionIndex < goalBound.connectionBounds.Length)
            {
                DataStructures.GoalBounding.Bounds bound = goalBound.connectionBounds[connectionIndex];
                if (bound.PositionInsideBounds(GoalNode.Position))
                {
                    base.ProcessChildNode(parentNode, connectionEdge, connectionIndex);
                    return;
                }
            }
            else
            {
                base.ProcessChildNode(parentNode, connectionEdge, connectionIndex);
                return;
            }

            DiscardedEdges++;
            return;
        }
    }
}
