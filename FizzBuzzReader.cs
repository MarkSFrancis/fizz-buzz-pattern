using System;
using System.Collections.Generic;
using System.Linq;

namespace FizzBuzzPattern
{
  public class FizzBuzzReader
  {
    public TryResult<Dictionary<int, string>> TryGetKeys(string pattern)
    {
      var split = SplitPattern(pattern).ToList();

      return
        ValidateStartIndex(split)
        .ThenTry(() => GetNonNumeric(split))
        .ThenTry(ExtractKeys);
    }

    private TryResult ValidateStartIndex(IReadOnlyList<string> pattern)
    {
      if (pattern.Count < 1)
      {
        return "Sequence must contain at least one element";
      }
      if (!int.TryParse(pattern[0], out var startIndex) || startIndex != 1)
      {
        return "Sequence must start with the number 1";
      }

      return TryResult.Succeed();
    }

    private IEnumerable<string> SplitPattern(string pattern)
    {
      return pattern
        .Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(p => p.Trim());
    }

    private TryResult<Dictionary<int, string>> GetNonNumeric(IReadOnlyList<string> pattern)
    {
      var results = new Dictionary<int, string>();

      for (int index = 1; index < pattern.Count; index++)
      {
        if (int.TryParse(pattern[index], out var val))
        {
          if (val != index + 1)
          {
            return $"Unexpected numeric value at position {index}. Pattern must increment by 1";
          }

          continue;
        }

        results.Add(index + 1, pattern[index]);
      }

      return results;
    }

    private Dictionary<int, string> ExtractKeys(Dictionary<int, string> possibleKeys)
    {
      Dictionary<int, string> keys = new Dictionary<int, string>();

      var keyGroups = new LinkedList<(string Key, IOrderedEnumerable<int> Value)>(possibleKeys
        .GroupBy(k => k.Value, k => k.Key)
        .Select(g => (g.Key, Value: g.OrderBy(g => g)))
        .OrderBy(g => g.Value.Min()));

      while (keyGroups.Count > 0)
      {
        var group = keyGroups.First.Value;

        var groupKey = group.Key;

        var partOfKey = keys.FirstOrDefault(k => groupKey.Contains(k.Value) && group.Value.All(v => IsFactorOf(v, k.Key)));
        while (partOfKey.Value != default)
        {
          // Known key - need to split known key out of group's key
          var groupIndex = groupKey.IndexOf(partOfKey.Value);

          groupKey = groupKey.Substring(0, groupIndex) + groupKey.Substring(groupIndex + partOfKey.Value.Length);
          partOfKey = keys.FirstOrDefault(k => k.Value.Contains(groupKey) && group.Value.All(v => IsFactorOf(v, k.Key)));
        }

        keys.Add(group.Value.First(), groupKey);
        keyGroups.RemoveFirst();
      }

      return keys;
    }

    private bool IsFactorOf(int value, int factorOf)
    {
      return value % factorOf == 0;
    }
  }
}
