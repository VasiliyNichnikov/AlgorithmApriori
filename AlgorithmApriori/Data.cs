using System.Collections.Generic;
using System.Linq;

namespace AlgorithmApriori
{
    public static class Data
    {
        public static Dictionary<int, List<string>> Table { get; }

        private static readonly Dictionary<int, List<string>> _table = new Dictionary<int, List<string>>
        {
            { 1, new List<string> { "Капуста", "перец", "кукуруза" } },
            { 2, new List<string> { "Спаржа", "кабачки", "кукуруза" } },
            { 3, new List<string> { "Кукуруза", "помидоры", "фасоль", "кабачки" } },
            { 4, new List<string> { "Перец", "кукуруза", "помидоры", "фасоль" } },
            { 5, new List<string> { "Фасоль", "спаржа", "капуста" } },
            { 6, new List<string> { "Кабачки", "спаржа", "фасоль", "помидоры" } },
            { 7, new List<string> { "Помидоры", "кукуруза" } },
            { 8, new List<string> { "Капуста", "помидоры", "перец" } },
            { 9, new List<string> { "Кабачки", "спаржа", "фасоль" } },
            { 10, new List<string> { "Фасоль", "кукуруза" } },
            { 11, new List<string> { "Перец", "капуста", "фасоль", "кабачки" } },
            { 12, new List<string> { "Спаржа", "фасоль", "кабачки" } },
            { 13, new List<string> { "Кабачки", "кукуруза", "спаржа", "фасоль" } },
            { 14, new List<string> { "Кукуруза", "перец", "помидоры", "фасоль", "капуста" } }
        };

        static Data()
        {
            Table = new Dictionary<int, List<string>>();
            foreach (var kvp in _table)
            {
                Table[kvp.Key] = _table[kvp.Key].Select(name => name.ToLower()).ToList();
            }
        }
    }
}