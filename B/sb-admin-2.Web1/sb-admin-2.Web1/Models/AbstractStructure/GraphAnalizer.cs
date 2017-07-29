using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models.AbstractStructure
{
    abstract public class GraphAnalizer
    {
        protected class Node
        {
            public Guid guid { get; private set; }

            public object Data { get; set; }

            public Node()
            {
                guid = Guid.NewGuid();
                Data = null;
            }
        }

        protected List<Node> nodeList;

        protected GraphAnalizer()
        {
            nodeList = new List<Node>();
        }
    }

    public class GraphDirected : GraphAnalizer
    {
        private Dictionary<Node, Node> nodeDict;

        public GraphDirected()
        {
            nodeDict = new Dictionary<Node, Node>();
        }

        public void Add(object data, object dataRef)
        {
            Node node = nodeList.Find(item => item.Data.Equals(data));
            if (node == null)
            {
                node = new Node() { Data = data };
                nodeList.Add(node);
            }

            Node nodeRef = nodeList.Find(item => item.Data.Equals(dataRef));
            if (nodeRef == null)
            {
                nodeRef = new Node() { Data = dataRef };
                nodeList.Add(nodeRef);
            }

            Node nodeTemp = null;
            if (!nodeDict.TryGetValue(node, out nodeTemp))
            {
                nodeDict.Add(node, nodeRef);
            }
        }

        private Node Next(Node node)
        {
            if (node != null)
            {
                Node nodeNext = null;
                if (nodeDict.TryGetValue(node, out nodeNext))
                {
                    return nodeNext;
                }
            }
            return null;
        }

        private bool IsCyclic(Node node)
        {
            Node nodeTemp = node;
            while (nodeTemp != null)
            {
                nodeTemp = Next(nodeTemp);
                if ((nodeTemp != null) && (nodeTemp.guid == node.guid))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCyclic(object data)
        {
            Node node = nodeList.First(item => item.Data.Equals(data));
            return IsCyclic(node);
        }
    }
}