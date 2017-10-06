using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Guapr.App
{
  /// <summary> Allows adding a entry to the app system menu for a wpf app. </summary>
  public class SystemMenuExtender
  {
    private readonly Window _owner;

    private readonly List<EventHandler> _handlers;

    /// <summary> The start id for new menus added to the system menu. </summary>
    private const int BaseUserId = 1000;

    private bool _isInitialized = false;

    /// <summary> Constructor. </summary>
    /// <param name="owner"> The window which owns the menu extender. </param>
    public SystemMenuExtender(Window owner)
    {
      _owner = owner;
      _handlers = new List<EventHandler>();
    }

    /// <summary> Gets the handle for the window. </summary>
    /// <returns> The handle to the window. </returns>
    private IntPtr GetHandle() 
      => new WindowInteropHelper(_owner).Handle;

    /// <summary> Adds a new menu item to the system menu. </summary>
    /// <param name="text"> The text on the menu. </param>
    /// <param name="handler"> The handler that gets invoked when the menu item is clicked. </param>
    public void Add(string text, EventHandler handler)
    {
      Initialize();

      var systemMenuHandle = GetSystemMenu(GetHandle(), false);

      var position = 6 + _handlers.Count;
      var userId = BaseUserId + _handlers.Count;

      _handlers.Add(handler);

      InsertMenu(systemMenuHandle, position, MF_BYPOSITION, userId, text);
    }

    /// <summary> Must be called before any item is added. </summary>
    private void Initialize()
    {
      if (_isInitialized)
        return;

      var systemMenuHandle = GetSystemMenu(GetHandle(), false);

      // All menu items are prefaced by one separator
      InsertMenu(systemMenuHandle, 5, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty);

      // Attach our WndProc handler to this Window
      var source = HwndSource.FromHwnd(GetHandle());
      source.AddHook(WndProc);

      _isInitialized = true;
    }

    /// <summary> Hook into WndProc method in order to intercept menu invocations. </summary>
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      // Check if a System Command has been executed
      if (msg == WM_SYSCOMMAND)
      {
        var customId = wParam.ToInt32();

        if (customId >= BaseUserId && customId < BaseUserId + _handlers.Count)
        {
          _handlers[customId - BaseUserId].Invoke(_owner, EventArgs.Empty);
        }
      }

      return IntPtr.Zero;
    }


    #region Win32 Stuff

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    public const int WM_SYSCOMMAND = 0x112;
    public const int MF_SEPARATOR = 0x800;
    public const int MF_BYPOSITION = 0x400;
    public const int MF_STRING = 0x0;

    [DllImport("user32.dll")]
    private static extern bool InsertMenu(IntPtr hMenu, int wPosition, int wFlags, int wIDNewItem, string lpNewItem);

    #endregion
  }
}