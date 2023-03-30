using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgorithmApriori
{
    public class Table
    {
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

        // Очень аккуратно нужно быть с этим списком. Ни в коем случае не менять последовательность имен.
        private List<string> _uniqueNames;

        private const int _minimumThreshold = 4;

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
        private void NormalizeData()
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
        }

        /// <summary>
        /// Генерируем ассоциативные правила
        /// </summary>
        public Table GenerateAssociativeRules()
        {
            // 0 итерация. получаем все стартовые комбинации
            var startCombinations = GetStartCombination();

            // 1 итерация. нужно перебрать все комбинации
            var allCombinations = GetAllCombinations(startCombinations);

            // 2 итерация. нужно опять перебрать все комбинации
            allCombinations = GetAllCombinations(allCombinations);

            foreach (var combination in allCombinations)
            {
                Console.WriteLine(
                    $"ElementOne: {combination[0]}; ElementTwo: {combination[1]}; ElementTwo: {combination[2]}; Count: {combination.Count}"); //ElementTwo: {combination[1]}; ElementThee: {combination[2]}
            }

            return this;
        }

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

        private IReadOnlyList<List<string>> GetAllCombinations(IReadOnlyList<IReadOnlyList<string>> startCombination)
        {
            var resultCombinations = new List<List<string>>();
            if (startCombination == null || startCombination.Count == 0)
            {
                return resultCombinations;
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
                    var canCombine = TryToCombinationUsingK(firstRow, secondRow, out var resultCombination);

                    // todo костыль)
                    if (resultCombination.Count == 2 && canCombine)
                    {
                        if (resultCombinations.Any(element =>
                                element[0] == resultCombination[1] && element[1] == resultCombination[0]))
                        {
                            continue;
                        }
                    }


                    if (canCombine)
                    {
                        resultCombinations.Add(resultCombination);
                    }
                }
            }

            return resultCombinations;
        }

        private List<List<string>> GetStartCombination()
        {
            return GetUniqueNames().Select(name => new List<string> { name }).ToList();
        }

        /// <summary>
        /// Пытаемся комбинировать значения
        /// </summary>
        private bool TryToCombinationUsingK(IReadOnlyList<string> elementsOne, IReadOnlyList<string> elementsTwo,
            out List<string> result)
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
                return CheckPassabilityOfMinimumThreshold(result);
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
                    result.AddRange(new[] { elementsOne[i], elementsTwo[i] });
                }
            }

            return CheckPassabilityOfMinimumThreshold(elementsOne) && CheckPassabilityOfMinimumThreshold(elementsTwo);
        }

        private bool CheckPassabilityOfMinimumThreshold(IReadOnlyList<string> elements)
        {
            var numberOfMatches = 0;
            foreach (var user in _normalizedData.Keys)
            {
                var numberOfData = elements.Sum(info => GetNumberOfDataFromUser(user, info));
                if (numberOfData / elements.Count == 1)
                {
                    numberOfMatches++;
                }
            }

            return numberOfMatches >= _minimumThreshold;
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