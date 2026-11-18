using System;

namespace MatrixLibrary.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при несовпадении размерностей матриц во время математических операций.
    /// </summary>
    public class MatrixDimensionException : ArgumentException
    {
        public MatrixDimensionException(string message)
            : base(message)
        {
        }

        public MatrixDimensionException(string operation, int rowsA, int colsA, int rowsB, int colsB)
            : base($"Невозможно выполнить операцию '{operation}'. Размерности не совпадают: [{rowsA}x{colsA}] и [{rowsB}x{colsB}].")
        {
        }
    }
}