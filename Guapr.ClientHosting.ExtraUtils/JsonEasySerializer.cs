using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using Guapr.ClientHosting.Internal;

namespace Guapr.ClientHosting.ExtraUtils
{
  /// <summary> Serializes state using JSON.net . </summary>
  [DomainUsage(DomainId.Client, CreatedBy = DomainId.Server, LoadedBy = DomainId.Server)]
  public class JsonEasySerializer : IEasySerializer
  {
    private readonly IStateDirectoryOwner _directoryOwner;

    public JsonEasySerializer(IStateDirectoryOwner directoryOwner)
    {
      _directoryOwner = directoryOwner;
    }

    /// <inheritdoc />
    public bool SaveState<T>(string name, T state)
    {
      name = GetSafeName(name);

      try
      {
        var file = GetSessionFile(name);
        var serializer = new JsonSerializer();

        using (var writer = new StreamWriter(file.Open(FileMode.Create)))
        {
          serializer.Serialize(writer, state);
          return true;
        }
      }
      catch (Exception e)
      {
        EntryPointUtilities.LogException($"Saving state with name «{name}»", e);
        return false;
      }
    }

    /// <inheritdoc />
    public bool LoadState<T>(string name, out T state)
    {
      name = GetSafeName(name);

      try
      {
        var file = GetSessionFile(name);
        if (!file.Exists)
        {
          state = default(T);
          return false;
        }

        using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read)))
        using (var jsonReader = new JsonTextReader(reader))
        {
          var serializer = new JsonSerializer();
          state = serializer.Deserialize<T>(jsonReader);
          return true;
        }
      }
      catch (Exception e)
      {
        EntryPointUtilities.LogException($"Loading state with name «{name}»", e);

        state = default(T);
        return false;
      }
    }

    private static string GetSafeName(string name)
    {
      if (name == null)
        return "";

      return "." + Convert.ToBase64String(Encoding.UTF8.GetBytes(name));
    }

    private FileInfo GetSessionFile(string name)
      => new FileInfo(Path.Combine(_directoryOwner.StateDirectory.FullName, $".__session.state{name}__"));
  }
}
