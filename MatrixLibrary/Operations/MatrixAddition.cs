using System;
using System.Collections.Generic;
using MatrixLibrary.Core;
using MatrixLibrary.Exceptions;

namespace MatrixLibrary.Operations
{
    /// <summary>
    /// Класс для выполнения операций сложения и вычитания матриц.
    /// </summary>
    internal static class MatrixAddition
    {
        public static CsrMatrix Add(CsrMatrix a, CsrMatrix b)
        {
            return AddOrSubtract(a, b, isSubtraction: false);
        }

        public static CsrMatrix Subtract(CsrMatrix a, CsrMatrix b)
        {
            return AddOrSubtract(a, b, isSubtraction: true);
        }

        private static CsrMatrix AddOrSubtract(CsrMatrix a, CsrMatrix b, bool isSubtraction)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            if (a.Rows != b.Rows || a.Columns != b.Columns)
            {
                string op = isSubtraction ? "Вычитание" : "Сложение";
                throw new MatrixDimensionException(op, a.Rows, a.Columns, b.Rows, b.Columns);
            }

            // разная симметрия - проще собрать через Builder, чем городить частные случаи в CSR
            if (a.IsSymmetric != b.IsSymmetric)
            {
                return AddViaBuilder(a, b, isSubtraction);
            }

            // одинаковая симметрия - сливаем строки как два отсортированных списка
            bool resultIsSymmetric = a.IsSymmetric;

            List<int> resultCols = new List<int>();
            List<double> resultValues = new List<double>();
            int[] resultRowPointers = new int[a.Rows + 1];

            for (int i = 0; i < a.Rows; i++)
            {
                resultRowPointers[i] = resultCols.Count;

                int ptrA = a.RowPointers[i];
                int endA = a.RowPointers[i + 1];

                int ptrB = b.RowPointers[i];
                int endB = b.RowPointers[i + 1];

                while (ptrA < endA && ptrB < endB)
                {
                    int colA = a.ColIndices[ptrA];
                    int colB = b.ColIndices[ptrB];

                    if (colA < colB)
                    {
                        resultCols.Add(colA);
                        resultValues.Add(a.Values[ptrA]);
                        ptrA++;
                    }
                    else if (colB < colA)
                    {
                        resultCols.Add(colB);
                        double valB = isSubtraction ? -b.Values[ptrB] : b.Values[ptrB];
                        resultValues.Add(valB);
                        ptrB++;
                    }
                    else
                    {
                        double valB = isSubtraction ? -b.Values[ptrB] : b.Values[ptrB];
                        double sum = a.Values[ptrA] + valB;

                        if (sum != 0.0)
                        {
                            resultCols.Add(colA);
                            resultValues.Add(sum);
                        }
                        ptrA++;
                        ptrB++;
                    }
                }

                while (ptrA < endA)
                {
                    resultCols.Add(a.ColIndices[ptrA]);
                    resultValues.Add(a.Values[ptrA]);
                    ptrA++;
                }

                while (ptrB < endB)
                {
                    resultCols.Add(b.ColIndices[ptrB]);
                    double valB = isSubtraction ? -b.Values[ptrB] : b.Values[ptrB];
                    resultValues.Add(valB);
                    ptrB++;
                }
            }

            resultRowPointers[a.Rows] = resultCols.Count;

            return new CsrMatrix(
                a.Rows,
                a.Columns,
                resultIsSymmetric,
                resultRowPointers,
                resultCols.ToArray(),
                resultValues.ToArray()
            );
        }

        private static CsrMatrix AddViaBuilder(CsrMatrix a, CsrMatrix b, bool isSubtraction)
        {
            var builder = new SparseMatrixBuilder(a.Rows, a.Columns, isSymmetric: false);

            for (int i = 0; i < a.Rows; i++)
            {
                foreach (var (col, val) in a.GetRowElements(i))
                {
                    builder[i, col] += val;
                    if (a.IsSymmetric && i != col)
                        builder[col, i] += val;
                }
            }

            for (int i = 0; i < b.Rows; i++)
            {
                foreach (var (col, val) in b.GetRowElements(i))
                {
                    double v = isSubtraction ? -val : val;
                    builder[i, col] += v;
                    if (b.IsSymmetric && i != col)
                        builder[col, i] += v;
                }
            }

            return builder.Build();
        }
    }
}