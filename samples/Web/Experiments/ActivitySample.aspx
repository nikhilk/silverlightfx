<%@ Page Language="C#" EnableViewState="false" EnableEventValidation="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<head runat="server">
  <title>Activity Sample</title>
</head>
<body style="height: 100%; margin: 0;">
  <div style="height: 100%;">
    <object style="width: 100%; height: 100%" type="application/x-silverlight">
	    <param name="source" value="Experiments.xap" />
	    <param name="version" value="2.0" />
	    <param name="initParams" value="page=ActivityPage" />
    </object>  
  </div>
</body>
</html>
