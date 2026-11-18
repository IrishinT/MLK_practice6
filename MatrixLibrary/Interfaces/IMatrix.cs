using System;

namespace MatrixLibrary.Interfaces
{
    /// <summary>
    /// Базовый интерфейс для всех типов матриц.
    /// </summary>
    public interface IMatrix
    {
        /// <summary>
        /// Количество строк в матрице.
        /// </summary>
        int Rows { get; }

        /// <summary>
        /// Количество столбцов в матрице.
        /// </summary>
        int Columns { get; }

        /// <summary>
        /// Указывает, является ли матрица симметричной (A[i, j] == A[j, i]).
        /// Если true, матрица обычно хранит только верхний или нижний треугольник данных.
        /// </summary>
        bool IsSymmetric { get; }

        /// <summary>
        /// Получает значение элемента по указанным индексам строки и столбца.
        /// </summary>
        /// <param name="row">Индекс строки (от 0 до Rows - 1).</param>
        /// <param name="col">Индекс столбца (от 0 до Columns - 1).</param>
        /// <returns>Значение элемента в ячейке [row, col].</returns>
        /// <exception cref="IndexOutOfRangeException">Выбрасывается, если индексы выходят за границы матрицы.</exception>
        double this[int row, int col] { get; }
    }
}