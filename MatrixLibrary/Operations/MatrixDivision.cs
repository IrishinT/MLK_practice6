using System;
using MatrixLibrary.Core;
using MatrixLibrary.Exceptions;
using MatrixLibrary.Solvers;

namespace MatrixLibrary.Operations
{
    /// <summary>
    /// Содержит алгоритмы деления матриц через решение систем линейных уравнений.
    /// </summary>
    public static class MatrixDivision
    {
        /// <summary>
        /// Выполняет матричное деление C = A / B (эквивалентно поиску C, такого что C * B = A).
        /// </summary>
        /// <param name="a">Делимое (матрица A).</param>
        /// <param name="b">Делитель (матрица B, обязана быть квадратной и симметричной).</param>
        /// <param name="solver">Решатель СЛАУ (по умолчанию ConjugateGradientSolver).</param>
        public static CsrMatrix Divide(CsrMatrix a, CsrMatrix b, ISlaeSolver solver = null)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            if (b.Rows != b.Columns)
                throw new MatrixDimensionException("Делитель (матрица B) обязан быть квадратной матрицей.");

            if (a.Columns != b.Rows)
                throw new MatrixDimensionException("Деление матриц A / B", a.Rows, a.Columns, b.Rows, b.Columns);

            solver ??= new ConjugateGradientSolver();

            var builder = new SparseMatrixBuilder(a.Rows, b.Columns, isSymmetric: false);

            // для каждой строки A решаем систему B * c_i = a_i
            for (int i = 0; i < a.Rows; i++)
            {
                double[] aRow = new double[a.Columns];
                foreach (var (col, val) in a.GetRowElements(i))
                {
                    aRow[col] = val;
                }

                if (a.IsSymmetric)
                {
                    for (int col = 0; col < i; col++)
                    {
                        aRow[col] = a[col, i];
                    }
                }

                bool isAllZeros = true;
                for (int k = 0; k < aRow.Length; k++)
                {
                    if (aRow[k] != 0.0) { isAllZeros = false; break; }
                }
                if (isAllZeros) continue;

                SolverResult result = solver.Solve(b, aRow);

                double[] cRow = result.Solution;
                for (int j = 0; j < cRow.Length; j++)
                {
                    // отсекаем вычислительный шум вроде 1e-16
                    if (Math.Abs(cRow[j]) > 1e-10)
                    {
                        builder[i, j] = cRow[j];
                    }
                }
            }

            return builder.Build();
        }
    }
}