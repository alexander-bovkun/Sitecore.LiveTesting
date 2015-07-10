namespace Sitecore.LiveTesting.Extensions.Initialization.Arguments
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Xml;
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
      IEnumerable<string> configurationNodes = parameter.GetCustomAttributes(typeof(SitecoreConfigurationReferenceAttribute)).Cast<SitecoreConfigurationReferenceAttribute>().Select(attribute => attribute.ConfigurationNode);

      if (!configurationNodes.Any())
      {
        configurationNodes = parameter.Member.GetCustomAttributes(typeof(SitecoreConfigurationReferenceAttribute)).Cast<SitecoreConfigurationReferenceAttribute>().Select(attribute => attribute.ConfigurationNode);
        
        if (parameter.Member.ReflectedType != null)
        {
          configurationNodes = configurationNodes.Union(parameter.Member.ReflectedType.GetCustomAttributes(typeof(SitecoreConfigurationReferenceAttribute)).Cast<SitecoreConfigurationReferenceAttribute>().Select(attribute => attribute.ConfigurationNode));
          configurationNodes = configurationNodes.Union(parameter.Member.ReflectedType.Assembly.GetCustomAttributes(typeof(SitecoreConfigurationReferenceAttribute)).Cast<SitecoreConfigurationReferenceAttribute>().Select(attribute => attribute.ConfigurationNode));
        }
      }

      XmlNode targetNode = null;

      foreach (string configurationNode in configurationNodes)
      {
        XmlNode nodeCandidate = Factory.GetConfigNode(configurationNode, true);

        if (parameter.ParameterType.IsAssignableFrom(Factory.CreateType(nodeCandidate, true)))
        {
          if (targetNode != null)
          {
            throw new InvalidOperationException(string.Format("Cannot resolve value for argument '{0}'. There are several configuration nodes applicable to this argument.", parameter.Name));
          }

          targetNode = nodeCandidate;
        }
      }

      return Factory.CreateObject(targetNode, true);
    }
  }
}
