namespace MatrixLibrary.Solvers
{
    /// <summary>
    /// Результат решения системы линейных уравнений (СЛАУ).
    /// </summary>
    public class SolverResult
    {
        /// <summary>
        /// Найденный вектор неизвестных (вектор x в уравнении Ax = b).
        /// </summary>
        public double[] Solution { get; }

        /// <summary>
        /// Количество выполненных итераций.
        /// </summary>
        public int Iterations { get; }

        /// <summary>
        /// Итоговая невязка (Residual Error, норма ||Ax - b||). 
        /// Чем ближе к 0, тем точнее решение.
        /// </summary>
        public double ResidualError { get; }

        /// <summary>
        /// Удалось ли сойтись к заданной точности за выделенное число итераций.
        /// </summary>
        public bool HasConverged { get; }

        public SolverResult(double[] solution, int iterations, double residualError, bool hasConverged)
        {
            Solution = solution;
            Iterations = iterations;
            ResidualError = residualError;
            HasConverged = hasConverged;
        }
    }
}