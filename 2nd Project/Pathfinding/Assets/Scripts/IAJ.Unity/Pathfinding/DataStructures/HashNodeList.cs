using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    //very simple (and unefficient) implementation of the open/closed sets
    public class HashNodeList : IOpenSet, IClosedSet
    {
        private Dictionary<int, NodeRecord> NodeValues { get; set; }

        public HashNodeList()
        {
            this.NodeValues = new Dictionary<int, NodeRecord>();
        }

        public void Initialize()
        {
            this.NodeValues.Clear();
        }

        public int CountOpen()
        {
            return this.NodeValues.Count;
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            this.NodeValues.Add(nodeRecord.GetHashCode(), nodeRecord);
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            this.NodeValues.Remove(nodeRecord.GetHashCode());
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            //here I cannot use the == comparer because the nodeRecord will likely be a different computational object
            //and therefore pointer comparison will not work, we need to use Equals
            //LINQ with a lambda expression
            int hashCode = nodeRecord.GetHashCode();

            if (NodeValues.ContainsKey(hashCode))
                return NodeValues[hashCode];

            return null;
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            this.NodeValues.Add(nodeRecord.GetHashCode(), nodeRecord);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            this.NodeValues.Remove(nodeRecord.GetHashCode());
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            //here I cannot use the == comparer because the nodeRecord will likely be a different computational object
            //and therefore pointer comparison will not work, we need to use Equals
            //LINQ with a lambda expression
            int hashCode = nodeRecord.GetHashCode();

            if (NodeValues.ContainsKey(hashCode))
                return NodeValues[hashCode];

            return null;
        }

        public ICollection<NodeRecord> All()
        {
            return this.NodeValues.Values;
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            //since the list is not ordered we do not need to remove the node and add the new one, just copy the different values
            //remember that if NodeRecord is a struct, for this to work we need to receive a reference
            nodeToBeReplaced.parent = nodeToReplace.parent;
            nodeToBeReplaced.fValue = nodeToReplace.fValue;
            nodeToBeReplaced.gValue = nodeToReplace.gValue;
            nodeToBeReplaced.hValue = nodeToReplace.hValue;
        }

        public NodeRecord GetBestAndRemove()
        {
            var best = this.PeekBest();
            this.NodeValues.Remove(best.GetHashCode());
            return best;
        }

        public NodeRecord PeekBest()
        {
            //welcome to LINQ guys, for those of you that remember LISP from the AI course, the LINQ Aggregate method is the same as lisp's Reduce method
            //so here I'm just using a lambda that compares the first element with the second and returns the lowest
            //by applying this to the whole list, I'm returning the node with the lowest F value.
            return this.NodeValues.Aggregate((nodeRecord1, nodeRecord2) => nodeRecord1.Value.fValue < nodeRecord2.Value.fValue ? nodeRecord1 : nodeRecord2).Value;
        }
    }
}
