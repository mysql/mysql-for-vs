﻿@ModelType $safeprojectname$.ChangePasswordModel

@Code
    ViewData("Title") = "Change Password"
End Code

<h2>Change Password</h2>
<p>
    Use the form below to change your password. 
</p>
<p>
    New passwords are required to be a minimum of @Membership.MinRequiredPasswordLength characters in length.
</p>


@Using Html.BeginForm()
    @Html.ValidationSummary(True, "Password change was unsuccessful. Please correct the errors and try again.")
    @<div>
        <fieldset>            
            <div class="editor-label">
                @Html.LabelFor(Function(m) m.OldPassword)
            </div>
            <div class="editor-field">
                @Html.PasswordFor(Function(m) m.OldPassword)
                @Html.ValidationMessageFor(Function(m) m.OldPassword)
            </div>

            <div class="editor-label">
                @Html.LabelFor(Function(m) m.NewPassword)
            </div>
            <div class="editor-field">
                @Html.PasswordFor(Function(m) m.NewPassword)
                @Html.ValidationMessageFor(Function(m) m.NewPassword)
            </div>

            <div class="editor-label">
                @Html.LabelFor(Function(m) m.ConfirmPassword)
            </div>
            <div class="editor-field">
                @Html.PasswordFor(Function(m) m.ConfirmPassword)
                @Html.ValidationMessageFor(Function(m) m.ConfirmPassword)
            </div>

            <p>
                <input type="submit" value="Change Password" />
            </p>
        </fieldset>
    </div>
End Using
