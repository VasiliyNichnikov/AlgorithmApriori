using System;

namespace AlgorithmApriori
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Работает хорошо)
            var normalizedData = Utils.NormalizeData();

            // foreach (var key in normalizedData.Keys)
            // {
            //     var selectedString = normalizedData[key];
            //     Console.Write($"User: {key} ");
            //     foreach (var item in selectedString)
            //     {
            //         Console.Write($"Name: {item.Name}. Quantity: {item.Quantity} - ");
            //     }
            //     Console.WriteLine();
            // }
        }
    }
}