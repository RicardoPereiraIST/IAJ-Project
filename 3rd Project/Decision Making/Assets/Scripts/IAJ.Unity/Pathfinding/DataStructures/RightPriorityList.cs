using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class RightPriorityList : IOpenSet, IComparer<NodeRecord>
    {
        private List<NodeRecord> Open { get; set; }

        public RightPriorityList()
        {
            this.Open = new List<NodeRecord>();    
        }
        public void Initialize()
        {
            //TODO implement
            this.Open.Clear();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            //TODO implement
            RemoveFromOpen(nodeToBeReplaced);
            AddToOpen(nodeToReplace);
        }

        public NodeRecord GetBestAndRemove()
        {
            NodeRecord best = Open[Open.Count - 1];
            Open.RemoveAt(Open.Count - 1);
            return best;
        }

        public NodeRecord PeekBest()
        {
            return Open[Open.Count - 1];
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            //a little help here, notice the difference between this method and the one for the LeftPriority list
            //...this one uses a different comparer with an explicit compare function (which you will have to define below)
            int index = this.Open.BinarySearch(nodeRecord,this);
            if (index < 0)
            {
                this.Open.Insert(~index, nodeRecord);
            }
            else this.Open.Insert(index, nodeRecord);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            int index = this.Open.BinarySearch(nodeRecord, this);
            if (index != -1)
            {
                Open.RemoveAt(index);
            }
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            return this.Open.FirstOrDefault(n => n.Equals(nodeRecord));
        }

        public ICollection<NodeRecord> All()
        {
            //TODO implement
            return Open;
        }

        public int CountOpen()
        {
            //TODO implement
            return Open.Count;
        }

        public int Compare(NodeRecord x, NodeRecord y)
        {
            return y.CompareTo(x);
        }
    }
}
