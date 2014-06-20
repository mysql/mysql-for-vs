@ModelType $safeprojectname$.LoginModel

@Code
    ViewData("Title") = "Login"
End Code

<h2>Login</h2>


@Html.ValidationSummary(True, "Login was unsuccessful. Please correct the errors and try again.")

@Using Html.BeginForm()
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
                @Html.LabelFor(Function(m) m.Password)
            </div>
            <div class="editor-field">
                @Html.PasswordFor(Function(m) m.Password)
                @Html.ValidationMessageFor(Function(m) m.Password)
            </div>

            <div class="editor-label">
                @Html.CheckBoxFor(Function(m) m.RememberMe)
                @Html.LabelFor(Function(m) m.RememberMe)
            </div>

            <p>
                <input type="submit" value="Login" />
            </p>
			<p>
				@Html.ActionLink("Register", "CreateUser") if you don't have an account.
			</p>
        </fieldset>
    </div>
End Using
