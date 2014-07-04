@ModelType $safeprojectname$.CreateUserModel

@Code
    ViewData("Title") = "Create User"
End Code

<h2>Register your new user account</h2>

@Using Html.BeginForm()
    @Html.ValidationSummary(true, "User creation was not successful. Please correct the errors and try again.")
    @<div>
        <fieldset>

            <div class="editor-label">
                @Html.LabelFor(Function(m) m.UserName)
            </div>
            <div class="editor-field">
                @Html.TextBoxFor(Function(m) m.UserName)
                @Html.ValidationMessageFor(Function(m) m.UserName)
            </div>

            <div class="editor-label">
                @Html.LabelFor(Function(m) m.Email)
            </div>
            <div class="editor-field">
                @Html.TextBoxFor(Function(m) m.Email)
                @Html.ValidationMessageFor(Function(m) m.Email)
            </div>

            <div class="editor-label">
                @Html.LabelFor(Function(m) m.Password)
            </div>
            <div class="editor-field">
                @Html.PasswordFor(Function(m) m.Password)
                @Html.ValidationMessageFor(Function(m) m.Password)
            </div>

            <div class="editor-label">
                @Html.LabelFor(Function(m) m.ConfirmPassword)
            </div>
            <div class="editor-field">
                @Html.PasswordFor(Function(m) m.ConfirmPassword)
                @Html.ValidationMessageFor(Function(m) m.ConfirmPassword)
            </div>
			
			<div class="editor-label">
                @Html.LabelFor(Function(m) m.Question)
            </div>
            <div class="editor-field">
                @Html.TextBoxFor(Function(m) m.Question)
                @Html.ValidationMessageFor(Function(m) m.Question)
            </div>			
			<div class="editor-label">
                @Html.LabelFor(Function(m) m.Answer)
            </div>
            <div class="editor-field">
                @Html.TextBoxFor(Function(m) m.Answer)
                @Html.ValidationMessageFor(Function(m) m.Answer)
            </div>
            <p>
                <input type="submit" value="Create User" />
            </p>			
			<p>
				Passwords are required to be a minimum of @Membership.MinRequiredPasswordLength characters in length.
			</p>
        </fieldset>
    </div>
End Using
