using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgorithmApriori
{
    public class Table
    {
        private struct UserNameAndQuantity
        {
            public string Name { get; set; }
            public int Quantity { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is UserNameAndQuantity correctedObj)
                {
                    return correctedObj.Name == Name && correctedObj.Quantity == Quantity;
                }

                return false;
            }
        }
        
        private readonly Dictionary<int, List<string>> _rawData = new Dictionary<int, List<string>>
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

        private readonly Dictionary<int, List<string>> _namesInLowercase = new Dictionary<int, List<string>>();
        private readonly Dictionary<int, List<int>> _normalizedData = new Dictionary<int, List<int>>();

        private List<string>
            _uniqueNames; // Очень аккуратно нужно быть с этим списком. Ни в коем случае не менять последовательность имен. 

        public Table()
        {
            foreach (var kvp in _rawData)
            {
                _namesInLowercase[kvp.Key] = _rawData[kvp.Key].Select(name => name.ToLower()).ToList();
            }

            NormalizeData();
        }

        
        /// <summary>
        /// Нормализуем данные и представляет таблицу в виде пользователей и продуктов и на пересечение пользователя
        /// и продукта ставим 0, если пользователь не приобретал продукт и 1 если приобретал
        /// </summary>
        private Table NormalizeData()
        {
            var uniqueNames = GetUniqueNames();

            foreach (var name in uniqueNames)
            {
                foreach (var key in _namesInLowercase.Keys)
                {
                    var names = _namesInLowercase[key];
                    var userHaveSelectedName = names.Contains(name);

                    var number = userHaveSelectedName ? 1 : 0;
                    if (_normalizedData.ContainsKey(key))
                    {
                        _normalizedData[key].Add(number);
                    }
                    else
                    {
                        _normalizedData[key] = new List<int> { number };
                    }
                }
            }

            return this;
        }
        
        /// <summary>
        /// Генерируем ассоциативные правила
        /// </summary>
        public Table GenerateAssociativeRules()
        {
            // 0 итерация. получаем все стартовые комбинации
            var startCombinations = GetStartCombination();
            
            // 1 итерация. нужно расчитать сколько продуктов каждого типа всего

            // 2 итерация. нужно перебрать все комбинации
            var allCombinations = GetAllCombinations(startCombinations);
            
            
            // 3 итерация. нужно понять какие данные оставить, а какие убрать (по сути повторяется итерация 1)
            // todo сейчас не работает, когда у нас в комбинации всего один продукт, так как при одном продукте мы учитываем значение среди всех пользователей
            var filteredStartCombinations = FilterOutCombinations(allCombinations);
            
            Console.WriteLine($"Filtered combinations: {filteredStartCombinations.Count}");
            
            foreach (var combination in filteredStartCombinations)
            {
                Console.WriteLine($"ElementOne: {combination[0]}; ElementTwo: {combination[1]}");
            }

            return this;
        }

        /// <summary>
        /// Фильтруем все комбинации, те которые не подоходят очищаем
        /// </summary>
        private IReadOnlyList<IReadOnlyList<string>> FilterOutCombinations(IReadOnlyList<IReadOnlyList<string>> allCombinations)
        {
            var resultCombinations = new List<IReadOnlyList<string>>();

            foreach (var combinations in allCombinations)
            {
                var sumCorrectedCombinations = 0;
                foreach (var user in _normalizedData.Keys)
                {
                    var numberOfData = combinations.Sum(info => GetNumberOfDataFromUser(user, info));
                    if (numberOfData / combinations.Count == 1)
                    {
                        sumCorrectedCombinations++;
                    }
                }
                
                if (sumCorrectedCombinations >= 4)  // todo вынести 4 в константы
                {
                    resultCombinations.Add(combinations);
                }
            }
            
            return resultCombinations;
        }

        // private int GetNumberOfDataFromUser(int user, List<string> nameInfo)
        // {
        //     return nameInfo.Sum(info => GetNumberOfDataFromUser(user, info));
        // }
        
        /// <summary>
        /// Получаем кол-во данных, которые хранятся у выбранного пользователя под заданным именим
        /// </summary>
        private int GetNumberOfDataFromUser(int user, string nameInfo)
        {
            if (!_normalizedData.ContainsKey(user))
            {
                Console.WriteLine($"Error. The user with this id ({user}) is not in the table");
                return 0;
            }

            var uniqueNames = GetUniqueNames();
            for (int i = 0; i < uniqueNames.Count; i++)
            {
                if (uniqueNames[i] == nameInfo)
                {
                    return _normalizedData[user][i];
                }
            }
            
            Console.WriteLine($"Error. There is no product with this name ({nameInfo}) in the user table");
            return 0;
        }

        
        
        private IReadOnlyList<int> GetStringDataByName(string name)
        {
            var uniqueNames = GetUniqueNames();
            if (!uniqueNames.Contains(name))
            {
                return new List<int>();
            }

            for (int i = 0; i < uniqueNames.Count; i++)
            {
                if (name == uniqueNames[i])
                {
                    return _normalizedData[i];
                }
            }

            return new List<int>();
        }

        private IReadOnlyList<IReadOnlyList<string>> GetAllCombinations(List<List<string>> startCombination)
        {
            // todo нужно избавиться от дубликатов, когда у нас есть одинаковые перестановки.
            // если у нас есть (a, b) и (b, a), то нужно оставить только одну из двух версий, но не обе
            var resultCombination = new List<List<string>>();
            if (startCombination == null || startCombination.Count == 0)
            {
                return resultCombination;
            }
            
            
            for (var i = 0; i < startCombination.Count; i++)
            {
                for (var j = 0; j < startCombination.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    
                    var firstRow = startCombination[i];
                    var secondRow = startCombination[j];
                    var canCombine = TryToCombinationUsingK(firstRow, secondRow, out var resultCombine);
                    if (canCombine)
                    {
                        resultCombination.Add(resultCombine);
                    }
                }
            }
            
            return resultCombination;
        }

        private List<List<string>> GetStartCombination()
        {
            return GetUniqueNames().Select(name => new List<string> { name }).ToList();
        }

        /// <summary>
        /// Пытаемся комбинировать значения
        /// </summary>
        private bool TryToCombinationUsingK(List<string> elementsOne, List<string> elementsTwo, out List<string> result)
        {
            if (elementsOne.Count != elementsTwo.Count)
            {
                Console.WriteLine("Error. elementsOne is not equal elementsTwo");
                result = new List<string>();
                return false;
            }
            var k = elementsOne.Count - 1;
            if (k < 0)
            {
                Console.WriteLine("Error.K can't have value less 0");
                result = new List<string>();
                return false;
            }

            if (k == 0)
            {
                result = new List<string> { elementsOne[0], elementsTwo[0] };
                return true;
            }

            result = new List<string>();
            for (var i = 0; i < elementsOne.Count; i++)
            {
                if (i < k && elementsOne[i] != elementsTwo[i])
                {
                    result = new List<string>();
                    return false;
                }

                if (i < k)
                {
                    result.Add(elementsOne[i]);
                }
                else
                {
                    result.AddRange(new string[] {elementsOne[i], elementsTwo[i]});
                }
            }
            return true;
        }

        private IReadOnlyList<string> GetUniqueNames()
        {
            if (_uniqueNames != null)
            {
                return _uniqueNames.ToArray();
            }

            _uniqueNames = new List<string>();
            foreach (var key in _namesInLowercase.Keys)
            {
                var names = _namesInLowercase[key];
                _uniqueNames.AddRange(names.Where(name => !_uniqueNames.Contains(name)));
            }

            return _uniqueNames.ToArray();
        }

        private List<UserNameAndQuantity> GetAmountOfDataFromUser()
        {
            var result = new List<UserNameAndQuantity>();
            var uniqueNames = GetUniqueNames();
            foreach (var userData in _normalizedData.Keys.Select(user => _normalizedData[user]))
            {
                for (var i = 0; i < uniqueNames.Count; i++)
                {
                    var name = uniqueNames[i];
                    var quantity = userData[i];
                    result.Add(new UserNameAndQuantity()
                    {
                        Name = name,
                        Quantity = quantity
                    });
                }
            }

            return result;
        }

        public override string ToString()
        {
            var builder = new StringBuilder("Table\n[UserId]");
            var uniqueNames = GetUniqueNames();
            for (var i = 0; i < uniqueNames.Count; i++)
            {
                var name = uniqueNames[i];
                builder.Append(uniqueNames.Count - 1 == i ? $"[{name}]" : $"[{name}] - ");
            }

            builder.Append("\n");

            if (_normalizedData == null || _normalizedData.Count == 0)
            {
                return builder.ToString();
            }

            foreach (var user in _normalizedData.Keys)
            {
                builder.Append($"[{user}]");
                for (var i = 0; i < _normalizedData[user].Count; i++)
                {
                    var info = _normalizedData[user][i];
                    builder.Append(_normalizedData[user].Count - 1 == i ? $"[{info}]" : $"[{info}] - ");
                }

                builder.Append("\n");
            }

            return builder.ToString();
        }
    }
}