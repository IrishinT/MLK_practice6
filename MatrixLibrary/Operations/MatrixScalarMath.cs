using System;
using MatrixLibrary.Core;

namespace MatrixLibrary.Operations
{
    /// <summary>
    /// Содержит алгоритмы для математических операций между матрицей и скаляром (числом).
    /// </summary>
    internal static class MatrixScalarMath
    {
        /// <summary>
        /// Умножает матрицу на скаляр. Возвращает новую матрицу.
        /// </summary>
        public static CsrMatrix Multiply(CsrMatrix matrix, double scalar)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            if (scalar == 0.0)
            {
                return new CsrMatrix(
                    matrix.Rows,
                    matrix.Columns,
                    matrix.IsSymmetric,
                    new int[matrix.Rows + 1],
                    Array.Empty<int>(),
                    Array.Empty<double>()
                );
            }

            // структура (указатели строк и индексы колонок) не меняется, значения - да
            int[] newRowPointers = matrix.RowPointers;
            int[] newColIndices = matrix.ColIndices;

            int nnz = matrix.NonZerosCount;
            double[] newValues = new double[nnz];

            for (int i = 0; i < nnz; i++)
            {
                newValues[i] = matrix.Values[i] * scalar;
            }

            return new CsrMatrix(
                matrix.Rows,
                matrix.Columns,
                matrix.IsSymmetric,
                newRowPointers,
                newColIndices,
                newValues
            );
        }

        /// <summary>
        /// Делит матрицу на скаляр. Возвращает новую матрицу.
        /// </summary>
        public static CsrMatrix Divide(CsrMatrix matrix, double scalar)
        {
            if (scalar == 0.0)
                throw new DivideByZeroException("Деление матрицы на ноль невозможно.");

            return Multiply(matrix, 1.0 / scalar);
        }
    }
}