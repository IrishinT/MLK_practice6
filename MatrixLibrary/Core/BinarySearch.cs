using System;

namespace MatrixLibrary.Core
{
    /// <summary>
    /// Вспомогательный класс для быстрого поиска элементов в отсортированных массивах.
    /// </summary>
    internal static class BinarySearch
    {
        /// <summary>
        /// Ищет значение в заданном диапазоне отсортированного массива.
        /// </summary>
        /// <param name="array">Массив для поиска (индексы столбцов).</param>
        /// <param name="lo">Начальный индекс (включительно).</param>
        /// <param name="hi">Конечный индекс (исключительно).</param>
        /// <param name="value">Искомое значение столбца.</param>
        /// <returns>Индекс найденного элемента или -1, если элемент не найден.</returns>
        public static int IndexOf(int[] array, int lo, int hi, int value)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            while (lo < hi)
            {
                int mid = lo + (hi - lo) / 2;
                int c = array[mid];

                if (c == value) return mid;

                if (c < value)
                    lo = mid + 1;
                else
                    hi = mid;
            }
            return -1;
        }
    }
}