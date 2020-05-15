using System;

namespace FizzBuzzPattern
{
  public class UserInput
  {
    public T ParseAndValidate<T>(string question, TryParse<T> convert, string error, Func<T, bool> isValid)
    {
      if (isValid is null)
      {
        throw new ArgumentNullException(nameof(isValid));
      }

      return ParseAndValidate(question, convert, error, v => isValid(v) ? TryResult<T>.Succeed(v) : error);
    }

    public T ParseAndValidate<T>(string question, TryParse<T> convert, string error, Func<T, TryResult<T>> isValid)
    {
      if (convert is null)
      {
        throw new ArgumentNullException(nameof(convert));
      }

      if (isValid is null)
      {
        throw new ArgumentNullException(nameof(isValid));
      }

      var tryResult = convert.ToTryResult(error);
      return Get<T>(question, (string v) => tryResult(v).ThenTry(isValid));
    }

    public T Parse<T>(string question, TryParse<T> convert, string error)
    {
      return ParseAndValidate(question, convert, error, t => t);
    }

    public T Get<T>(string question, Func<string, TryResult<T>> convert)
    {
      if (string.IsNullOrWhiteSpace(question))
      {
        throw new ArgumentException("Please specify a question to show the user", nameof(question));
      }

      if (convert is null)
      {
        throw new ArgumentNullException(nameof(convert));
      }

      TryResult<T> tryResult;
      string lastErrorMessage = null;
      Console.WriteLine(question);

      do
      {
        var userInput = Console.ReadLine();

        tryResult = convert(userInput);
        if (tryResult)
        {
          return tryResult.Result;
        }

        UndoWriteLine(userInput.Length);
        if (lastErrorMessage != null)
        {
          UndoWriteLine(lastErrorMessage.Length);
        }

        Console.WriteLine(tryResult.ErrorMessage);

        lastErrorMessage = tryResult.ErrorMessage;
      } while (true);
    }

    private static void UndoWriteLine(int lastLineLength)
    {
      if (Console.IsOutputRedirected)
      {
        return;
      }
      
      Console.CursorLeft = 0;
      Console.CursorTop--;
      Console.Write(new string(' ', lastLineLength));
      Console.CursorLeft = 0;
    }

    public delegate bool TryParse<T>(string input, out T value);
  }

  public static class TryParseExtensions
  {
    public static Func<string, TryResult<T>> ToTryResult<T>(this UserInput.TryParse<T> tryParse, string errorMessage)
    {
      return s =>
      {
        if (!tryParse(s, out T result))
        {
          return errorMessage;
        }

        return result;
      };
    }
  }
}
