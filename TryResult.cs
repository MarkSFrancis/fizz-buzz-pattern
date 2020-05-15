using System;

namespace FizzBuzzPattern
{
  public class TryResult
  {
    protected TryResult()
    {
      Success = true;
    }

    protected TryResult(string error)
    {
      Success = false;
      _error = error;
    }

    public bool Success { get; }
    private string _error;

    public string ErrorMessage => Success ? null : _error;

    public override string ToString()
    {
      if (Success)
      {
        return "Success";
      }
      else
      {
        return "Error: " + ErrorMessage;
      }
    }

    public static TryResult Fail(string error)
    {
      return new TryResult(error);
    }

    public static TryResult Succeed()
    {
      return new TryResult();
    }

    public void Deconstruct(out bool success, out string error)
    {
      success = Success;
      error = ErrorMessage;
    }

    public static implicit operator bool(TryResult result)
    {
      return result.Success;
    }

    public static implicit operator TryResult(string error)
    {
      return Fail(error);
    }

    public static implicit operator TryResult((bool success, string error) result)
    {
      return result.success ? Succeed() : Fail(result.error);
    }

    public static implicit operator (bool success, string error)(TryResult result)
    {
      return (result.Success, result.Success ? null : result.ErrorMessage);
    }
  }

  public class TryResult<T> : TryResult
  {
    protected TryResult(string error) : base(error)
    {
    }

    protected TryResult(T result) : base()
    {
      Result = result;
    }

    public T Result { get; }

    public void Deconstruct(out T result, out string error)
    {
      result = Result;
      error = ErrorMessage;
    }

    public void Deconstruct(out bool success, out T result, out string error)
    {
      success = Success;
      result = Result;
      error = ErrorMessage;
    }

    public static new TryResult<T> Fail(string error)
    {
      return new TryResult<T>(error);
    }

    public static TryResult<T> Succeed(T result)
    {
      return new TryResult<T>(result);
    }

    public static implicit operator TryResult<T>(string error)
    {
      return Fail(error);
    }

    public static implicit operator TryResult<T>(T result)
    {
      return Succeed(result);
    }
  }

  public static class ParseFluent
  {
    public static TryResult ThenTry(this TryResult input, Func<TryResult> tryExecute)
    {
      if (!input)
      {
        return input;
      }

      return tryExecute();
    }

    public static TryResult<TOut> ThenTry<TOut>(this TryResult input, Func<TryResult<TOut>> tryExecute)
    {
      if (!input)
      {
        return TryResult<TOut>.Fail(input.ErrorMessage);
      }

      return tryExecute();
    }

    public static TryResult<TOut> ThenTry<TOut>(this TryResult input, Func<TOut> execute)
    {
      if (!input)
      {
        return TryResult<TOut>.Fail(input.ErrorMessage);
      }

      return execute();
    }

    public static TryResult ThenTry<TIn>(this TryResult<TIn> input, Func<TIn, TryResult> tryExecute)
    {
      if (!input)
      {
        return input;
      }

      return tryExecute(input.Result);
    }

    public static TryResult<TOut> ThenTry<TIn, TOut>(this TryResult<TIn> input, Func<TIn, TryResult<TOut>> tryExecute)
    {
      if (!input)
      {
        return TryResult<TOut>.Fail(input.ErrorMessage);
      }

      return tryExecute(input.Result);
    }

    public static TryResult<TOut> ThenTry<TIn, TOut>(this TryResult<TIn> input, Func<TIn, TOut> execute)
    {
      if (!input)
      {
        return TryResult<TOut>.Fail(input.ErrorMessage);
      }

      return execute(input.Result);
    }
  }
}
