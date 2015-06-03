namespace Sitecore.LiveTesting.Extensions.Initialization.Arguments
{
  using System;
  using System.Linq;
  using System.Reflection;
  using Sitecore.Configuration;
  using Sitecore.LiveTesting.Initialization.Arguments;

  /// <summary>
  /// The Sitecore argument provider.
  /// </summary>
  public class SitecoreArgumentProvider : ArgumentProvider
  {
    /// <summary>
    /// Resolves the value for the parameter.
    /// </summary>
    /// <param name="value">The initial value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The new value.</returns>
    protected override object ResolveValue(object value, ParameterInfo parameter)
    {
      SitecoreConfigurationMapAttribute mapping = parameter.GetCustomAttributes(typeof(SitecoreConfigurationMapAttribute)).Cast<SitecoreConfigurationMapAttribute>().SingleOrDefault(map => map.ArgumentType == parameter.ParameterType);

      if (mapping == null)
      {
        mapping = parameter.Member.GetCustomAttributes(typeof(SitecoreConfigurationMapAttribute)).Cast<SitecoreConfigurationMapAttribute>().SingleOrDefault(map => map.ArgumentType == parameter.ParameterType);
      }

      if (mapping == null)
      {
        mapping = parameter.Member.ReflectedType.GetCustomAttributes(typeof(SitecoreConfigurationMapAttribute)).Cast<SitecoreConfigurationMapAttribute>().SingleOrDefault(map => map.ArgumentType == parameter.ParameterType);
      }

      if (mapping == null)
      {
        mapping = parameter.Member.ReflectedType.Assembly.GetCustomAttributes(typeof(SitecoreConfigurationMapAttribute)).Cast<SitecoreConfigurationMapAttribute>().SingleOrDefault(map => map.ArgumentType == parameter.ParameterType);
      }

      if (mapping == null)
      {
        return null;
      }

      return Factory.CreateObject(mapping.ConfigurationNode, true);
    }
  }
}
