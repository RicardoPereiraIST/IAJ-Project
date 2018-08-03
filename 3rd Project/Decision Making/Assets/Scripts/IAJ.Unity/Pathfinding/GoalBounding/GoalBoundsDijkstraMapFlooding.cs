using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.GoalBounding
{
    //The Dijkstra algorithm is similar to the A* but with a couple of differences
    //1) no heuristic function
    //2) it will not stop until the open list is empty
    //3) we dont need to execute the algorithm in multiple steps (because it will be executed offline)
    //4) we don't need to return any path (partial or complete)
    //5) we don't need to do anything when a node is already in closed
    public class GoalBoundsDijkstraMapFlooding
    {
        protected NodeRecordArray NodeRecordArray { get; set; }

        private NodeRecord startNodeRecord;

        private NodeRecord bestNode;

        private int startNodeIndex;

        public IOpenSet Open { get; protected set; }
        public IClosedSet Closed { get; protected set; }
        
        public GoalBoundsDijkstraMapFlooding(List<NavigationGraphNode> nodes)
        {
            this.NodeRecordArray = new NodeRecordArray(nodes);
            this.Open = this.NodeRecordArray;
            this.Closed = this.NodeRecordArray;
        }

        public void Search(NavigationGraphNode startNode, NodeGoalBounds nodeGoalBounds)
        {

            //TODO: Implement the algorithm that calculates the goal bounds using a dijkstra
            //Given that the nodes in the graph correspond to the edges of a polygon, we won't be able to use the vertices of the polygon to update the bounding boxes
            startNodeRecord = this.NodeRecordArray.GetNodeRecord(startNode);
            startNodeIndex = startNodeRecord.node.NodeIndex;
            startNodeRecord.startNodeIndex = startNodeIndex;

            bool first = true;
            Open.AddToOpen(startNodeRecord);

            while (this.Open.CountOpen() > 0)
            {
                bestNode = this.Open.GetBestAndRemove();

                if(!first) nodeGoalBounds.connectionBounds[bestNode.StartNodeOutConnectionIndex].UpdateBounds(bestNode.node.Position);

                this.Closed.AddToClosed(bestNode);

                for (int i = 0; i < bestNode.node.OutEdgeCount; i++)
                {
                    if(first)
                        this.ProcessChildNode(bestNode, bestNode.node.EdgeOut(i), i, nodeGoalBounds);
                    else this.ProcessChildNode(bestNode, bestNode.node.EdgeOut(i), bestNode.StartNodeOutConnectionIndex, nodeGoalBounds);
                }
                first = false;
            }
        }

       
        protected void ProcessChildNode(NodeRecord bestNode, NavigationGraphEdge connectionEdge, int connectionIndex, NodeGoalBounds nodeGoalBounds)
        {
            //TODO: Implement this method that processes a child node. Then you can use it in the Search method above.
            var childNode = connectionEdge.ToNode;
            NodeRecord childNodeRecord = this.NodeRecordArray.GetNodeRecord(childNode);

            if (childNodeRecord.startNodeIndex != startNodeIndex) childNodeRecord.status = NodeStatus.Unvisited;

            if (childNodeRecord == null)
            {
                //this piece of code is used just because of the special start nodes and goal nodes added to the RAIN Navigation graph when a new search is performed.
                //Since these special goals were not in the original navigation graph, they will not be stored in the NodeRecordArray and we will have to add them
                //to a special structure
                //it's ok if you don't understand this, this is a hack and not part of the NodeArrayA* algorithm, just do NOT CHANGE THIS, or your algorithm will not work
                childNodeRecord = new NodeRecord
                {
                    node = childNode,
                    parent = bestNode,
                    status = NodeStatus.Unvisited
                };
                this.NodeRecordArray.AddSpecialCaseNode(childNodeRecord);
            }

            if (childNodeRecord.status == NodeStatus.Closed) return;

            float g = bestNode.gValue + connectionEdge.Cost;
            float f = g;

            if (childNodeRecord.status == NodeStatus.Unvisited)
            {
                ChangeNodeValues(childNodeRecord, 0, g, f, bestNode, connectionIndex);
                NodeRecordArray.AddToOpen(childNodeRecord);
                return;
            }
            if (childNodeRecord.status == NodeStatus.Open && childNodeRecord.fValue > f)
            {
                ChangeNodeValues(childNodeRecord, 0, g, f, bestNode, connectionIndex);
                this.NodeRecordArray.Replace(childNodeRecord, childNodeRecord);
                return;
            }
        }

        private void ChangeNodeValues(NodeRecord node, float h, float g, float f, NodeRecord parent, int connectionIndex)
        {
            node.gValue = g;
            node.hValue = h;
            node.fValue = f;
            node.parent = parent;
            node.StartNodeOutConnectionIndex = connectionIndex;
            node.startNodeIndex = startNodeIndex;
        }

    }
}
