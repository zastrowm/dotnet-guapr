GUAPR, GUI App Refresh, is an application that loads a .NET WPF app and automatically reloads the GUI controls as the WPF app is recompiled.  This allows for faster development when running/developing GUI applications.

## Usage

To get started, your assembly must:

- Reference the [WpfHosting.Client](https://www.nuget.org/packages/WpfHosting.Client) nuget package
- Add an assembly attribute to your .NET application/dll that points to a valid `HostedEntryPoint` type

## HostedEntryPoint

You must apply the `HostedTargetTypeAttribute` to your assembly, passing in a reference to a type that extends `HostedEntryPoint` .  It is the responsibility of this `HostedEntryPoint`  subclass to create the control that will be shown in GUAPR and to load/save any related state that is needed for making the reload cycle as seamless as possible.

Here is an example of an application entry point that merely consists of a textbox:

```csharp
// required to give GUAPR a control
[assembly: HostedTargetType(typeof(ExampleWatchEntryPoint))]

// required to give GUAPR a control
public class ExampleWatchEntryPoint : HostedEntryPoint
{
  // this is where we load the control that we want to show and load any saved
  // data from our configuration dictionary
  public override FrameworkElement Initialize(Dictionary<string, string> configuration)
  {
    // If we have the saved text from last time, use it, otherwise use the default text
    if (!configuration.TryGetValue("message", out string message))
      message = "No Message Loaded";

    return new TextBox()
            {
              Text = message
            };
  }

  // this gets invoked before we unload the app, and allows us to save any data
  // to the configuration dictionary so that the next time we load, we can load
  // the app right to where we were before
  public override void Shutdown(FrameworkElement originalInstance, Dictionary<string, string> configuration)
  {
    var textBox = (TextBox)originalInstance;
    // save the message that currently have in the textbox
    configuration["message"] = textBox.Text;
  }
}
```

If you add this to a an empty project, you'll notice that when run from GUAPR, the text from your textbox is maintained even when you recompile the project.