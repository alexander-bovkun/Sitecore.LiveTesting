namespace Sitecore.LiveTesting.Extensions.Request
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.LiveTesting.Extensions.InitializationHandlers.Request;
  using Sitecore.LiveTesting.Initialization;
  using Sitecore.LiveTesting.Request;
  using Sitecore.Security.Accounts;

  /// <summary>
  /// The authenticated request.
  /// </summary>
  [Serializable]
  public class AuthenticatedRequest : Request
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticatedRequest"/> class.
    /// </summary>
    /// <param name="userName">The user name.</param>
    public AuthenticatedRequest(string userName)
    {
      Assert.ArgumentNotNullOrEmpty(userName, "userName");

      this.InitializationHandlers.Add(new InitializationHandler(typeof(HttpContextProvider), new object[0]));
      this.InitializationHandlers.Add(new InitializationHandler(typeof(UserSwitcher), new object[] { userName, true }));
    }
  }
}
