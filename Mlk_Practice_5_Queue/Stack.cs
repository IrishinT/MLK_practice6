namespace Mlk_practice_stack
{

    internal class StackNode<T>
    {
        public T Value { get; set; }
        public StackNode<T> Prev { get; set; }

        public StackNode(T value, StackNode<T> prev)
        {
            Value = value;
            Prev = prev;
        }
    }

    public interface IStack<T>
    {
        public void Push(T value);

        public T Pop();

        public T Peek();

        public bool HasNext();
    }


    public class LinkedStack<T> : IStack<T>
    {

        private StackNode<T> top;

        public LinkedStack()
        {
            top = null;

        }

        public void Push(T value)
        {
            top = new StackNode<T>(value, top);
        }

        public bool HasNext()
        {
            return top != null;
        }

        public T Pop()
        {
            if (top == null)
                throw new IndexOutOfRangeException("Stack is empty");

            var temp = top;
            top = temp.Prev;

            return temp.Value;
        }

        public T Peek()
        {
            if (top == null)
                return default;

            return top.Value;
        }

    }


    public class ArrayStack<T> : IStack<T>
    {

        private int _capacity;
        private int pointer;
        private T[] elements;

        public ArrayStack(int capacity = 16)
        {
            _capacity = capacity;
            elements = new T[capacity];
            pointer = -1;
        }

        public void Push(T value)
        {
            if ((pointer + 1) >= _capacity)
            {
                Grow();
            }

            elements[++pointer] = value;
        }

        private void Grow()
        {
            int newCapacity = _capacity * 2;
            T[] newArray = new T[newCapacity];

            for (int i = 0; i <= pointer; i++)
            {
                newArray[i] = elements[i];
            }

            elements = newArray;
            _capacity = newCapacity;
        }

        public T Pop()
        {
            if (pointer < 0)
                throw new IndexOutOfRangeException("Stack is empty");

            var res = elements[pointer];

            elements[pointer--] = default;

            return res;
        }

        public T Peek()
        {
            return elements[pointer];
        }

        public bool HasNext()
        {
            return pointer >= 0;
        }

    }

}
