using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Guapr.ClientHosting;
using Guapr.ClientHosting.Test;

[assembly: HostedTargetType(typeof(ExampleHostEntryPoint))]

namespace Guapr.ClientHosting.Test
{
  /// <summary> Entry point that simply exposes two text boxes with a First and Last Name. </summary>
  internal class ExampleHostEntryPoint : HostedEntryPoint<OurCustomControl, State>
  {
    // invoked when the app is starting.  The control that provides the entry point to the GUI
    // should be created and returned in here. 
    protected override OurCustomControl Startup(IEntryPointStartupInfo startupInfo, State previousState)
    {
      // if we never started before, this can be null
      previousState = previousState ?? new State();
      previousState.ReloadIndex++;

      // create the gui, restoring the state via the State class (which is the only thing that's
      // saved between reloads) 
      var gui = new OurCustomControl(previousState.ReloadIndex)
                {
                  FirstName =
                  {
                    Text = previousState.FirstName,
                  },
                  LastName =
                  {
                    Text = previousState.LastName
                  }
                };

      // when the host has granted us focus, let's refocus the last text box we were at
      startupInfo.FocusGranted +=
        (sender, args) =>
        {
          var toFocus = previousState.IsLastNameFocused
            ? gui.LastName
            : gui.FirstName;

          toFocus.Focus();
          toFocus.SelectionStart = previousState.SelectionIndex;
        };

      return gui;
    }

    // Invoked when the current session of the GUI is shutting down.  Should be used to save whatever state we
    // want to maintain so that the reload is as seamless as possible
    protected override State Shutdown(OurCustomControl gui, IEntryPointShutdownInfo shutdownInfo)
    {
      var focusedTextBox = gui.LastName.IsFocused ? gui.LastName : gui.FirstName;

      return new State()
             {
               FirstName = gui.FirstName.Text,
               LastName = gui.LastName.Text,
               IsLastNameFocused = focusedTextBox == gui.LastName,
               SelectionIndex = focusedTextBox.SelectionStart,
               ReloadIndex = gui.ReloadIndex,
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