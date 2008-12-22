<%@ Page Language="C#" EnableViewState="false" EnableEventValidation="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Theme Samples</title>
  <style>object { border: solid 1px silver; display: inline-block; }</style>
</head>
<body>
  <object style="width: 290px; height: 100px" type="application/x-silverlight">
    <param name="source" value="ThemeSample.xap" />
    <param name="version" value="2.0" />
    <param name="initParams" value="page=Default" />
  </object>
  
  <br /><br />

  <object style="width: 290px; height: 100px" type="application/x-silverlight">
    <param name="source" value="ThemeSample.xap" />
    <param name="version" value="2.0" />
    <param name="initParams" value="theme=Red" />
  </object>

  &nbsp;

  <object style="width: 290px; height: 100px" type="application/x-silverlight">
    <param name="source" value="ThemeSample.xap" />
    <param name="version" value="2.0" />
    <param name="initParams" value="theme=Blue" />
  </object>

  &nbsp;

  <object style="width: 290px; height: 100px" type="application/x-silverlight">
    <param name="source" value="ThemeSample.xap" />
    <param name="version" value="2.0" />
  </object>
</body>
</html>
