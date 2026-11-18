using System;
using MatrixLibrary.Core;

namespace MatrixLibrary.Operations
{
    /// <summary>
    /// Содержит операции геометрических и структурных трансформаций матрицы.
    /// </summary>
    public static class MatrixTransformations
    {
        /// <summary>
        /// Выполняет транспонирование матрицы (A^T), меняя строки и столбцы местами.
        /// </summary>
        public static CsrMatrix Transpose(CsrMatrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            // для симметричной матрицы A^T == A, менять нечего
            if (matrix.IsSymmetric)
            {
                return matrix;
            }

            var builder = new SparseMatrixBuilder(matrix.Columns, matrix.Rows, isSymmetric: false);

            for (int i = 0; i < matrix.Rows; i++)
            {
                foreach (var (col, val) in matrix.GetRowElements(i))
                {
                    builder[col, i] = val;
                }
            }

            return builder.Build();
        }

        /// <summary>
        /// Вычисляет след матрицы (Trace) - сумму элементов главной диагонали: sum(A[i, i]).
        /// След определен только для квадратных матриц.
        /// </summary>
        public static double Trace(CsrMatrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            if (matrix.Rows != matrix.Columns)
                throw new InvalidOperationException("След (Trace) можно вычислить только для квадратной матрицы.");

            double sum = 0.0;
            for (int i = 0; i < matrix.Rows; i++)
            {
                sum += matrix[i, i];
            }

            return sum;
        }

        /// <summary>
        /// Извлекает главную диагональ матрицы в виде вектора.
        /// Длина вектора равна min(Rows, Columns).
        /// </summary>
        public static double[] GetDiagonal(CsrMatrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            int minDimension = Math.Min(matrix.Rows, matrix.Columns);
            double[] diagonal = new double[minDimension];

            for (int i = 0; i < minDimension; i++)
            {
                diagonal[i] = matrix[i, i];
            }

            return diagonal;
        }
    }
}