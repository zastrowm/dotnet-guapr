using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Guapr.ClientHosting.Internal;

namespace Guapr.ClientHosting
{
  /// <summary> Additional utilities that can be used by <see cref="IHostedEntryPoint"/> </summary>
  public static class EntryPointUtilities
  {
    /// <summary> Implementation-specific instance. </summary>
    internal static ExtraUtils Implementation { get; set; }

    /// <summary>
    ///  Creates a serializer that can be used to store state data without having to implement a
    ///  custom serialization routine.
    ///  
    ///  The serializer implementation is subject to change and shouldn't be assumed to be robust.  If
    ///  the simplicity of the serializer proof to be too simple, you should use
    ///  <see cref="IStateDirectoryOwner"/> to write data to a file using a different serialization
    ///  technique, such as JSON or XML serialization implemented in client code.
    /// </summary>
    /// <remarks>
    ///  Currently uses JSON.net, but may change depending on the version of the application/library
    ///  used.
    /// </remarks>
    /// <param name="directoryOwner"> The state directory owner to create a serializer from. </param>
    /// <returns> A serializer which can perform very simple serialization. </returns>
    public static IEasySerializer CreateSimpleSerializer(this IStateDirectoryOwner directoryOwner) 
      => Implementation.CreateEasySerializer(directoryOwner);

    /// <summary> Outputs a message to <see cref="Debug.WriteLine(string)"/>. </summary>
    public static void LogException(string operationBeingPerformed, Exception exception)
    {
      AlwaysDebug.DebugWriteLine(">> " + operationBeingPerformed + " :: " + exception);
    }

    /// <summary> Executes the given callback, catching any exceptions that occur. </summary>
    /// <param name="description"> The description of the operation being performed.  Will be passed
    ///  to <see cref="LogException"/> if an exception occurs. </param>
    /// <param name="callback"> The callback to invoke, and which might throw. </param>
    public static void SafeExecute(string description, Action callback)
    {
      try
      {
        callback.Invoke();
      }
      catch (Exception e)
      {
        LogException(description, e);
      }
    }
  }
}