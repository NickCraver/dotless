﻿namespace dotless.Core.Parser.Infrastructure.Nodes
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
using System;

    public class NodeList : NodeList<Node>
    {
        public NodeList()
        {
            Inner = new List<Node>();
        }

        public NodeList(params Node[] nodes)
            : this((IEnumerable<Node>) nodes)
        {
        }

        public NodeList(IEnumerable<Node> nodes)
        {
            Inner = new List<Node>(nodes);
        }

        public override Node Copy()
        {
            return new NodeList(Inner.Select(s => s.Copy()));
        }
    }

    public class NodeList<TNode> : Node, IList<TNode>
        where TNode : Node
    {
        protected List<TNode> Inner;

        public NodeList()
        {
            Inner = new List<TNode>();
        }

        public NodeList(params TNode[] nodes)
            : this((IEnumerable<TNode>) nodes)
        {
        }

        public NodeList(IEnumerable<TNode> nodes)
        {
            Inner = new List<TNode>(nodes);
        }

        public void AddRange(IEnumerable<TNode> nodes)
        {
            Inner.AddRange(nodes);
        }

        public IEnumerator<TNode> GetEnumerator()
        {
            return Inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TNode item)
        {
            Inner.Add(item);
        }

        public void Clear()
        {
            Inner.Clear();
        }

        public bool Contains(TNode item)
        {
            return Inner.Contains(item);
        }

        public void CopyTo(TNode[] array, int arrayIndex)
        {
            Inner.CopyTo(array, arrayIndex);
        }

        public bool Remove(TNode item)
        {
            return Inner.Remove(item);
        }

        public int Count
        {
            get { return Inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IList) Inner).IsReadOnly; }
        }

        public int IndexOf(TNode item)
        {
            return Inner.IndexOf(item);
        }

        public void Insert(int index, TNode item)
        {
            Inner.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Inner.RemoveAt(index);
        }

        public TNode this[int index]
        {
            get { return Inner[index]; }
            set { Inner[index] = value; }
        }

        public override Node Copy()
        {
            return new NodeList<TNode>(Inner.Select(i => i));
        }

        public bool Any(Func<TNode, bool> del)
        {
            var c = Inner.Count;

            for (var i = 0; i < c; i++)
            {
                if (del(Inner[i])) return true;
            }

            return false;
        }
    }
}