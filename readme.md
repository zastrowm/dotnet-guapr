GUAPR, GUI App Refresh, is an application that loads a .NET WPF app and automatically reloads the GUI controls as the WPF app is recompiled.  This allows for faster development when running/developing GUI applications.

## Usage

To get started, your assembly must:

- Reference the Guapr.ClientHosting nuget package
- Add an assembly attribute to your .NET application/dll that points to a valid `HostedEntryPoint` type

## HostedEntryPoint

You must apply the `HostedTargetTypeAttribute` to your assembly, passing in a reference to a type that implements `IHostedEntryPoint` .  It is the responsibility of this `IHostedEntryPoint`  subclass to create the control that will be shown in GUAPR and to load/save any related state that is needed for making the reload cycle as seamless as possible.

The fastest way to implement `IHostedEntryPoint` is by subclassing the `HostedEntryPoint<TFrameworkElement, TState>` abstract class.

### Example

Given a custom control that simply has a textbox for your first name and last name:

```csharp
// a simple control that simply demonstrates the UI
internal class OurCustomControl : UserControl
{
  public TextBox FirstName { get; }
  public TextBox LastName { get; }
  // let's also display how many times the app has been reloaded
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
```

As for the state that we're going to save and load when the app refreshes, let's save  text that was in both textboxes and, to make the reload really seamless, the last focused textbox:

```csharp
// the state that will be saved between sessions.  Put whatever you want in here to
// make the  reload as seamless as possible. 
internal class State
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public bool IsLastNameFocused { get; set; }
  public int SelectionIndex { get; set; }
  public int ReloadIndex { get; set; }
}
```

Now we need to implement the startup and shutdown logic to create and teardown the control as needed:

```csharp
// Entry point to let the client know how to start the app
internal class ExampleHostEntryPoint : HostedEntryPoint<OurCustomControl, State>
{
  // invoked when the app is starting.  We need to return the gui control here 
  protected override OurCustomControl Startup(
     IEntryPointStartupApi startupApi,
     State previousState)
  {
    // if we never started before, previousState could be null
    previousState = previousState ?? new State();
    previousState.ReloadIndex++;

    // create the gui, restoring the state via the State class (which is the only thing
    // that is saved between reloads) 
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

    // when the host has granted us focus, we will refocus the text box that had focus
    // before the reload
    startupApi.FocusGranted +=
      (sender, args) =>
      {
        var toFocus = previousState.IsLastNameFocused
          ? gui.LastName
          : gui.FirstName;

        toFocus.Focus();
        toFocus.SelectionStart = previousState.SelectionIndex;
      };

    // of course, return the gui that we want displayed
    return gui;
  }
    
  // Invoked when the current session of the GUI is shutting down.  Should be used to 
  // save whatever state we want to maintain so that the reload is as seamless as possible
  protected override State Shutdown(
    OurCustomControl gui,
    IEntryPointShutdownApi shutdownApi)
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
```

The only remaining step then is to add the assembly attribute letting the host know about your entry point:

```csharp
[assembly: HostedTargetType(typeof(ExampleHostEntryPoint))]
```

If you add this to a an empty project, you'll notice that when run from GUAPR, the text that you enter into the textboxes is maintained even if you recompile the project.