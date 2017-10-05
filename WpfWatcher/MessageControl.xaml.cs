using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfWatcher
{
  /// <summary>
  /// Interaction logic for WaitingForChangesControl.xaml
  /// </summary>
  public partial class MessageControl : UserControl
  {
    public MessageControl()
    {
      InitializeComponent();
    }

    public string Message
    {
      get { return (string)MessageHolder.Content; }
      set { MessageHolder.Content = value; }
    }

    public string AdditionalInformation
    {
      get { return AdditionalInformationControl.Text; }
      set
      {
        AdditionalInformationControl.Text = value;
        var visibility = string.IsNullOrWhiteSpace(value)
          ? Visibility.Hidden
          : Visibility.Visible;

        CopyToClipboardButton.Visibility = visibility;
        AdditionalInformationControl.Visibility = visibility;
      }
    }

    private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(AdditionalInformationControl.Text);
    }

    private void ResetApplicationButton_Click(object sender, RoutedEventArgs e)
    {
      var mainWindow = (MainWindow)Window.GetWindow(this);
      mainWindow.Reset();
    }
  }
}