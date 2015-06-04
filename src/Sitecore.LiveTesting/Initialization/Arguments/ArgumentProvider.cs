namespace Sitecore.LiveTesting.Initialization.Arguments
{
  using System;
  using System.Reflection;

  /// <summary>
  /// Defines the base class for all argument providers.
  /// </summary>
  public abstract class ArgumentProvider : IInitializationContextAware
  {
    /// <summary>
    /// Sets initialization context.
    /// </summary>
    /// <param name="context">The context.</param>
    public void SetInitializationContext(object context)
    {
      TestInitializationContext testInitializationContextCandidate = context as TestInitializationContext;

      if (testInitializationContextCandidate == null)
      {
        throw new NotSupportedException(string.Format("Only contexts derived from '{0}' are supported.", typeof(TestInitializationContext).AssemblyQualifiedName));
      }

      ParameterInfo[] parameters = testInitializationContextCandidate.Method.GetParameters();

      for (int index = 0; index < testInitializationContextCandidate.Arguments.Length; ++index)
      {
        if (this.NeedToProvideValue(testInitializationContextCandidate.Arguments[index], parameters[index]))
        {
          testInitializationContextCandidate.Arguments[index] = this.ResolveValue(testInitializationContextCandidate.Arguments[index], parameters[index]);
        }
      }
    }

    /// <summary>
    /// Resolves the value for the parameter.
    /// </summary>
    /// <param name="value">The initial value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The new value.</returns>
    protected abstract object ResolveValue(object value, ParameterInfo parameter);

    /// <summary>
    /// Determines if other value than the specified one should be provided.
    /// </summary>
    /// <param name="value">The initial value.</param>
    /// <param name="parameter">The parameter to which new value will be passed.</param>
    /// <returns><value>true</value> if other than initial value needs to be specified, otherwise <value>false</value>.</returns>
    protected virtual bool NeedToProvideValue(object value, ParameterInfo parameter)
    {
      return value == null;
    }
  }
}
