using System.Collections.Generic;
using System.Linq;

namespace AlgorithmApriori
{
    public struct UserNameAndQuantity
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
    
    public class Utils
    {
        public static Dictionary<int, List<UserNameAndQuantity>> NormalizeData()
        {
            var table = Data.Table;
            var uniqueNames = GetUniqueNames(table);

            var normalizedTable = new Dictionary<int, List<UserNameAndQuantity>>();
            foreach (var name in uniqueNames)
            {
                foreach (var key in table.Keys)
                {
                    var names = table[key];
                    var userHaveSelectedName = names.Contains(name);
                    var data = new UserNameAndQuantity
                    {
                        Name = name,
                        Quantity = userHaveSelectedName ? 1 : 0
                    };
                    
                    if (normalizedTable.ContainsKey(key))
                    {
                        normalizedTable[key].Add(data);
                    }
                    else
                    {
                        normalizedTable[key] = new List<UserNameAndQuantity> { data };
                    }
                }
            }

            return normalizedTable;
        }

        private static string[] GetUniqueNames(Dictionary<int, List<string>> table)
        {
            var uniqueNames = new List<string>();
            foreach (var kvp in table)
            {
                var names = table[kvp.Key];
                uniqueNames.AddRange(names.Where(name => !uniqueNames.Contains(name)));
            }

            return uniqueNames.ToArray();
        }
    }
}