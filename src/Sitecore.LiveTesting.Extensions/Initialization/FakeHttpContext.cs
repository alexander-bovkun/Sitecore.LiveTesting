namespace Sitecore.LiveTesting.Extensions.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Runtime.Remoting.Messaging;
  using System.Runtime.Serialization;
  using System.Threading;
  using System.Web;
  using System.Web.Hosting;

  /// <summary>
  /// Initialization handler that sets up fake http context as a current http context.
  /// </summary>
  public sealed class FakeHttpContext
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeHttpContext"/> class.
    /// </summary>
    public FakeHttpContext()
    {
      CallContext.LogicalSetData("Sitecore.LiveTesting.FakeHttpContextInitializer", new FakeHttpContextInitializer());
    }

    /// <summary>
    /// Defines the class that sets up fake http context on call context transitions.
    /// </summary>
    [Serializable]
    public sealed class FakeHttpContextInitializer : ISerializable
    {
      /// <summary>
      /// The http context map.
      /// </summary>
      private static readonly IDictionary<int, HttpContext> HttpContextMap = new Dictionary<int, HttpContext>();

      /// <summary>
      /// Initializes a new instance of the <see cref="FakeHttpContextInitializer"/> class.
      /// </summary>
      public FakeHttpContextInitializer()
      {
        HttpContext fakeContext;

        lock (HttpContextMap)
        {
          if (HostingEnvironment.IsHosted && (!HttpContextMap.ContainsKey(Thread.CurrentThread.ManagedThreadId)))
          {
            HttpContextMap.Add(Thread.CurrentThread.ManagedThreadId, new HttpContext(new SimpleWorkerRequest("Sitecore.LiveTesting.Test.aspx", string.Empty, TextWriter.Null)));
          }

          fakeContext = HttpContextMap[Thread.CurrentThread.ManagedThreadId];
        }

        HttpContext.Current = fakeContext;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="FakeHttpContextInitializer"/> class.
      /// </summary>
      /// <param name="info">The info.</param>
      /// <param name="context">The context.</param>
      private FakeHttpContextInitializer(SerializationInfo info, StreamingContext context) : this()
      {
      }

      /// <summary>
      /// The get object data.
      /// </summary>
      /// <param name="info">The info.</param>
      /// <param name="context">The context.</param>
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
      }
    }
  }
}
