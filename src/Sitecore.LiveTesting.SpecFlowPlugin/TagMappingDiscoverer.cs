namespace Sitecore.LiveTesting.SpecFlowPlugin
{
  using System.Collections.Generic;
  using System.IO;
  using System.Security;

  /// <summary>
  /// Defines discoverer that finds which code snippets correspond to tags.
  /// </summary>
  public class TagMappingDiscoverer
  {
    /// <summary>
    /// The last feature file.
    /// </summary>
    private string lastPath;

    /// <summary>
    /// The mapping.
    /// </summary>
    private IDictionary<string, string> cache = new Dictionary<string, string>();

    /// <summary>
    /// The get tag mapping.
    /// </summary>
    /// <param name="featureFile">The feature File.</param>
    /// <param name="tag">The tag.</param>
    /// <returns>The mapped code snippet.</returns>
    public string GetTagMapping(string featureFile, string tag)
    {
      IDictionary<string, string> mapping = this.DiscoverTagMappings(featureFile);

      if (mapping.ContainsKey(tag))
      {
        return mapping[tag];
      }

      return null;
    }

    /// <summary>The discover tag mappings.</summary>
    /// <param name="path">The path.</param>
    /// <returns>The discovered mappings.</returns>
    protected virtual IDictionary<string, string> DiscoverTagMappings(string path)
    {
      if (this.lastPath == path)
      {
        return this.cache;
      }

      this.cache = new Dictionary<string, string>();
      Stack<string> configurationFileNames = new Stack<string>(this.DiscoverTagMappingsConfigurationFiles(path));

      while (configurationFileNames.Count > 0)
      {
        this.ApplyConfiguration(this.cache, configurationFileNames.Pop());
      }

      this.lastPath = path;
      return this.cache;
    }

    /// <summary>
    /// The discover tag mappings configuration files.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The collection of file paths.</returns>
    protected virtual IEnumerable<string> DiscoverTagMappingsConfigurationFiles(string path)
    {
      const string TagMappingsFileName = "TagMappings.txt";

      DirectoryInfo directory = new DirectoryInfo(path);
      directory = directory.Parent;

      while (directory != null)
      {
        FileInfo[] files = directory.GetFiles(TagMappingsFileName, SearchOption.TopDirectoryOnly);

        if (files.Length == 1)
        {
          yield return files[0].FullName;
        }

        try
        {
          directory = directory.Parent;
        }
        catch (SecurityException)
        {
          directory = null;
        }
      }
    }

    /// <summary>
    /// The apply configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configurationFile">The configuration file.</param>
    protected virtual void ApplyConfiguration(IDictionary<string, string> configuration, string configurationFile)
    {
      string[] lines = File.ReadAllLines(configurationFile);

      foreach (string line in lines)
      {
        int index = line.IndexOf(' ');

        if (index > -1)
        {
          string key = line.Substring(0, index);
          string value = line.Substring(index + 1);

          if (configuration.ContainsKey(key))
          {
            configuration[key] = value;
          }
          else
          {
            configuration.Add(key, value);
          }

        }
      }
    }
  }
}
