using System.Collections.Generic;

namespace MatrixLibrary.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с разреженными матрицами (в которых большинство элементов - нули).
    /// </summary>
    public interface ISparseMatrix : IMatrix
    {
        /// <summary>
        /// Количество физически хранящихся ненулевых элементов в матрице (NNZ).
        /// </summary>
        int NonZerosCount { get; }

        /// <summary>
        /// Возвращает перечисление всех ненулевых элементов в указанной строке.
        /// Это критически важный метод для быстрых математических алгоритмов, 
        /// чтобы не перебирать нули.
        /// </summary>
        /// <param name="rowIndex">Индекс строки.</param>
        /// <returns>Последовательность пар (Индекс_столбца, Значение).</returns>
        IEnumerable<(int Column, double Value)> GetRowElements(int rowIndex);
    }
}