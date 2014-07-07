Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Globalization

Public Class ChangePasswordModel
    Private oldPasswordValue As String
    Private newPasswordValue As String
    Private confirmPasswordValue As String

    <Required()> _
    <DataType(DataType.Password)> _
    <Display(Name:="Current password")> _
    Public Property OldPassword() As String
        Get
            Return oldPasswordValue
        End Get
        Set(ByVal value As String)
            oldPasswordValue = value
        End Set
    End Property

    <Required()> _
    <StringLength(100, ErrorMessage:="The {0} must be at least {2} characters long.", MinimumLength:=$minimumrequiredlength$)> _
    <DataType(DataType.Password)> _
    <Display(Name:="New password")> _
    Public Property NewPassword() As String
        Get
            Return newPasswordValue
        End Get
        Set(ByVal value As String)
            newPasswordValue = value
        End Set
    End Property

    <DataType(DataType.Password)> _
    <Display(Name:="Confirm new password")> _
    <System.Web.Mvc.Compare("NewPassword", ErrorMessage:="The new password and confirmation password do not match.")> _
    Public Property ConfirmPassword() As String
        Get
            Return confirmPasswordValue
        End Get
        Set(ByVal value As String)
            confirmPasswordValue = value
        End Set
    End Property
End Class

Public Class LoginModel
    Private userNameValue As String
    Private passwordValue As String
    Private rememberMeValue As Boolean

    <Required()> _
    <Display(Name:="User name")> _
    Public Property UserName() As String
        Get
            Return userNameValue
        End Get
        Set(ByVal value As String)
            userNameValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Password)> _
    <Display(Name:="Password")> _
    Public Property Password() As String
        Get
            Return passwordValue
        End Get
        Set(ByVal value As String)
            passwordValue = value
        End Set
    End Property

    <Display(Name:="Remember me?")> _
    Public Property RememberMe() As Boolean
        Get
            Return rememberMeValue
        End Get
        Set(ByVal value As Boolean)
            rememberMeValue = value
        End Set
    End Property
End Class

Public Class CreateUserModel
    Private userNameValue As String
    Private passwordValue As String
    Private confirmPasswordValue As String
    Private emailValue As String
	Private questionValue As String
	Private answerValue AS String

    <Required()> _
    <Display(Name:="User name")> _
    Public Property UserName() As String
        Get
            Return userNameValue
        End Get
        Set(ByVal value As String)
            userNameValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.EmailAddress)> _
    <Display(Name:="Email address")> _
    Public Property Email() As String
        Get
            Return emailValue
        End Get
        Set(ByVal value As String)
            emailValue = value
        End Set
    End Property

    <Required()> _
    <StringLength(100, ErrorMessage:="The {0} must be at least {2} characters long.", MinimumLength:=$minimumrequiredlength$)> _
    <DataType(DataType.Password)> _
    <Display(Name:="Password")> _
    Public Property Password() As String
        Get
            Return passwordValue
        End Get
        Set(ByVal value As String)
            passwordValue = value
        End Set
    End Property

    <DataType(DataType.Password)> _
    <Display(Name:="Confirm password")> _
    <System.Web.Mvc.Compare("Password", ErrorMessage:="The password and confirmation password do not match.")> _
    Public Property ConfirmPassword() As String
        Get
            Return confirmPasswordValue
        End Get
        Set(ByVal value As String)
            confirmPasswordValue = value
        End Set
    End Property
	
    $requiredquestionandanswer$  
    <Display(Name:="Password question")> _
    Public Property Question() As String
        Get
            Return questionValue
        End Get
        Set(ByVal value As String)
            questionValue = value
        End Set
    End Property
	
	$requiredquestionandanswer$   
    <Display(Name:="Password answer")> _
    Public Property Answer() As String
        Get
            Return answerValue
        End Get
        Set(ByVal value As String)
            answerValue = value
        End Set
    End Property
	
End Class
