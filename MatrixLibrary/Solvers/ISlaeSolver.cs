using MatrixLibrary.Core;

namespace MatrixLibrary.Solvers
{
    /// <summary>
    /// Интерфейс для алгоритмов решения систем линейных алгебраических уравнений (СЛАУ) вида: A * x = b.
    /// </summary>
    public interface ISlaeSolver
    {
        /// <summary>
        /// Решает систему A * x = b.
        /// </summary>
        /// <param name="A">Матрица коэффициентов (квадратная).</param>
        /// <param name="b">Вектор правой части.</param>
        /// <param name="tolerance">Требуемая точность решения (по умолчанию 1e-10).</param>
        /// <param name="maxIterations">Максимальное число шагов.</param>
        /// <param name="initialGuess">Начальное приближение вектора x (если null, берется нулевой вектор).</param>
        SolverResult Solve(CsrMatrix A, double[] b, double tolerance = 1e-10, int maxIterations = 1000, double[] initialGuess = null);
    }
}