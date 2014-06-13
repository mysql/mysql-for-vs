Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        ViewData("Message") = "Welcome, to your new MVC application!"

        Return View()
    End Function

    Function About() As ActionResult
        Return View()
    End Function
End Class
