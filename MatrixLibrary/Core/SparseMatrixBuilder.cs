using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixLibrary.Core
{
    /// <summary>
    /// Строитель для удобного и пошагового создания разреженной матрицы (CsrMatrix).
    /// Использует промежуточный формат DOK (Dictionary of Keys).
    /// </summary>
    public class SparseMatrixBuilder
    {
        private readonly int Rows;
        private readonly int Cols;
        private readonly bool IsSymmetric;

        // Словарь хранит данные. Ключ - кортеж (Строка, Столбец), Значение - число.
        private readonly Dictionary<(int, int), double> Data;

        public SparseMatrixBuilder(int rows, int cols, bool isSymmetric = false)
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Размеры матрицы должны быть больше нуля.");

            if (isSymmetric && rows != cols)
                throw new ArgumentException("Симметричная матрица обязана быть квадратной.");

            Rows = rows;
            Cols = cols;
            IsSymmetric = isSymmetric;
            Data = new Dictionary<(int, int), double>();
        }

        /// <summary>
        /// Индексатор для удобного добавления элементов: builder[i, j] = 5.0;
        /// </summary>
        public double this[int row, int col]
        {
            get
            {
                CheckBounds(row, col);

                if (IsSymmetric && row > col)
                    (row, col) = (col, row);

                return Data.TryGetValue((row, col), out double val) ? val : 0.0;
            }
            set
            {
                CheckBounds(row, col);

                // Если матрица симметричная, принудительно сохраняем только верхний треугольник.
                if (IsSymmetric && row > col)
                {
                    (row, col) = (col, row);
                }

                if (value == 0.0)
                {
                    // Не храним нули. Если ноль присваивают существующему элементу - удаляем его.
                    Data.Remove((row, col));
                }
                else
                {
                    Data[(row, col)] = value;
                }
            }
        }

        /// <summary>
        /// Конвертирует накопленные данные в высокопроизводительный формат CSR.
        /// </summary>
        /// <returns>Неизменяемый объект CsrMatrix.</returns>
        public CsrMatrix Build()
        {
            int nnz = Data.Count;

            // Выделяем массивы точного размера
            int[] rowPointers = new int[Rows + 1];
            int[] colIndices = new int[nnz];
            double[] values = new double[nnz];

            // Сортируем ключи сначала по строкам, затем по столбцам.
            // Это необходимое условие для корректного CSR формата.
            var sortedEntries = Data
                .OrderBy(kvp => kvp.Key.Item1)
                .ThenBy(kvp => kvp.Key.Item2)
                .ToList();

            int currentNnz = 0;
            int currentRow = 0;

            rowPointers[0] = 0;

            foreach (var entry in sortedEntries)
            {
                int row = entry.Key.Item1;
                int col = entry.Key.Item2;
                double val = entry.Value;

                // Если мы перешли на новую строку (или перепрыгнули пустые строки)
                while (currentRow < row)
                {
                    currentRow++;
                    rowPointers[currentRow] = currentNnz;
                }

                colIndices[currentNnz] = col;
                values[currentNnz] = val;
                currentNnz++;
            }

            // Заполняем оставшиеся пустые строки (если они есть в конце матрицы)
            while (currentRow < Rows)
            {
                currentRow++;
                rowPointers[currentRow] = nnz;
            }

            // Вызываем internal-конструктор CsrMatrix
            return new CsrMatrix(Rows, Cols, IsSymmetric, rowPointers, colIndices, values);
        }

        private void CheckBounds(int row, int col)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                throw new IndexOutOfRangeException($"Индекс [{row}, {col}] находится вне границ матрицы {Rows}x{Cols}.");
        }
    }
}