namespace Sitecore.LiveTesting.Extensions.Initialization
{
  using Sitecore.Events.Hooks;

  /// <summary>
  /// Defines the adapter class from <see cref="IHook"/> to proper initialization handler.
  /// </summary>
  /// <typeparam name="T">The type of adapted class.</typeparam>
  public class HookToInitializationHandlerAdapter<T> where T : IHook, new()
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HookToInitializationHandlerAdapter{T}"/> class.
    /// </summary>
    public HookToInitializationHandlerAdapter()
    {
      new T().Initialize();
    }
  }
}
