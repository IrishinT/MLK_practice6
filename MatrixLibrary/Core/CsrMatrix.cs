using System;
using System.Collections.Generic;
using MatrixLibrary.Interfaces;
using MatrixLibrary.Operations;
// using MatrixLibrary.Operations; // Раскомментируем на следующем шаге

namespace MatrixLibrary.Core
{
    /// <summary>
    /// Разреженная матрица в формате Compressed Sparse Row (CSR).
    /// Класс является неизменяемым (Immutable) после создания.
    /// </summary>
    public class CsrMatrix : ISparseMatrix
    {
        // Внутренние массивы формата CSR. 
        // internal позволяет классам математики (Operations) читать их напрямую для максимальной скорости.
        internal readonly int[] RowPointers;
        internal readonly int[] ColIndices;
        internal readonly double[] Values;

        public int Rows { get; }
        public int Columns { get; }
        public bool IsSymmetric { get; }
        public int NonZerosCount => Values.Length;

        /// <summary>
        /// Конструктор доступен только внутри сборки (internal). 
        /// Создавать матрицу должен класс SparseMatrixBuilder.
        /// </summary>
        internal CsrMatrix(int rows, int cols, bool isSymmetric,
                           int[] rowPointers, int[] colIndices, double[] values)
        {
            Rows = rows;
            Columns = cols;
            IsSymmetric = isSymmetric;
            RowPointers = rowPointers;
            ColIndices = colIndices;
            Values = values;
        }

        public double this[int row, int col]
        {
            get
            {
                CheckBounds(row, col);

                // Если матрица симметричная, мы храним только верхний треугольник (где col >= row).
                // При запросе нижнего треугольника - зеркально меняем индексы.
                if (IsSymmetric && row > col)
                {
                    (row, col) = (col, row);
                }

                int lo = RowPointers[row];
                int hi = RowPointers[row + 1];

                int index = BinarySearch.IndexOf(ColIndices, lo, hi, col);
                return index < 0 ? 0.0 : Values[index];
            }
        }

        public IEnumerable<(int Column, double Value)> GetRowElements(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows)
                throw new IndexOutOfRangeException("Индекс строки вне границ матрицы.");

            int start = RowPointers[rowIndex];
            int end = RowPointers[rowIndex + 1];

            for (int k = start; k < end; k++)
            {
                yield return (ColIndices[k], Values[k]);
            }
        }

        private void CheckBounds(int row, int col)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Columns)
                throw new IndexOutOfRangeException($"Индекс [{row}, {col}] находится вне границ матрицы {Rows}x{Columns}.");
        }

        public static CsrMatrix operator +(CsrMatrix a, CsrMatrix b) => MatrixAddition.Add(a, b);
        public static CsrMatrix operator -(CsrMatrix a, CsrMatrix b) => MatrixAddition.Subtract(a, b);

        // Умножение матрицы на матрицу
        public static CsrMatrix operator *(CsrMatrix a, CsrMatrix b) => MatrixMultiplication.Multiply(a, b);

        // Умножение матрицы на вектор
        public static double[] operator *(CsrMatrix a, double[] vector) => MatrixMultiplication.Multiply(a, vector);

        // Умножение и деление на скаляр (число)
        public static CsrMatrix operator *(CsrMatrix a, double scalar) => MatrixScalarMath.Multiply(a, scalar);
        public static CsrMatrix operator *(double scalar, CsrMatrix a) => MatrixScalarMath.Multiply(a, scalar);
        public static CsrMatrix operator /(CsrMatrix a, double scalar) => MatrixScalarMath.Divide(a, scalar);

        // Деление матрицы на матрицу: C = A / B
        public static CsrMatrix operator /(CsrMatrix a, CsrMatrix b) => MatrixDivision.Divide(a, b);

        public CsrMatrix Transpose() => MatrixTransformations.Transpose(this);
        public double Trace() => MatrixTransformations.Trace(this);

    }
}