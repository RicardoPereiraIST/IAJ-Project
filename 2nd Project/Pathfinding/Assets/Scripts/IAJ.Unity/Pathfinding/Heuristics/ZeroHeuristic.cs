using RAIN.Navigation.Graph;
using System;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class ZeroHeuristic : IHeuristic
    {
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            return 0;
        }
    }
}
