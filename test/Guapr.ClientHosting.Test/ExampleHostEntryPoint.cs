using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Guapr.ClientHosting;
using Guapr.ClientHosting.Test;

[assembly: HostedTargetType(typeof(ExampleHostEntryPoint))]

namespace Guapr.ClientHosting.Test
{
  // Entry point that simply exposes two text boxes with a First and Last Name. 
  [DisplayName("Example Host")]
  [Description("Demonstrates a way to implement IHostedEntryPoint")]
  internal class ExampleHostEntryPoint : SimpleStateHostedEntryPoint<OurCustomControl, State>
  {
    /// <inheritdoc />
    public override State CreateDefaultState()
    {
      // if the app was never constructed before, this method will be invoked, so go ahead and
      // create the default state
      return new State();
    }

    /// <inheritdoc />
    public override OurCustomControl CreateControl()
    {
      // create the gui, restoring the state via the State class (which is the only thing that's
      // saved between reloads) 

      CurrentState.ReloadIndex++;

      return new OurCustomControl(CurrentState.ReloadIndex)
             {
               FirstName =
               {
                 Text = CurrentState.FirstName,
               },
               LastName =
               {
                 Text = CurrentState.LastName
               }
             };
    }

    /// <inheritdoc />
    public override void OnFocusReady()
    {
      // when the host has granted us focus, let's refocus the last text box we were at
      var toFocus = CurrentState.IsLastNameFocused
        ? Control.LastName
        : Control.FirstName;

      toFocus.Focus();
      toFocus.SelectionStart = CurrentState.SelectionIndex;
    }

    /// <inheritdoc />
    public override State GetStateToPersist()
    {
      // Invoked when the current session of the GUI is shutting down.  Should be used to 
      // save whatever state we want to maintain so that the reload is as seamless as possible
      var focusedTextBox = Control.LastName.IsFocused ? Control.LastName : Control.FirstName;

      return new State()
             {
               FirstName = Control.FirstName.Text,
               LastName = Control.LastName.Text,
               IsLastNameFocused = focusedTextBox == Control.LastName,
               SelectionIndex = focusedTextBox.SelectionStart,
               ReloadIndex = Control.ReloadIndex,
             };
    }
  }

  // the state that will be saved between sessions.  Put whatever you want in here to make the
  // reload as seamless as possible. 
  internal class State
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsLastNameFocused { get; set; }
    public int SelectionIndex { get; set; }
    public int ReloadIndex { get; set; }
  }

  // a simple control that simply demonstrates the UI
  internal class OurCustomControl : UserControl
  {
    public TextBox FirstName { get; }
    public TextBox LastName { get; }
    public int ReloadIndex { get; }

    public OurCustomControl(int reloadIndex)
    {
      ReloadIndex = reloadIndex;

      Content = new StackPanel()
                {
                  Children =
                  {
                    new Label() { Content = $"I've been Reloaded {reloadIndex} time(s)" },
                    new Label() { Content = "First Name:" },
                    (FirstName = new TextBox()),
                    new Label() { Content = "Last Name:" },
                    (LastName = new TextBox()),
                  }
                };
    }
  }
}