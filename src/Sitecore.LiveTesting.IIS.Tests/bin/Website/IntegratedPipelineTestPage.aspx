<%@ Page language="c#" %>

<script runat="server">
  void Page_Load(object sender, EventArgs e)
  {
    Xunit.Assert.Equal("queryString=test", Request.QueryString.ToString());
    Xunit.Assert.Equal("POST", Request.HttpMethod);
    Xunit.Assert.Equal("header value", Request.Headers["custom-header"]);

    using (System.IO.StreamReader inputStream = new System.IO.StreamReader(Request.InputStream))
    {
      Xunit.Assert.Equal("data", inputStream.ReadToEnd());
    }
  }
</script>