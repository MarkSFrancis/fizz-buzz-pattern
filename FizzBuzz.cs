using System.Collections.Generic;
using System.Text;

namespace FizzBuzzPattern
{
  public class FizzBuzz
  {
    public FizzBuzz(Dictionary<int, string> pattern)
    {
      Pattern = pattern;
    }

    public Dictionary<int, string> Pattern { get; }

    public IEnumerable<string> Play(int from, int count)
    {
      for(int index = 0; index < count; index++)
      {
        yield return GetValueAt(from + index);
      }
    }

    public string GetValueAt(int index)
    {
      StringBuilder text = new StringBuilder();

      foreach(var pair in Pattern)
      {
        if (index % pair.Key == 0)
        {
          text.Append(pair.Value);
        }
      }

      return text.Length == 0 ? index.ToString() : text.ToString();
    }
  }
}
