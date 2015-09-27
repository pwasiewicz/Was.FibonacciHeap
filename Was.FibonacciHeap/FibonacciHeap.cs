namespace Was.FibonacciHeap
{
    using System.Collections.Generic;
    using FibonacciHeap;

    public class FibonacciHeap<TValue, TPriority>
    {
        private readonly IComparer<TPriority> comparer;
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
        }

        public HeapNode<TValue, TPriority> Push(TValue value, TPriority priority)
        {
            var newNode = HeapNode< TValue, TPriority>.MakeNew(value, priority);

            this.min = this.Merge(this.min, newNode);
            this.size++;

            return newNode;
        }

        public TValue Min
        {
            get
            {
                if (this.min == null) return default(TValue);

                return this.min.Value;
            }
        }

        public bool IsEmpty()
        {
            return this.min == null;
        }

        public HeapNode<TValue, TPriority> PopMin()
        {
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

            var tree = new List<HeapNode<TValue, TPriority>>();
            var toVisit = new LinkedList<HeapNode<TValue, TPriority>>();

            if (this.min == null)
            {
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
                    while (curr.Degree >= tree.Count)
                    {
                        tree.Add(null);
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

            return minElem;
        }

        public void DecreaseKey(HeapNode<TValue, TPriority> entry, TPriority newPriorty)
        {
            entry.Priority = newPriorty;
            if (entry.Parent != null && this.comparer.Compare(entry.Priority, entry.Parent.Priority) <= 0)
            {
                this.Cut(entry);
            }
            if (this.comparer.Compare(entry.Priority, this.min.Priority) <= 0)
            {
                this.min = entry;
            }
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
                if (entry.Next != entry)
                {
                    entry.Parent.Child = entry.Next;
                }
                else
                {
                    entry.Parent.Child = null;
                }
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

            if (first == null && second != null)
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
