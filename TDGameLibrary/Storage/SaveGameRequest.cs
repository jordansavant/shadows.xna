using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Storage
{
    public class SaveGameRequest
    {
        public SaveGameRequest(string containerName, string saveName, Type saveType, object saveData)
        {
            ContainerName = containerName;
            SaveName = saveName;
            SaveType = saveType;
            SaveData = saveData;
        }

        public string SaveName;
        public string ContainerName;
        public Type SaveType;
        public object SaveData;
    }
}
