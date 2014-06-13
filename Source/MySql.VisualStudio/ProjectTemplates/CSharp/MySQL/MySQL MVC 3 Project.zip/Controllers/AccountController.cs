using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using $safeprojectname$.Models;

namespace $safeprojectname$.Controllers
{
  public class AccountController : Controller
  {

    //
    // GET: /Account/LogOn

    public ActionResult LogOn()
    {
      return View();
    }

    //
    // POST: /Account/LogOn

    [HttpPost]
    public ActionResult LogOn(LogOnModel model, string returnUrl)
    {
      if (ModelState.IsValid)
      {        
        if (Membership.ValidateUser(model.UserName, model.Password))
        {
          FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
          if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
              && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
          {
            return Redirect(returnUrl);
          }
          else
          {
            return RedirectToAction("Index", "Home");
          }
        }
        else
        {
          ModelState.AddModelError("", "The user name or password provided is incorrect.");
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    //
    // GET: /Account/LogOff

    public ActionResult LogOff()
    {
      FormsAuthentication.SignOut();

      return RedirectToAction("Index", "Home");
    }

    //
    // GET: /Account/Register

    public ActionResult Register()
    {
      return View();
    }

    //
    // POST: /Account/Register

    [HttpPost]
    public ActionResult Register(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        // Attempt to register the user
        MembershipCreateStatus createStatus;
        Membership.CreateUser(model.UserName, model.Password, model.Email, model.PasswordQuestion, model.PasswordAnswer, true, null, out createStatus);

        if (createStatus == MembershipCreateStatus.Success)
        {
          FormsAuthentication.SetAuthCookie(model.UserName, false);
          return RedirectToAction("Index", "Home");
        }
        else
        {
          ModelState.AddModelError("", ErrorCodeToString(createStatus));
        }
      }
      
      return View(model);
    }

    //
    // GET: /Account/ChangePassword

    [Authorize]
    public ActionResult ChangePassword()
    {
      return View();
    }

    //
    // POST: /Account/ChangePassword

    [Authorize]
    [HttpPost]
    public ActionResult ChangePassword(ChangePasswordModel model)
    {
      if (ModelState.IsValid)
      {
      
        bool changePasswordSucceeded;
        try
        {
          MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true);
          changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
        }
        catch (Exception)
        {
          changePasswordSucceeded = false;
        }

        if (changePasswordSucceeded)
        {
          return RedirectToAction("ChangePasswordSuccess");
        }
        else
        {
          ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        }
      }
      
      return View(model);
    }

    //
    // GET: /Account/ChangePasswordSuccess

    public ActionResult ChangePasswordSuccess()
    {
      return View();
    }

    #region Status Codes
    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
   
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "User name already exists. Please enter a different user name.";

        case MembershipCreateStatus.DuplicateEmail:
          return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "Email is invalid. Please enter a different value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please enter a different value and try again.";

        case MembershipCreateStatus.ProviderError:
          return "The authentication provider returned an error. Please verify and try again.";

        case MembershipCreateStatus.UserRejected:
          return "The user creation request has been canceled. Please verify and try again.";

        default:
          return "An unknown error occurred.";
      }
    }
    #endregion
  }
}
