namespace Sitecore.LiveTesting.SpecFlow.Tests
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using TechTalk.SpecFlow;
  using Xunit;

  /// <summary>
  /// Defines the step definitions.
  /// </summary>
  [Binding]
  public class StepDefinitions
  {
    /// <summary>
    /// The after feature.
    /// </summary>
    [AfterFeature]
    [Scope(Feature = "Initialization handlers")]
    public static void AfterFeature()
    {
      Assert.True(DisposableInitializationHandler.DisposedCount > 0);
    }

    /// <summary>
    /// The after scenario.
    /// </summary>
    [AfterScenario]
    [Scope(Feature = "Initialization handlers")]
    public static void AfterScenario()
    {
      InitializationHandler.InitializedCount = 0;
    }

    /// <summary>
    /// Given test with defined initialization handler.
    /// </summary>
    [Given(@"test with defined initialization handler")]
    public void GivenTestWithDefinedInitializationHandler()
    {
    }

    /// <summary>
    /// Given test with defined initialization handler that implements i disposable.
    /// </summary>
    [Given(@"test with defined initialization handler that implements IDisposable")]
    public void GivenTestWithDefinedInitializationHandlerThatImplementsIDisposable()
    {
    }

    /// <summary>
    /// Given test with multiple defined initialization handlers.
    /// </summary>
    [Given(@"test with multiple defined initialization handlers")]
    public void GivenTestWithMultipleDefinedInitializationHandlers()
    {
    }

    /// <summary>
    /// When test is about to be executed.
    /// </summary>
    [When(@"test is about to be executed")]
    public void WhenTestIsAboutToBeExecuted()
    {
    }

    /// <summary>
    /// When test was executed.
    /// </summary>
    [When(@"test was executed")]
    public void WhenTestWasExecuted()
    {
    }

    /// <summary>
    /// Then initialization handler for the test is created.
    /// </summary>
    [Then(@"initialization handler for the test is created")]
    public void ThenInitializationHandlerForTheTestIsCreated()
    {
      Assert.Equal(1, InitializationHandler.InitializedCount);
    }

    /// <summary>
    /// Then initialization handler instance is disposed.
    /// </summary>
    [Then(@"initialization handler instance is disposed")]
    public void ThenInitializationHandlerInstanceIsDisposed()
    {
      // See the actual assertion in AfterFeature hook.
    }

    /// <summary>
    /// Then initialization handlers are created in order of their priority.
    /// </summary>
    [Then(@"initialization handlers are created in order of their priority")]
    public void ThenInitializationHandlersAreCreatedInOrderOfTheirPriority()
    {
      // See the actual assertion in LowPriorityInitializationHandler itself.
    }

    /// <summary>
    /// Sample initialization handler.
    /// </summary>
    public class InitializationHandler
    {
      /// <summary>
      /// The counters.
      /// </summary>
      private static readonly IDictionary<int, int> Counters = new ConcurrentDictionary<int, int>();
      
      /// <summary>
      /// Initializes a new instance of the <see cref="InitializationHandler"/> class.
      /// </summary>
      public InitializationHandler()
      {
        ++InitializedCount;
      }

      /// <summary>
      /// Gets or sets number of initialized instances.
      /// </summary>
      public static int InitializedCount
      {
        get
        {
          if (!Counters.ContainsKey(Thread.CurrentThread.ManagedThreadId))
          {
            Counters.Add(Thread.CurrentThread.ManagedThreadId, 0);
          }

          Trace.WriteLine(Thread.CurrentThread.ManagedThreadId);
          
          return Counters[Thread.CurrentThread.ManagedThreadId];
        }
        
        set
        {
          if (!Counters.ContainsKey(Thread.CurrentThread.ManagedThreadId))
          {
            Counters.Add(Thread.CurrentThread.ManagedThreadId, value);
          }
          else
          {
            Counters[Thread.CurrentThread.ManagedThreadId] = value;
          }
        } 
      }
    }

    /// <summary>
    /// Sample disposable initialization handler.
    /// </summary>
    public class DisposableInitializationHandler : InitializationHandler, IDisposable
    {
      /// <summary>
      /// The disposed counters.
      /// </summary>
      private static readonly IDictionary<int, int> DisposedCounters = new ConcurrentDictionary<int, int>();      

      /// <summary>
      /// Gets or sets number of initialized instances.
      /// </summary>
      public static int DisposedCount 
      {
        get
        {
          if (!DisposedCounters.ContainsKey(Thread.CurrentThread.ManagedThreadId))
          {
            DisposedCounters.Add(Thread.CurrentThread.ManagedThreadId, 0);
          }

          return DisposedCounters[Thread.CurrentThread.ManagedThreadId];
        }

        set
        {
          if (!DisposedCounters.ContainsKey(Thread.CurrentThread.ManagedThreadId))
          {
            DisposedCounters.Add(Thread.CurrentThread.ManagedThreadId, value);
          }
          else
          {
            DisposedCounters[Thread.CurrentThread.ManagedThreadId] = value;
          }
        } 
      }

      /// <summary>
      /// Disposes instance of the type.
      /// </summary>
      public void Dispose()
      {
        this.Dispose(true);
      }

      /// <summary>
      /// Disposes instance of the type.
      /// </summary>
      /// <param name="disposing">Determines if object is being disposed.</param>
      protected virtual void Dispose(bool disposing)
      {
        ++DisposedCount;
      }
    }

    /// <summary>
    /// Defines the low priority initialization handler.
    /// </summary>
    public class LowPriorityInitializationHandler
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="LowPriorityInitializationHandler"/> class.
      /// </summary>
      public LowPriorityInitializationHandler()
      {
        Assert.Equal(2, InitializationHandler.InitializedCount);
      }
    }
  }
}
