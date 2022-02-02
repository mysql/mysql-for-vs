// Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System;
using System.Linq;
using System.Windows.Forms;
using MySql.Utility.Classes;

namespace MySql.Utility.Forms
{
  /// <summary>
  /// A dialog that introduces a common validating infrastructure for input fields.
  /// </summary>
  public partial class ValidatingBaseDialog : AutoStyleableBaseDialog
  {
    #region Fields

    /// <summary>
    /// Flag indicating whether the <see cref="TextChangedHandler"/> logic must not execute.
    /// </summary>
    private bool _skipTextChanged;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatingBaseDialog"/> class.
    /// </summary>
    public ValidatingBaseDialog()
    {
      _skipTextChanged = false;
      IsUserInput = true;
      InitializeComponent();
      ErrorProperties = ErrorProviderProperties.Empty;
      ErrorLabel = null;
      SkipUpdateAcceptButton = false;
    }

    #region Properties

    /// <summary>
    /// Gets or sets <see cref="ErrorProviderProperties"/> to use with the <see cref="ValidationsErrorProvider"/>.
    /// </summary>
    protected ErrorProviderProperties ErrorProperties { get; }

    /// <summary>
    /// Gets or sets an optional <see cref="Label"/> shown next to an <see cref="ErrorProvider"/>.
    /// </summary>
    protected Label ErrorLabel { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Control"/> that the <see cref="ValidationsErrorProvider"/> is attached to.
    /// </summary>
    protected Control ErrorProviderControl { get; set; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="ErrorProviderControl"/> is enabled.
    /// </summary>
    protected bool ErrorProviderControlEnabled => ErrorProviderControl != null
                                                  && ErrorProviderControl.Enabled
                                                  && (!(ErrorProviderControl is TextBox textBox) || !textBox.ReadOnly);

    /// <summary>
    /// Gets or sets a value indicating whether when text changes on an input control was due user input or programmatic.
    /// </summary>
    protected bool IsUserInput;

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="UpdateAcceptButton"/> method is called.
    /// </summary>
    protected bool SkipUpdateAcceptButton { get; set; }

    #endregion Properties

    /// <summary>
    /// Fire validations for fields with no data, just for the sake of displaying their related error providers.
    /// </summary>
    /// <param name="validateInvisible">Flag indicating whether invisible controls are validated.</param>
    /// <param name="validateDisabled">Flag indicating whether disabled controls are validated.</param>
    protected virtual void FireAllValidations(bool validateInvisible = false, bool validateDisabled = false)
    {
      if (!IsUserInput)
      {
        return;
      }

      SkipUpdateAcceptButton = true;
      foreach (var control in this.GetChildControlsOfType<Control>())
      {
        var controlEnabled = control.Enabled
                             && (!(control is TextBox textBox) || !textBox.ReadOnly);
        if (!control.Visible && !validateInvisible
            || !controlEnabled && !validateDisabled)
        {
          continue;
        }

        if (control.InvokeRequired)
        {
          control.Invoke(new MethodInvoker(() => control.Validate(false)));
        }
        else
        {
          control.Validate(false);
        }
      }

      SkipUpdateAcceptButton = false;
      UpdateAcceptButton();
    }

    /// <summary>
    /// Fire validations for fields with no data, just for the sake of displaying their related error providers.
    /// </summary>
    /// <param name="validateInvisible">Flag indicating whether invisible controls are validated.</param>
    /// <param name="validateDisabled">Flag indicating whether disabled controls are validated.</param>
    protected void FireAllValidationsAsync(bool validateInvisible = false, bool validateDisabled = false)
    {
      if (!IsUserInput
          || ValidationsBackgroundWorker.IsBusy)
      {
        // If the BackgroundWorker is busy no need to put the main thread UI on wait in order to fire again,
        // bad design from the caller to attempt to validate twice in a very short period of time.
        return;
      }

      ValidationsBackgroundWorker.RunWorkerAsync(new Tuple<bool, bool>(validateInvisible, validateDisabled));
    }

    /// <summary>
    /// Resets the validations timer.
    /// </summary>
    protected void ResetValidationsTimer()
    {
      ValidationsTimer.Stop();
      ValidationsTimer.Start();
    }

    /// <summary>
    /// Sets the text property value of the given control.
    /// </summary>
    /// <param name="control">Any object inheriting from <see cref="Control"/>.</param>
    /// <param name="textValue">Text to assign to the control's Text property.</param>
    protected virtual void SetControlTextSkippingValidation(Control control, string textValue)
    {
      if (control == null)
      {
        return;
      }

      if (InvokeRequired)
      {
        Invoke(new MethodInvoker(() => SetControlTextSkippingValidation(control, textValue)));
        return;
      }

      if (control.Text == textValue)
      {
        return;
      }

      _skipTextChanged = true;
      control.Text = textValue;
      _skipTextChanged = false;
    }

    /// <summary>
    /// Handles the TextChanged event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    /// <remarks>This event method is meant to be used with the <see cref="Control.TextChanged"/> event.</remarks>
    protected virtual void TextChangedHandler(object sender, EventArgs e)
    {
      if (_skipTextChanged)
      {
        return;
      }

      ResetValidationsTimer();
    }

    /// <summary>
    /// Updates the enabled or disabled status of the button assigned to the <see cref="Form.AcceptButton"/> of the dialog.
    /// </summary>
    protected virtual void UpdateAcceptButton()
    {
      if (InvokeRequired)
      {
        Invoke(new MethodInvoker(UpdateAcceptButton));
        return;
      }

      if (SkipUpdateAcceptButton
          || !(AcceptButton is Button acceptButton))
      {
        return;
      }

      acceptButton.Enabled = !ValidationsErrorProvider.HasErrors();
    }

    /// <summary>
    /// Handles the TextValidated event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    /// <remarks>This event method is meant to be used with the <see cref="Control.Validated"/> event.</remarks>
    protected virtual void ValidatedHandler(object sender, EventArgs e)
    {
      if (InvokeRequired)
      {
        Invoke(new MethodInvoker(() => ValidatedHandler(sender, e)));
        return;
      }

      ValidationsTimer.Stop();
      if (!IsUserInput)
      {
        return;
      }

      ErrorProviderControl = sender as Control;
      if (ErrorProviderControl == null)
      {
        return;
      }

      ErrorLabel = null;
      ErrorProperties.ErrorMessage = ValidateFields();

      // This single error provider should be used for the majority of validations, unless other providers are needed, which can be handled in this same method.
      ValidationsErrorProvider.SetProperties(ErrorProviderControl, ErrorProperties);

      if (ErrorLabel != null)
      {
        ErrorLabel.Text = ErrorProperties.ErrorMessage;
      }

      UpdateAcceptButton();
    }

    /// <summary>
    /// Contains calls to methods that validate the given control's value.
    /// </summary>
    /// <returns>An error message or <c>null</c> / <see cref="string.Empty"/> if everything is valid.</returns>
    /// <remarks>This event method is meant to ALWAYS been overridden to specify actions on the switch to validate different text boxes.</remarks>
    protected virtual string ValidateFields()
    {
      if (InvokeRequired)
      {
        return (string)Invoke(new Func<string>(ValidateFields));
      }

      if (ErrorProviderControl == null)
      {
        return null;
      }

      string errorMessage = null;
      switch (ErrorProviderControl.Name)
      {
        case "SomeControl":
          ErrorProviderControl = null; // Here we may override the control to attach the error provider to a different one.
          ErrorLabel = null; // Here we may set the ErrorLabel to a specific label control used along with SomeControl.
          errorMessage = "Add here a call to a method that validates the control's value and returns an error message or null/empty if everything is valid";
          break;
      }

      return errorMessage;
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="MySqlWorkbenchConnectionDialog"/> is being closed.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ValidatingBaseDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.Cancel
          && ValidationsBackgroundWorker.IsBusy)
      {
        ValidationsBackgroundWorker.CancelAsync();
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="ValidationsTimer"/> timer's elapses.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void ValidationsTimerTick(object sender, EventArgs e)
    {
      var focusedControl = this.GetChildControlsOfType<Control>().FirstOrDefault(control => control.Focused);
      if (focusedControl != null)
      {
        focusedControl.Validate(false);
      }
      else
      {
        // In case no control has focus no validation is needed, just stop the timer.
        ValidationsTimer.Stop();
      }
    }

    /// <summary>
    /// EVent delegate method meant to run an asynchronous operation.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ValidationsBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      if (e.Argument is Tuple<bool, bool> tuple)
      {
        FireAllValidations(tuple.Item1, tuple.Item2);
      }
      else
      {
        FireAllValidations();
      }
    }
  }
}