<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    <link href="@Url.Content("~/Content/Site.css")" rel="stylesheet" type="text/css" />    
	<script src="@Url.Content("~/Scripts/jquery-$jqueryversion$.min.js")" type="text/javascript"></script>
</head>
<body>
    <div class="page">
        <div id="header">
           
                <h1>My MVC Application</h1>
           
            <div id="logindisplay">
                @Html.Partial("_LoginPartial")
            </div>
            <div id="menucontainer">
                <ul id="menu">
                    <li>@Html.ActionLink("Home", "Index", "Home")</li>
                    <li>@Html.ActionLink("About", "About", "Home")</li>
                </ul>
            </div>
        </div>
        <div id="main">                       
            @If Request.IsAuthenticated Then    
                 @<div id="logindisplay">Welcome <strong>@User.Identity.Name</strong>!</div> 
			End If				 
            @RenderBody()
        </div>
        <div id="footer">
        </div>
    </div> 
</body>
</html>
