<%@ Page language="c#" %>

<script runat="server">
  void Page_Load(object sender, EventArgs e)
  {
    string header = HttpContext.Current.Request.QueryString["header"];

    if (!string.IsNullOrEmpty(header))
    {
      Response.Write(HttpContext.Current.Request.Headers[header]);
    }
    else
    {
      Response.Write("Test page");
    }
  }
</script>