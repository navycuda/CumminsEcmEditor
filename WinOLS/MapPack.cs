using CumminsEcmEditor.Tools;
using CumminsEcmEditor.Tools.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CumminsEcmEditor.WinOLS
{
    [Serializable]
    public class MapPack
    {
        public Map[] maps { get; set; }

        public MapPack() { }
        public MapPack(string mapPath) =>
            ProcessMapPack(EcmFiles.Load(mapPath));

        private void ProcessMapPack(string[] json)
        {
            string raw = string.Concat(json);
            JsonSerializerOptions jsonOptions = new()
            {
                IncludeFields = true,
            };
            maps = JsonSerializer.Deserialize<MapPack>(raw)
                                 .maps
                                 .OrderBy(m => m.GetId())
                                 .ToArray();
        }
    }
}