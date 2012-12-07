using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TDGameLibrary.Map
{
    public class ExclusiveContentManager : ContentManager
    {
        public ExclusiveContentManager(IServiceProvider serviceProvider,
            string RootDirectory)
            : base(serviceProvider, RootDirectory)
        {
        }

        public T LoadContentExclusive<T>(string AssetName)
        {
            return ReadAsset<T>(AssetName, null);
        }

        public void Unload(IDisposable ContentItem)
        {
            ContentItem.Dispose();
        }
    }
}
