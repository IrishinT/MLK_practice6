using System;
using QueueLibrary.Dequeue;

namespace MLK_practice6
{
    /// <summary>
    /// Умножение произвольно больших целых чисел без использования BigInteger и библиотечных коллекций.
    /// </summary>
    public static class BigIntegerMultiplier
    {
        /// <summary>
        /// Перемножает два больших целых числа, представленных в виде строк.
        /// </summary>
        public static string Multiply(string a, string b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            (bool negA, string digitsA) = ParseNumber(a);
            (bool negB, string digitsB) = ParseNumber(b);

            if (digitsA.Length == 0 || digitsB.Length == 0)
                return "0";

            bool isNegative = negA != negB;

            int[] products = new int[digitsA.Length + digitsB.Length];

            // Умножение в столбик справа налево с немедленным переносом разряда
            for (int i = digitsA.Length - 1; i >= 0; i--)
            {
                int da = digitsA[i] - '0';
                for (int j = digitsB.Length - 1; j >= 0; j--)
                {
                    int db = digitsB[j] - '0';
                    int sum = da * db + products[i + j + 1];
                    products[i + j + 1] = sum % 10;
                    products[i + j] += sum / 10;
                }
            }

            // Пропускаем ведущие нули
            int start = 0;
            while (start < products.Length && products[start] == 0)
                start++;

            if (start == products.Length)
                return "0";

            // Собираем результат через ArrayDeque<char> из CollectionLibrary
            var resultDeque = new ArrayDeque<char>(products.Length - start + 1);

            if (isNegative)
                resultDeque.OfferLast('-');

            for (int k = start; k < products.Length; k++)
                resultDeque.OfferLast((char)('0' + products[k]));

            char[] chars = new char[resultDeque.GetCount()];
            int idx = 0;
            while (resultDeque.HasNext())
                chars[idx++] = resultDeque.PullFirst();

            return new string(chars);
        }

        private static (bool IsNegative, string NormalizedDigits) ParseNumber(string raw)
        {
            string trimmed = raw.Trim();
            if (trimmed.Length == 0)
                throw new FormatException("Входная строка пуста.");

            int start = 0;
            bool isNegative = false;

            if (trimmed[0] == '-')
            {
                isNegative = true;
                start = 1;
            }
            else if (trimmed[0] == '+')
            {
                start = 1;
            }

            if (start >= trimmed.Length)
                throw new FormatException($"Некорректное число: '{raw}'.");

            for (int i = start; i < trimmed.Length; i++)
            {
                if (trimmed[i] < '0' || trimmed[i] > '9')
                    throw new FormatException($"Недопустимый символ '{trimmed[i]}' в числе '{raw}'.");
            }

            // Убираем ведущие нули
            while (start < trimmed.Length && trimmed[start] == '0')
                start++;

            return (isNegative, trimmed.Substring(start));
        }
    }
}