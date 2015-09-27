namespace Was.FibonacciHeap
{
    using System.Collections;
    using System.Linq;
    using System;
    using System.Collections.Generic;

    public interface IFibonacciHeap<TValue, TPriority> : IEnumerable<TValue>
    {
        TValue Min { get; }

        HeapNode<TValue, TPriority> Push(TValue value, TPriority priority);
        HeapNode<TValue, TPriority> Pop();

        void DecreaseKey(HeapNode<TValue, TPriority> entry, TPriority newPriorty);

        bool IsEmpty();
    }

    public class FibonacciHeap<TValue, TPriority> : IFibonacciHeap<TValue, TPriority>
    {
        private readonly IComparer<TPriority> comparer;
        private readonly HashSet<HeapNode<TValue, TPriority>> nodes;

        private HeapNode<TValue, TPriority> min;
        private int size;

        public FibonacciHeap()
            : this(Comparer<TPriority>.Default)
        {
        }

        public FibonacciHeap(IComparer<TPriority> comparer)
        {
            this.min = null;
            this.size = 0;

            this.comparer = comparer;
            this.nodes = new HashSet<HeapNode<TValue, TPriority>>();
        }

        public HeapNode<TValue, TPriority> Push(TValue value, TPriority priority)
        {
            var newNode = HeapNode< TValue, TPriority>.MakeNew(value, priority);

            this.min = this.Merge(this.min, newNode);
            this.size++;

            this.nodes.Add(newNode);

            return newNode;
        }

        public TValue Min
        {
            get
            {
                if (this.min == null) throw new EmptyHeapException();

                return this.min.Value;
            }
        }

        public bool IsEmpty()
        {
            return this.size == 0;
        }

        public HeapNode<TValue, TPriority> Pop()
        {
            if (this.IsEmpty()) throw new EmptyHeapException();
            this.size--;

            var minElem = this.min;
            
            if (this.min.Next == this.min)
            {
                this.min = null;
            }
            else
            {
                this.min.Prev.Next = this.min.Next;
                this.min.Next.Prev = this.min.Prev;
                this.min = this.min.Next;
            }

            if (minElem.Child != null)
            {
                var current = minElem.Child;
                do
                {
                    current.Parent = null;
                    current = current.Next;
                } while (current != minElem.Child);
            }

            this.min = this.Merge(this.min, minElem.Child);

            var tree = new Dictionary<int, HeapNode<TValue, TPriority>>();
            var toVisit = new LinkedList<HeapNode<TValue, TPriority>>();

            if (this.min == null)
            {
                this.nodes.Remove(minElem);
                return minElem;
            }
            

            for (var current = this.min;
                    toVisit.Count == 0 || toVisit.First.Value != current; 
                    current = current.Next)
            {
                toVisit.AddLast(current);
            }

            foreach (var globalCurr in toVisit)
            {
                var curr = globalCurr;
                while (true)
                {
                    if (!tree.ContainsKey(curr.Degree))
                    {
                        tree.Add(curr.Degree, null);
                    }

                    if (tree[curr.Degree] == null)
                    {
                        tree[curr.Degree] = curr;
                        break;
                    }

                    var other = tree[curr.Degree];
                    tree[curr.Degree] = null;

                    var min = this.comparer.Compare(other.Priority, curr.Priority) < 0 ? other : curr;
                    var max = this.comparer.Compare(other.Priority, curr.Priority) < 0 ? curr : other;

                    max.Next.Prev = max.Prev;
                    max.Prev.Next = max.Next;

                    max.Next = max.Prev = max;
                    min.Child = this.Merge(min.Child, max);
                    max.Parent = min;

                    max.Marked = false;
                    ++min.Degree;
                    curr = min;
                }

                if (this.comparer.Compare(curr.Priority, this.min.Priority) <= 0)
                {
                    this.min = curr;
                }
            }

            this.nodes.Remove(minElem);
            return minElem;
        }

        public void DecreaseKey(HeapNode<TValue, TPriority> entry, TPriority newPriorty)
        {
            if (!this.nodes.Contains(entry)) throw new ArgumentException("Node belongs to other heap", nameof(entry));

            entry.Priority = newPriorty;
            if (entry.Parent != null
                    && this.comparer.Compare(entry.Priority, entry.Parent.Priority) <= 0)
            {
                this.Cut(entry);
            }
            if (this.comparer.Compare(entry.Priority, this.min.Priority) <= 0)
            {
                this.min = entry;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return this.nodes.Select(n => n.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Cut(HeapNode<TValue, TPriority> entry)
        {
            entry.Marked = false;
            if (entry.Parent == null)
            {
                return;
            }

            if (entry.Next != entry)
            {
                entry.Next.Prev = entry.Prev;
                entry.Prev.Next = entry.Next;
            }

            if (entry.Parent.Child == entry)
            {
                entry.Parent.Child = entry.Next != entry ? entry.Next : null;
            }

            entry.Parent.Degree--;
            entry.Prev = entry.Next = entry;
            this.min = this.Merge(this.min, entry);

            if (entry.Parent.Marked)
                this.Cut(entry.Parent);
            else
                entry.Parent.Marked = true;

            entry.Parent = null;
        }

        private HeapNode<TValue, TPriority> Merge(
                            HeapNode<TValue, TPriority> first,
                            HeapNode<TValue, TPriority> second)
        {

            if (first == null && second == null)
            {
                return null;
            }

            if (first != null && second == null)
            {
                return first;
            }

            if (first == null)
            {
                return second;
            }

            var firstNext = first.Next;
            first.Next = second.Next;
            first.Next.Prev = first;
            second.Next = firstNext;
            second.Next.Prev = second;
            return this.comparer.Compare(first.Priority, second.Priority) < 0 
                                      ? first 
                                      : second;
        }
    }
}
