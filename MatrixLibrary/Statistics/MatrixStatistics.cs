using System;
using MatrixLibrary.Core;

namespace MatrixLibrary.Statistics
{
    /// <summary>
    /// Содержит статистические методы анализа числовых характеристик и рассеивания данных матрицы.
    /// </summary>
    public static class MatrixStatistics
    {
        /// <summary>
        /// Вычисляет норму Фробениуса (Frobenius Norm) матрицы: sqrt(sum(A[i, j]^2)).
        /// Служит аналогом длины (евклидовой нормы) для матрицы.
        /// </summary>
        public static double FrobeniusNorm(CsrMatrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            var (_, sumOfSquares) = ComputeSums(matrix);
            return Math.Sqrt(sumOfSquares);
        }

        /// <summary>
        /// Вычисляет среднее арифметическое (Mean) всех элементов матрицы (включая неявно хранящиеся нули).
        /// </summary>
        public static double Mean(CsrMatrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            long totalElements = (long)matrix.Rows * matrix.Columns;
            if (totalElements == 0) return 0.0;

            var (sum, _) = ComputeSums(matrix);
            return sum / totalElements;
        }

        /// <summary>
        /// Вычисляет дисперсию / рассеивание (Variance) элементов матрицы вокруг среднего значения:
        /// Var = (1/N) * sum((A[i, j] - Mean)^2).
        /// Показывает степень разброса весов в матрице.
        /// </summary>
        public static double Variance(CsrMatrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            long totalElements = (long)matrix.Rows * matrix.Columns;
            if (totalElements == 0) return 0.0;

            var (sum, sumOfSquares) = ComputeSums(matrix);
            double mean = sum / totalElements;

            // Var = E[X^2] - (E[X])^2, считаем за один проход по матрице
            double variance = (sumOfSquares / totalElements) - (mean * mean);

            // из-за погрешностей double может выйти -0.000000000001
            return Math.Max(0.0, variance);
        }

        /// <summary>
        /// Вычисляет среднеквадратичное отклонение (Standard Deviation, СКО / сигма).
        /// Квадратный корень из дисперсии. Имеет ту же размерность, что и исходные данные.
        /// </summary>
        public static double StandardDeviation(CsrMatrix matrix)
        {
            return Math.Sqrt(Variance(matrix));
        }

        /// <summary>
        /// Вычисляет среднеквадратичное значение (RMS - Root Mean Square) элементов матрицы:
        /// RMS = sqrt((1/N) * sum(A[i, j]^2)).
        /// </summary>
        public static double RootMeanSquare(CsrMatrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            long totalElements = (long)matrix.Rows * matrix.Columns;
            if (totalElements == 0) return 0.0;

            var (_, sumOfSquares) = ComputeSums(matrix);
            return Math.Sqrt(sumOfSquares / totalElements);
        }

        /// <summary>
        /// Считает сумму элементов и сумму их квадратов за один проход, учитывая симметрию матрицы.
        /// </summary>
        private static (double Sum, double SumOfSquares) ComputeSums(CsrMatrix m)
        {
            double sum = 0.0;
            double sumOfSquares = 0.0;

            for (int i = 0; i < m.Rows; i++)
            {
                int start = m.RowPointers[i];
                int end = m.RowPointers[i + 1];

                for (int p = start; p < end; p++)
                {
                    int j = m.ColIndices[p];
                    double val = m.Values[p];
                    double valSq = val * val;

                    // для симметричной матрицы недиагональный элемент считается дважды - A[i,j] и A[j,i]
                    if (m.IsSymmetric && i != j)
                    {
                        sum += 2.0 * val;
                        sumOfSquares += 2.0 * valSq;
                    }
                    else
                    {
                        sum += val;
                        sumOfSquares += valSq;
                    }
                }
            }

            return (sum, sumOfSquares);
        }
    }
}