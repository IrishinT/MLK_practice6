namespace CollectionLibrary.List
{

    internal class SinglyListNode<T>
    {
        public T Value { get; set; }
        public SinglyListNode<T>? Next { get; set; }

        public SinglyListNode(T value, SinglyListNode<T>? next)
        {
            Value = value;
            Next = next;
        }
    }

    public interface LinkedList<T>
    {
        T GetFirst();

        T GetLast();

        public T this[int index] { get; set; }

        T Get(int index);

        void Set(int index, T value);

        void InsertBefore(int index, T value);

        void InsertAfter(int index, T value);

        void AddLast(T value);

        void AddFirst(T value);

        void Remove(int index);

        void RemoveFirst();

        void Clear();

        int Size();

        bool isEmpty();
    }

    public class SinglyLinkedListImpl<T> : LinkedList<T>
    {
        private SinglyListNode<T>? head, tail;

        public int Size { get; private set; }

        public T this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Size)
                throw new IndexOutOfRangeException($"Index {index} is out of range.");
        }

        private SinglyListNode<T> GetNode(int index)
        {
            ValidateIndex(index);
            var current = head!;
            for (int i = 0; i < index; i++)
                current = current.Next!;
            return current;
        }

        public T Get(int index) => GetNode(index).Value;

        public void Set(int index, T value) => GetNode(index).Value = value;

        public void AddFirst(T value)
        {
            head = new SinglyListNode<T>(value, head);
            tail ??= head;
            Size++;
        }

        public void AddLast(T value)
        {
            var node = new SinglyListNode<T>(value, null);
            if (tail == null)
                head = node;
            else
                tail.Next = node;

            tail = node;
            Size++;
        }

        public void InsertBefore(int index, T value)
        {
            ValidateIndex(index);
            if (index == 0)
                AddFirst(value);
            else
                InsertAfter(index - 1, value);
        }

        public void InsertAfter(int index, T value)
        {
            ValidateIndex(index);
            if (index == Size - 1)
            {
                AddLast(value);
                return;
            }

            var previous = GetNode(index);
            previous.Next = new SinglyListNode<T>(value, previous.Next);
            Size++;
        }

        public void Remove(int index)
        {
            ValidateIndex(index);
            if (index == 0)
            {
                RemoveFirst();
                return;
            }

            var previous = GetNode(index - 1);
            previous.Next = previous.Next!.Next;
            if (previous.Next == null)
                tail = previous;
            Size--;
        }

        public void RemoveFirst()
        {
            if (head == null)
                throw new InvalidOperationException("List is empty");

            head = head.Next;
            if (head == null)
                tail = null;
            Size--;
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
