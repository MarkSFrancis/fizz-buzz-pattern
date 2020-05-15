using System;

namespace FizzBuzzPattern
{
  internal class Program
  {
    private static readonly FizzBuzzReader FizzBuzzReader = new FizzBuzzReader();
    private static readonly UserInput UserInput = new UserInput();

    static void Main(string[] args)
    {
      var patternKeys = UserInput.Get("Please enter a template to use for fizz buzz:", v => FizzBuzzReader.TryGetKeys(v));

      Console.WriteLine("Pattern detected: ");
      Console.WriteLine(string.Join(Environment.NewLine, patternKeys));
      Console.WriteLine();

      var startAt = UserInput.ParseAndValidate<int>("Start at:", int.TryParse, "Cannot start at zero or less", i => i > 0);
      var count = UserInput.ParseAndValidate<int>("Count:", int.TryParse, "Count cannot be less than zero", i => i > 0);
      Console.WriteLine();

      var game = new FizzBuzz(patternKeys);
      var results = game.Play(startAt, count);
      Console.WriteLine(string.Join(Environment.NewLine, results));
    }
  }
}
