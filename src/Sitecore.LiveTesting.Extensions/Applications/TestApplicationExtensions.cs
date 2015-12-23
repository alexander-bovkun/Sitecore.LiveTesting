namespace Sitecore.LiveTesting.Extensions.Applications
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.LiveTesting.Applications;

  /// <summary>
  /// Defines convenience extension methods for <see cref="TestApplication"/>.
  /// </summary>
  public static class TestApplicationExtensions
  {
    /// <summary>
    /// Executes the specified action in test application context.
    /// </summary>
    /// <param name="application">The application to execute action in.</param>
    /// <param name="action">The action to execute.</param>
    public static void ExecuteAction(this TestApplication application, Action action)
    {
      Assert.ArgumentNotNull(application, "application");

      application.ExecuteAction(action);
    }

    /// <summary>
    /// Executes the specified action in test application context with the given argument.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="application">The application to execute action in.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="argument">The argument to provide to the action.</param>
    public static void ExecuteAction<T>(this TestApplication application, Action<T> action, T argument)
    {
      Assert.ArgumentNotNull(application, "application");

      application.ExecuteAction(action, argument);
    }

    /// <summary>
    /// Executes the specified function in test application context.
    /// </summary>
    /// <typeparam name="TResult">The type of return value.</typeparam>
    /// <param name="application">The application to execute function in.</param>
    /// <param name="function">The function to execute.</param>
    /// <returns>The return value of executed function.</returns>
    public static TResult ExecuteFunction<TResult>(this TestApplication application, Func<TResult> function)
    {
      Assert.ArgumentNotNull(application, "application");

      return (TResult)application.ExecuteAction(function);
    }

    /// <summary>
    /// Executes the specified function in test application context with the given argument.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <typeparam name="TResult">The type of return value.</typeparam>
    /// <param name="application">The application to execute function in.</param>
    /// <param name="function">The function to execute.</param>
    /// <param name="argument">The argument to provide to the function.</param>
    /// <returns>The return value of executed function.</returns>
    public static TResult ExecuteFunction<T, TResult>(this TestApplication application, Func<T, TResult> function, T argument)
    {
      Assert.ArgumentNotNull(application, "application");

      return (TResult)application.ExecuteAction(function, argument);
    }
  }
}