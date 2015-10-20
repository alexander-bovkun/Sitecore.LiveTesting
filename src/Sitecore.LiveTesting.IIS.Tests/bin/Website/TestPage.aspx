<%@ Page language="c#" %>

<script runat="server">
  void Page_Load(object sender, EventArgs e)
  {
    Response.Write(Sitecore.LiveTesting.IIS.Tests.Applications.IISTestApplicationManagerTest.TestEnvironmentVariable);
  }
</script>