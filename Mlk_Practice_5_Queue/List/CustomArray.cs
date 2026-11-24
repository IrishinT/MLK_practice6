namespace CollectionLibrary.List
{
    public class CustomArray<T>
    {
        private readonly T[] items;

        public CustomArray(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            items = new T[length];
        }

        public int Length => items.Length;

        public T this[int index]
        {
            get
            {
                ValidateIndex(index);
                return items[index];
            }
            set
            {
                ValidateIndex(index);
                items[index] = value;
            }
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException($"Index {index} is out of range.");
        }
    }
}
