namespace CollectionLibrary.List
{
    public class DoublyLinkedListImpl<T> : LinkedList<T>
    {
        private sealed class Node(T value, Node? previous, Node? next)
        {
            public T Value { get; set; } = value;
            public Node? Prev { get; set; } = previous;
            public Node? Next { get; set; } = next;
        }

        private Node? head, tail;

        public int Size { get; private set; }

        public T this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        private Node GetNode(int index)
        {
            if (index < 0 || index >= Size)
                throw new IndexOutOfRangeException($"Index {index} is out of range.");

            if (index < Size / 2)
            {
                var current = head!;
                for (int i = 0; i < index; i++)
                    current = current.Next!;
                return current;
            }

            var fromTail = tail!;
            for (int i = Size - 1; i > index; i--)
                fromTail = fromTail.Prev!;
            return fromTail;
        }

        private void InsertBetween(Node? previous, Node? next, T value)
        {
            var node = new Node(value, previous, next);
            if (previous == null)
                head = node;
            else
                previous.Next = node;

            if (next == null)
                tail = node;
            else
                next.Prev = node;

            Size++;
        }

        public T Get(int index) => GetNode(index).Value;

        public void Set(int index, T value) => GetNode(index).Value = value;

        public void AddFirst(T value) => InsertBetween(null, head, value);

        public void AddLast(T value) => InsertBetween(tail, null, value);

        public void InsertBefore(int index, T value)
        {
            var next = GetNode(index);
            InsertBetween(next.Prev, next, value);
        }

        public void InsertAfter(int index, T value)
        {
            var previous = GetNode(index);
            InsertBetween(previous, previous.Next, value);
        }

        private void RemoveNode(Node node)
        {
            if (node.Prev == null)
                head = node.Next;
            else
                node.Prev.Next = node.Next;

            if (node.Next == null)
                tail = node.Prev;
            else
                node.Next.Prev = node.Prev;

            Size--;
        }

        public void Remove(int index) => RemoveNode(GetNode(index));

        public void RemoveFirst()
        {
            if (head == null)
                throw new InvalidOperationException("List is empty");

            RemoveNode(head);
        }

        public void Clear()
        {
            head = null;
            tail = null;
            Size = 0;
        }

        public T GetFirst() => head != null ? head.Value : throw new InvalidOperationException("List is empty");

        public T GetLast() => tail != null ? tail.Value : throw new InvalidOperationException("List is empty");

        int LinkedList<T>.Size() => Size;

        public bool isEmpty() => Size == 0;
    }
}
