namespace Sitecore.LiveTesting.Extensions.Tests.Initialization
{
  using System;
  using Sitecore.Events.Hooks;
  using Sitecore.LiveTesting.Extensions.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="HookAdapter{T}"/>
  /// </summary>
  public class HookToInitializationHandlerAdapterTest
  {
    /// <summary>
    /// Should create new instance of type provided in generic argument and call its initialize method.
    /// </summary>
    [Fact]
    public void ShouldCreateNewInstanceOfTypeProvidedInGenericArgumentAndCallItsInitializeMethod()
    {
      Activator.CreateInstance<HookAdapter<Hook>>();

      Assert.True(Hook.Initialized);
    }

    /// <summary>
    /// Sample hook class.
    /// </summary>
    public class Hook : IHook
    {
      /// <summary>
      /// Gets or sets a value indicating whether initialize method was called or not.
      /// </summary>
      public static bool Initialized { get; set; }
      
      /// <summary>
      /// The initialize.
      /// </summary>
      public void Initialize()
      {
        Initialized = true;
      }
    }
  }
}
