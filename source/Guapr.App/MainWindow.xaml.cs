using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Guapr.App.Properties;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Guapr.App
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private ControlWatcher _watcher;
    private Dictionary<string, AssemblyConfiguration> _configurations;

    public MainWindow()
    {
      InitializeComponent();

      LoadSettings();

      Closed += HandleClosed;

      AllowDrop = true;
      DragEnter += OnDragEnter;
      Drop += OnDrop;

      Loaded += delegate
                {
                  var systemMenu = new SystemMenuExtender(this);
                  systemMenu.Add("&Reload Client", (_1, _2) => Reload());
                  systemMenu.Add("&Clear && Reset Application", (_1, _2) => Reset());
                };
    }

    private void OnDragEnter(object sender, DragEventArgs e)
    {
      var file = GetFileDrop(e);
      if (file == null)
        return;

      e.Effects = DragDropEffects.Copy;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
      var file = GetFileDrop(e);
      if (file == null)
        return;

      PathToAssembly.Text = file;
      WatchClicked(null, null);
    }

    private string GetFileDrop(DragEventArgs args)
    {
      var files = (string[])args.Data.GetData(DataFormats.FileDrop);

      if (files == null || files.Length <= 0)
        return null;

      return (
        from file in files
        let ext = Path.GetExtension(file)?.ToLower()
        where ext == ".dll" || ext == ".exe"
        select file
        ).FirstOrDefault();
    }

    private void LoadSettings()
    {
      _configurations =
        JsonConvert.DeserializeObject<Dictionary<string, AssemblyConfiguration>>(Settings.Default.Configurations ?? "")
        ?? new Dictionary<string, AssemblyConfiguration>();

      Height = Settings.Default.Height;
      Width = Settings.Default.Width;
      PathToAssembly.Text = Settings.Default.LastAssemblyPath;
    }

    private void SaveSettings()
    {
      Settings.Default.LastAssemblyPath = PathToAssembly.Text;
      Settings.Default.Height = (int)Height;
      Settings.Default.Width = (int)Width;
      Settings.Default.Configurations = JsonConvert.SerializeObject(_configurations);
      Settings.Default.Save();
    }

    private void HandleClosed(object sender, EventArgs e)
    {
      _watcher?.Dispose();
      SaveSettings();
    }

    private void BrowseButtonClicked(object sender, RoutedEventArgs e)
    {
      var openFileDialog = new OpenFileDialog()
                           {
                             Filter = "Assemblies|*.exe;*.dll"
                           };

      if (true == openFileDialog.ShowDialog()
          && File.Exists(openFileDialog.FileName))
      {
        PathToAssembly.Text = openFileDialog.FileName;
      }
    }

    private void WatchClicked(object sender, RoutedEventArgs e)
    {
      var pathToAssembly = PathToAssembly.Text;
      if (!File.Exists(pathToAssembly))
        return;

      Reset();

      try
      {
        // TODO update this so that it doesn't keep ALL configurations
        AssemblyConfiguration configuration;
        if (!_configurations.TryGetValue(pathToAssembly, out configuration))
        {
          configuration = new AssemblyConfiguration()
                          {
                            PathToAssembly = pathToAssembly
                          };
          _configurations[configuration.PathToAssembly] = configuration;
        }

        Environment.CurrentDirectory = Path.GetDirectoryName(pathToAssembly);
        _watcher = new ControlWatcher(configuration);
      }
      catch (Exception exception)
      {
        MessageBox.Show($"Error Loading Assembly:\r\n    {pathToAssembly}\r\n > {exception.Message}\r\n\r\nLoading has been canceled",
                        "Error Loading Assembly",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
        return;
      }

      LoadAssemblyInput.Visibility = Visibility.Collapsed;
      HostedControl.Visibility = Visibility.Visible;

      Host.Content = _watcher;

      SaveSettings();
    }

    internal void Reset()
    {
      LoadAssemblyInput.Visibility = Visibility.Visible;
      HostedControl.Visibility = Visibility.Collapsed;

      (Host.Content as ControlWatcher)?.Dispose();

      Host.Content = null;
    }

    internal void Reload()
    {
      (Host.Content as ControlWatcher)?.Reload();
    }

    private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
    {
      Process.Start("https://bitbucket.org/zastrowm/.net-guapr/overview");
    }
  }
}