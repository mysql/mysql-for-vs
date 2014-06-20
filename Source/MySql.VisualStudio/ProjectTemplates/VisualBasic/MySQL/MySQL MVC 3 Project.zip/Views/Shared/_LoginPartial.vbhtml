@If Request.IsAuthenticated Then
@<div id="logoutdisplay"> @Html.ActionLink("Logout", "SignOut", "Account")</div>
Else
@:<div id="logindisplay"> @Html.ActionLink("Login", "Login", "Account") </div>
End If