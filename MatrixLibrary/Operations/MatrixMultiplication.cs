using System;
using System.Collections.Generic;
using MatrixLibrary.Core;
using MatrixLibrary.Exceptions;

namespace MatrixLibrary.Operations
{
    /// <summary>
    /// Содержит высокопроизводительные алгоритмы умножения разреженных матриц.
    /// </summary>
    internal static class MatrixMultiplication
    {
        /// <summary>
        /// Умножение матрицы на вектор (SpMV - Sparse Matrix-Vector multiplication).
        /// Самая часто вызываемая функция в итерационных решателях.
        /// </summary>
        public static double[] Multiply(CsrMatrix matrix, double[] vector)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            if (vector == null) throw new ArgumentNullException(nameof(vector));

            if (matrix.Columns != vector.Length)
                throw new MatrixDimensionException($"Умножение на вектор", matrix.Rows, matrix.Columns, vector.Length, 1);

            double[] result = new double[matrix.Rows];

            for (int i = 0; i < matrix.Rows; i++)
            {
                int start = matrix.RowPointers[i];
                int end = matrix.RowPointers[i + 1];

                for (int p = start; p < end; p++)
                {
                    int j = matrix.ColIndices[p];
                    double val = matrix.Values[p];

                    result[i] += val * vector[j];

                    // для симметричной матрицы хранится только верхний треугольник,
                    // так что A[i, j] подразумевает и A[j, i]
                    if (matrix.IsSymmetric && i != j)
                    {
                        result[j] += val * vector[i];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Умножение разреженной матрицы на разреженную (SpGEMM).
        /// Возвращает новую матрицу.
        /// </summary>
        public static CsrMatrix Multiply(CsrMatrix a, CsrMatrix b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            if (a.Columns != b.Rows)
                throw new MatrixDimensionException("Умножение матриц", a.Rows, a.Columns, b.Rows, b.Columns);

            // результат A*B почти никогда не симметричен, даже если A и B симметричны
            var builder = new SparseMatrixBuilder(a.Rows, b.Columns, isSymmetric: false);

            List<(int Col, double Val)>[] rowsA = UnpackFullRows(a);
            List<(int Col, double Val)>[] rowsB = UnpackFullRows(b);

            double[] rowAccumulator = new double[b.Columns];
            List<int> activeColumns = new List<int>();

            for (int i = 0; i < a.Rows; i++)
            {
                activeColumns.Clear();

                foreach (var aElem in rowsA[i])
                {
                    int k = aElem.Col;
                    double valA = aElem.Val;

                    foreach (var bElem in rowsB[k])
                    {
                        int j = bElem.Col;
                        double valB = bElem.Val;

                        if (rowAccumulator[j] == 0.0)
                        {
                            activeColumns.Add(j);
                        }

                        rowAccumulator[j] += valA * valB;
                    }
                }

                foreach (int j in activeColumns)
                {
                    if (rowAccumulator[j] != 0.0)
                    {
                        builder[i, j] = rowAccumulator[j];
                        rowAccumulator[j] = 0.0;
                    }
                }
            }

            return builder.Build();
        }

        /// <summary>
        /// Вспомогательный метод. Возвращает массив, где индекс - это строка, 
        /// а значение - список всех ненулевых элементов этой строки.
        /// Если матрица симметричная, восстанавливает недостающий нижний треугольник.
        /// </summary>
        private static List<(int Col, double Val)>[] UnpackFullRows(CsrMatrix m)
        {
            var rows = new List<(int Col, double Val)>[m.Rows];
            for (int i = 0; i < m.Rows; i++)
            {
                rows[i] = new List<(int Col, double Val)>();
            }

            for (int i = 0; i < m.Rows; i++)
            {
                int start = m.RowPointers[i];
                int end = m.RowPointers[i + 1];

                for (int p = start; p < end; p++)
                {
                    int j = m.ColIndices[p];
                    double val = m.Values[p];

                    rows[i].Add((j, val));

                    if (m.IsSymmetric && i != j)
                    {
                        rows[j].Add((i, val));
                    }
                }
            }

            return rows;
        }
    }
}