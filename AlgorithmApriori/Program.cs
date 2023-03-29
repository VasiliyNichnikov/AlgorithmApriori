using System;

namespace AlgorithmApriori
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Работает хорошо)
            var table = new Table();
            // Console.Write(table);
            table.GenerateAssociativeRules();

            // var normalizedData = Utils.NormalizeDataReturnList();
            // var normalizeTable = Utils.NormalizeDataReturnTable();
            //
            // var firstSet = new List<List<UserNameAndQuantity>>();
            // foreach (var item in normalizedData)
            // {
            //     firstSet.Add(new List<UserNameAndQuantity> { item });
            // }
            //
            // Utils.FindSets(normalizeTable, firstSet);
            //
            // foreach (var key in normalizeTable.Keys)
            // {
            //     var selectedString = normalizeTable[key];
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