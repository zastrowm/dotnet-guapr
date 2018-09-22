using System;
using System.Collections.Generic;
using System.Linq;

namespace Guapr.ClientHosting
{
  /// <summary> An object that can serialize to/from a <see cref="IStateDirectoryOwner"/>. </summary>
  public interface IEasySerializer
  {
    /// <summary>
    ///  Saves the given object to the <see cref="IStateDirectoryOwner"/> with the given name.
    /// </summary>
    /// <typeparam name="T"> The type of data to save </typeparam>
    /// <param name="name"> The name of the state to save. </param>
    /// <param name="state"> [out] The state which should be persisted. </param>
    /// <returns> True if the state was saved, false if it failed for some reason. </returns>
    bool SaveState<T>(string name, T state);

    /// <summary> Loads state with the given name from a <see cref="IStateDirectoryOwner"/>. </summary>
    /// <typeparam name="T"> The type of data to load. </typeparam>
    /// <param name="name"> The name of the state to load. </param>
    /// <param name="state"> [out] The state that loaded, if the method returns true. </param>
    /// <returns> True if the state was loaded, false if it failed for any reason. </returns>
    bool LoadState<T>(string name, out T state);
  }
}