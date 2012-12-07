using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Storage
{
    public class LoadGameRequest
    {
        public LoadGameRequest(string containerName, string saveName, Type saveType)
        {
            ContainerName = containerName;
            SaveName = saveName;
            SaveType = saveType;
            IsLoaded = false;
            LoadFailed = false;
            Data = null;
        }

        public string SaveName;
        public string ContainerName;
        public Type SaveType;
        public volatile bool IsLoaded;
        public volatile bool LoadFailed;
        public Exception LoadFailException;
        public object Data;
    }
}
