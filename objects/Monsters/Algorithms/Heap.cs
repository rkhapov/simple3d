using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace objects.Monsters.Algorithms
{
    public abstract class Heap<T> : IEnumerable<T>
    {
        private const int InitialCapacity = 0;
        private const int GrowFactor = 2;
        private const int MinGrow = 1;

        private int capacity = InitialCapacity;
        private T[] heap = new T[InitialCapacity];
        private int tail = 0;

        public int Count => tail;

        public int Capacity => capacity;

        protected IComparer<T> Comparer { get; private set; }
        protected abstract bool Dominates(T x, T y);

        protected Heap() : this(Comparer<T>.Default)
        {
        }

        protected Heap(IComparer<T> comparer) : this(Enumerable.Empty<T>(), comparer)
        {
        }

        protected Heap(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {
        }

        protected Heap(IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            Comparer = comparer;

            foreach (var item in collection)
            {
                if (Count == Capacity)
                    Grow();

                heap[tail++] = item;
            }

            for (var i = Parent(tail - 1); i >= 0; i--)
                BubbleDown(i);
        }

        public void Add(T item)
        {
            if (Count == Capacity)
                Grow();

            heap[tail++] = item;
            BubbleUp(tail - 1);
        }

        private void BubbleUp(int i)
        {
            if (i == 0 || Dominates(heap[Parent(i)], heap[i]))
                return; //correct domination (or root)

            Swap(i, Parent(i));
            BubbleUp(Parent(i));
        }

        public T GetMin()
        {
            if (Count == 0) throw new InvalidOperationException("Heap is empty");
            return heap[0];
        }

        public T ExtractDominating()
        {
            if (Count == 0) throw new InvalidOperationException("Heap is empty");
            var ret = heap[0];
            tail--;
            Swap(tail, 0);
            BubbleDown(0);
            return ret;
        }

        private void BubbleDown(int i)
        {
            var dominatingNode = Dominating(i);
            if (dominatingNode == i) return;
            Swap(i, dominatingNode);
            BubbleDown(dominatingNode);
        }

        private int Dominating(int i)
        {
            var dominatingNode = i;
            dominatingNode = GetDominating(YoungChild(i), dominatingNode);
            dominatingNode = GetDominating(OldChild(i), dominatingNode);

            return dominatingNode;
        }

        private int GetDominating(int newNode, int dominatingNode)
        {
            if (newNode < tail && !Dominates(heap[dominatingNode], heap[newNode]))
                return newNode;
            else
                return dominatingNode;
        }

        private void Swap(int i, int j)
        {
            var tmp = heap[i];
            heap[i] = heap[j];
            heap[j] = tmp;
        }

        private static int Parent(int i)
        {
            return (i + 1) / 2 - 1;
        }

        private static int YoungChild(int i)
        {
            return (i + 1) * 2 - 1;
        }

        private static int OldChild(int i)
        {
            return YoungChild(i) + 1;
        }

        private void Grow()
        {
            var newCapacity = capacity * GrowFactor + MinGrow;
            var newHeap = new T[newCapacity];
            Array.Copy(heap, newHeap, capacity);
            heap = newHeap;
            capacity = newCapacity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return heap.Take(Count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}