using System;
using MatrixLibrary.Core;
using MatrixLibrary.Vectors;

namespace MatrixLibrary.Solvers
{
    /// <summary>
    /// Решатель СЛАУ методом сопряженных градиентов (Conjugate Gradient Method).
    /// Идеален для симметричных разреженных матриц (A = A^T).
    /// </summary>
    public class ConjugateGradientSolver : ISlaeSolver
    {
        public SolverResult Solve(CsrMatrix A, double[] b, double tolerance = 1e-10, int maxIterations = 1000, double[] initialGuess = null)
        {
            if (A == null) throw new ArgumentNullException(nameof(A));
            if (b == null) throw new ArgumentNullException(nameof(b));

            if (A.Rows != A.Columns)
                throw new ArgumentException("Матрица должна быть квадратной для решения СЛАУ.");

            if (A.Rows != b.Length)
                throw new ArgumentException($"Размерность матрицы ({A.Rows}) не совпадает с вектором b ({b.Length}).");

            int n = b.Length;

            double[] x = initialGuess != null ? (double[])initialGuess.Clone() : new double[n];

            double[] Ax = A * x;
            double[] r = VectorMath.Subtract(b, Ax);
            double[] p = (double[])r.Clone();

            double rOldDot = VectorMath.Dot(r, r);

            if (Math.Sqrt(rOldDot) < tolerance)
            {
                return new SolverResult(x, 0, Math.Sqrt(rOldDot), true);
            }

            int iteration = 0;
            for (iteration = 0; iteration < maxIterations; iteration++)
            {
                double[] Ap = A * p;
                double pAp = VectorMath.Dot(p, Ap);

                // pAp == 0 обычно значит, что матрица не положительно определена
                if (Math.Abs(pAp) < 1e-15)
                    break;

                double alpha = rOldDot / pAp;

                for (int i = 0; i < n; i++)
                    x[i] += alpha * p[i];

                for (int i = 0; i < n; i++)
                    r[i] -= alpha * Ap[i];

                double rNewDot = VectorMath.Dot(r, r);
                double residual = Math.Sqrt(rNewDot);

                if (residual < tolerance)
                {
                    return new SolverResult(x, iteration + 1, residual, true);
                }

                double beta = rNewDot / rOldDot;

                for (int i = 0; i < n; i++)
                    p[i] = r[i] + beta * p[i];

                rOldDot = rNewDot;
            }

            return new SolverResult(x, iteration, Math.Sqrt(rOldDot), false);
        }
    }
}