using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanHeuristic : IHeuristic
    {
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            float distance = Vector3.Distance(node.Position, goalNode.Position);
            return distance;
        }
    }
}
