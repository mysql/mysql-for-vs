Imports System.Diagnostics.CodeAnalysis
Imports System.Security.Principal
Imports System.Web.Routing

Public Class AccountController
    Inherits System.Web.Mvc.Controller

    Public Function Login() As ActionResult
        Return View()
    End Function

	
    <HttpPost()> _
    Public Function Login(ByVal model As LoginModel, ByVal returnUrl As String) As ActionResult
        If ModelState.IsValid Then
            If Membership.ValidateUser(model.UserName, model.Password) Then
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe)
                If Url.IsLocalUrl(returnUrl) AndAlso returnUrl.Length > 1 AndAlso returnUrl.StartsWith("/") _
                   AndAlso Not returnUrl.StartsWith("//") AndAlso Not returnUrl.StartsWith("/\\") Then
                    Return Redirect(returnUrl)
                Else
                    Return RedirectToAction("Index", "Home")
                End If
            Else
                ModelState.AddModelError("", "The user name or password is not correct.")
            End If
        End If
    
        Return View(model)
    End Function

        

    Public Function SignOut() As ActionResult
        FormsAuthentication.SignOut()

        Return RedirectToAction("Index", "Home")
    End Function

  
    Public Function CreateUser() As ActionResult
        Return View()
    End Function

    
    <HttpPost()> _
    Public Function CreateUser(ByVal model As CreateUserModel) As ActionResult
        If ModelState.IsValid Then            
            Dim createStatus As MembershipCreateStatus
            Membership.CreateUser(model.UserName, model.Password, model.Email, model.Question, model.Answer, True, Nothing, createStatus)

            If createStatus = MembershipCreateStatus.Success Then
                FormsAuthentication.SetAuthCookie(model.UserName, False)
                Return RedirectToAction("Index", "Home")
            Else
                ModelState.AddModelError("", ErrorCodeToString(createStatus))
            End If
        End If

        Return View(model)
    End Function


    <Authorize()> _
    Public Function ChangePassword() As ActionResult
        Return View()
    End Function


    <Authorize()> _
    <HttpPost()> _
    Public Function ChangePassword(ByVal model As ChangePasswordModel) As ActionResult
        If ModelState.IsValid Then            
            Dim changePasswordSucceeded As Boolean

            Try
                Dim currentUser As MembershipUser = Membership.GetUser(User.Identity.Name, True)
                changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword)
            Catch ex As Exception
                changePasswordSucceeded = False
            End Try

            If changePasswordSucceeded Then
                Return RedirectToAction("ChangePasswordSuccess")
            Else
                ModelState.AddModelError("", "The password is incorrect or new password is invalid.")
            End If
        End If
        
        Return View(model)
    End Function


    Public Function ChangePasswordSuccess() As ActionResult
        Return View()
    End Function

#Region "Status Code"
    Public Function ErrorCodeToString(ByVal createStatus As MembershipCreateStatus) As String
      
        Select Case createStatus
            Case MembershipCreateStatus.DuplicateUserName
                Return "User name already exists. Please enter a different user name."

            Case MembershipCreateStatus.DuplicateEmail
                Return "A user name for that e-mail address already exists. Please enter a different e-mail address."

            Case MembershipCreateStatus.InvalidPassword
                Return "The password provided is invalid. Please enter a valid password value."

            Case MembershipCreateStatus.InvalidEmail
                Return "Email is invalid. Please enter a different value and try again."

            Case MembershipCreateStatus.InvalidAnswer
                Return "The password answer provided is invalid. Please check the value and try again."

            Case MembershipCreateStatus.InvalidQuestion
                Return "The password question provided is invalid. Please check the value and try again."

            Case MembershipCreateStatus.InvalidUserName
                Return "The user name provided is invalid. Please enter a different value and try again."

            Case MembershipCreateStatus.ProviderError
                Return "The authentication provider returned an error. Please verify and try again."

            Case MembershipCreateStatus.UserRejected
                Return "The user creation request has been cancelled. Please verify and try again."

            Case Else
                Return "An unknown error occurred."
        End Select
    End Function
#End Region

End Class
