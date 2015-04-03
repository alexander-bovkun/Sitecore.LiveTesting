using Sitecore.LiveTesting.Initialization;
using Sitecore.LiveTesting.Tests.Initialization;

[assembly: InitializationHandler(typeof(TestApplicationInitializationActionDiscovererTest), "parameter")]
[assembly: InitializationHandler(typeof(TestApplicationInitializationActionDiscovererTest), Priority = -1)]
[assembly: InitializationHandler(typeof(TestApplicationInitializationActionDiscovererTest), Priority = 1)]

namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using NSubstitute;
  using Sitecore.LiveTesting.Application;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="TestApplicationInitializationActionDiscoverer"/>.
  /// </summary>
  public class TestApplicationInitializationActionDiscovererTest
  {
    /// <summary>
    /// Should throw not supported exception for other than test application initialization context types of contexts.
    /// </summary>
    [Fact]
    public void ShouldThrowNotSupportedExceptionForOtherThanTestApplicationInitializationContextTypesOfContexts()
    {
      TestApplicationInitializationActionDiscoverer discoverer = new TestApplicationInitializationActionDiscoverer();

      Assert.ThrowsDelegate action = () => discoverer.GetInitializationActions("context");

      Assert.Throws<NotSupportedException>(action);
    }

    /// <summary>
    /// Should discover initialization actions matching to assembly defined initialization handler attributes.
    /// </summary>
    [Fact]
    public void ShouldDiscoverInitializationActionsMatchingToAssemblyDefinedInitializationHandlerAttributes()
    {
      TestApplicationInitializationContext context = new TestApplicationInitializationContext(new TestApplication(Substitute.For<InitializationManager>(new TestInitializationActionDiscoverer(), new InitializationActionExecutor())));
      TestApplicationInitializationActionDiscoverer discoverer = new TestApplicationInitializationActionDiscoverer();

      IEnumerable<InitializationAction> result = discoverer.GetInitializationActions(context).ToArray();

      Assert.Equal(3, result.Count());

      Assert.Equal(typeof(TestApplicationInitializationActionDiscovererTest).AssemblyQualifiedName, result.First().Id);
      Assert.IsType<InitializationHandler>(result.First().State);
      Assert.Equal(typeof(TestApplicationInitializationActionDiscovererTest), ((InitializationHandler)result.First().State).Type);
      Assert.Empty(((InitializationHandler)result.First().State).Arguments);
      Assert.Equal(context, result.First().Context);

      Assert.Equal(typeof(TestApplicationInitializationActionDiscovererTest).AssemblyQualifiedName, result.ElementAt(1).Id);
      Assert.IsType<InitializationHandler>(result.ElementAt(1).State);
      Assert.Equal(typeof(TestApplicationInitializationActionDiscovererTest), ((InitializationHandler)result.ElementAt(1).State).Type);
      Assert.Equal(new object[] { "parameter" }, ((InitializationHandler)result.ElementAt(1).State).Arguments);
      Assert.Equal(context, result.ElementAt(1).Context);

      Assert.Equal(typeof(TestApplicationInitializationActionDiscovererTest).AssemblyQualifiedName, result.ElementAt(2).Id);
      Assert.IsType<InitializationHandler>(result.ElementAt(2).State);
      Assert.Equal(typeof(TestApplicationInitializationActionDiscovererTest), ((InitializationHandler)result.ElementAt(2).State).Type);
      Assert.Empty(((InitializationHandler)result.ElementAt(2).State).Arguments);
      Assert.Equal(context, result.ElementAt(2).Context);
    }
  }
}
