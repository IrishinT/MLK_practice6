using System;
using CollectionLibrary.List;

namespace MLK_practice6;

internal static class Program
{
    private static void Main()
    {
        TestBigIntegerMultiplication();
        TestPolynomialArithmetic();
        TestFindKElementFromTail();
    }

    private static T FindKElementFromTail<T>(SinglyLinkedListImpl<T> list, int pos)
    {
        if (pos < 1 || pos > list.Size)
            throw new ArgumentOutOfRangeException(nameof(pos));

        return list[list.Size - pos];
    }
    private static void TestFindKElementFromTail()
    {
        Console.WriteLine("\n=== Задача 3: Поиск k-го элемента с конца списка ===");

        var list = new SinglyLinkedListImpl<int>();
        list.AddLast(10);
        list.AddLast(20);
        list.AddLast(30);
        list.AddLast(40);

        (int Position, int Expected)[] cases = [(1, 40), (2, 30), (4, 10)];
        foreach (var (position, expected) in cases)
        {
            int actual = FindKElementFromTail(list, position);
            if (actual != expected)
                throw new InvalidOperationException($"k = {position}: ожидалось {expected}, получено {actual}.");

            Console.WriteLine($"k = {position}: {actual} — тест пройден.");
        }
    }

    private static void TestBigIntegerMultiplication()
    {
        Console.WriteLine("=== Задача 1: Умножение больших целых чисел ===");

        string n1 = "123456789012345678901234567890";
        string n2 = "987654321098765432109876543210";
        Console.WriteLine($"{n1} * {n2} =\n{BigIntegerMultiplier.Multiply(n1, n2)}");
        Console.WriteLine($"-9999 * 99 = {BigIntegerMultiplier.Multiply("-9999", "99")}");
        Console.WriteLine($"-12345 * -100 = {BigIntegerMultiplier.Multiply("-12345", "-100")}");
        Console.WriteLine($"0 * 99999999999999999999 = {BigIntegerMultiplier.Multiply("0", "99999999999999999999")}");
    }

    private static void TestPolynomialArithmetic()
    {
        Console.WriteLine("\n=== Задача 2: Арифметика полиномов ===");

        // P(x) = 3x^3 - 2x^2 + x - 5  => [-5, 1, -2, 3]
        var p = new Polynomial(-5, 1, -2, 3);
        // Q(x) = x - 2               => [-2, 1]
        var q = new Polynomial(-2, 1);

        Console.WriteLine($"P(x) = {p}");
        Console.WriteLine($"Q(x) = {q}");
        Console.WriteLine($"P + Q = {p + q}");
        Console.WriteLine($"P - Q = {p - q}");
        Console.WriteLine($"P * Q = {p * q}");

        // Деление с остатком
        var (quotient, remainder) = p.DivideWithRemainder(q);
        Console.WriteLine($"P / Q (частное) = {quotient}");
        Console.WriteLine($"P % Q (остаток) = {remainder}");
        Console.WriteLine($"Проверка (Q * частное + остаток == P): {(quotient * q + remainder).ApproximatelyEquals(p)}");

        // Точное деление без остатка: (x^3 - 1) / (x - 1) = x^2 + x + 1
        var a = new Polynomial(-1, 0, 0, 1);
        var b = new Polynomial(-1, 1);
        Console.WriteLine($"({a}) / ({b}) [нацело] = {a.DivideExact(b)}");
    }
}
