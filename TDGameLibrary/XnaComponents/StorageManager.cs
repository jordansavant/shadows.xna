using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using TDGameLibrary.Storage;
using System.Threading;

namespace TDGameLibrary.XnaComponents
{
    public class StorageManager : GameComponent
    {
        public StorageManager(Game game, int threadSleepMilliseconds)
            : base(game)
        {
            game.Components.Add(new GamerServicesComponent(game));
            ThreadSleepMilliseconds = threadSleepMilliseconds;
            SaveGameRequests = new Queue<SaveGameRequest>();
            LoadGameRequests = new Queue<LoadGameRequest>();
            StorageThread = new Thread(UpdateStorageRequests);
        }

        public EventHandler OnDeviceSelectionCancel;
        public EventHandler OnGamerPrivilegeExceptionWhileLoading;
        public bool IsDeviceSelectionCancelled;

        private int ThreadSleepMilliseconds;
        private Thread StorageThread;
        private Object StateObject;
        private StorageDevice StorageDevice;
        private PlayerIndex _playerIndex = PlayerIndex.One;
        public PlayerIndex PlayerIndex
        {
            get
            {
                return _playerIndex;
            }
            set
            {
                // If we are updating the player, reset storage device
                if (_playerIndex != value)
                {
                    RequestStorageDevice = true;
                }
                _playerIndex = value;
            }
        }

        private Queue<SaveGameRequest> SaveGameRequests;
        private Queue<LoadGameRequest> LoadGameRequests;

        private volatile bool IsStorageDeviceLoaded;
        private volatile bool IsStorageDeviceLoading;
        private volatile bool RequestStorageDevice;


        public override void Update(GameTime gameTime)
        {
            if (StorageThread.ThreadState == ThreadState.Unstarted)
            {
                StorageThread.Start();
            }

            base.Update(gameTime);
        }


        private void UpdateStorageRequests()
        {
#if XBOX
            StorageThread.SetProcessorAffinity(4);
#endif

            SaveGameRequest sg = null;
            LoadGameRequest lg = null;
            while (true)
            {
                if (RequestStorageDevice)
                {
                    RequestDevice();
                    RequestStorageDevice = false;
                }

                if (IsStorageDeviceLoaded)
                {
                    lock (this)
                    {
                        if (SaveGameRequests.Count > 0)
                        {
                            sg = SaveGameRequests.Dequeue();
                        }
                    }
                    Thread.Sleep(ThreadSleepMilliseconds);

                    if (sg != null)
                    {
                        if (!DoSave(sg))
                        {
                            lock (this)
                            {
                                SaveGameRequests.Enqueue(sg);
                            }
                            Thread.Sleep(ThreadSleepMilliseconds);
                        }
                        else
                        {
                            sg = null;
                        }
                    }


                    lock (this)
                    {
                        if (LoadGameRequests.Count > 0)
                        {
                            lg = LoadGameRequests.Dequeue();
                        }
                    }
                    Thread.Sleep(ThreadSleepMilliseconds);

                    if (lg != null)
                    {
                        if (!DoLoad(lg))
                        {
                            lock (this)
                            {
                                LoadGameRequests.Enqueue(lg);
                            }
                            Thread.Sleep(ThreadSleepMilliseconds);
                        }
                        else
                        {
                            lg = null;
                        }
                    }
                }
            }
        }


        public void Save(SaveGameRequest saveGameRequest)
        {
            lock (this)
            {
                SaveGameRequests.Enqueue(saveGameRequest);
            }

            if (!IsStorageDeviceLoaded && !IsStorageDeviceLoading)
            {
                RequestStorageDevice = true;
            }
        }


        public void Load(LoadGameRequest loadGameRequest)
        {
            lock (this)
            {
                LoadGameRequests.Enqueue(loadGameRequest);
            }

            if (!IsStorageDeviceLoaded && !IsStorageDeviceLoading)
            {
                RequestStorageDevice = true;
            }
        }


        private void RequestDevice()
        {
            if (!Guide.IsVisible)
            {
                IsStorageDeviceLoaded = false;
                IsStorageDeviceLoading = true;
                StorageDevice = null;
                StateObject = (Object)"Get Device for Player";
                StorageDevice.BeginShowSelector(PlayerIndex, GetDevice, StateObject);
            }
        }


        // Callback method for when the player chooses the device.
        private void GetDevice(IAsyncResult result)
        {
            lock (this)
            {
                // Close the menu and get the result.
                StorageDevice = StorageDevice.EndShowSelector(result);

                // If no storage device was chosen:
                if (StorageDevice == null)
                {
                    // Notify listeners.
                    IsDeviceSelectionCancelled = true;
                    if (OnDeviceSelectionCancel != null)
                    {
                        OnDeviceSelectionCancel.Invoke(this, EventArgs.Empty);
                    }
                }

                // If a device selected and the device is connected.
                if (StorageDevice != null && StorageDevice.IsConnected)
                {
                    IsStorageDeviceLoaded = true;
                }

                IsStorageDeviceLoading = false;
            }
        }



        private bool DoSave(SaveGameRequest saveGameRequest)
        {
            // If we have a storage device:
            if (StorageDevice != null && StorageDevice.IsConnected)
            {
                IAsyncResult result = StorageDevice.BeginOpenContainer(saveGameRequest.ContainerName, SaveCallback, saveGameRequest);

                return true;
            }

            return false;
        }



        private void SaveCallback(IAsyncResult result)
        {
            StorageContainer container = StorageDevice.EndOpenContainer(result);

            SaveGameRequest saveGameRequest = (SaveGameRequest)result.AsyncState;

            // Overwrite existing save.
            if (container.FileExists(saveGameRequest.SaveName))
            {
                container.DeleteFile(saveGameRequest.SaveName);
            }

            // Create the file.
            Stream stream = container.CreateFile(saveGameRequest.SaveName);

            // Convert the object to XML data and put it in the stream.
            XmlSerializer serializer = new XmlSerializer(saveGameRequest.SaveType);
            serializer.Serialize(stream, saveGameRequest.SaveData);

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
        }




        private bool DoLoad(LoadGameRequest loadGameRequest)
        {
            // If we have a storage device:
            if (StorageDevice != null && StorageDevice.IsConnected)
            {
                // Open a storage container.
                // CAN THROW GamerPrivilegeException If PlayerIndex not signed into a profile
                try
                {
                    IAsyncResult result = StorageDevice.BeginOpenContainer(loadGameRequest.ContainerName, LoadCallback, loadGameRequest);
                }
                catch (GamerPrivilegeException gpe)
                {
                    OnGamerPrivilegeExceptionWhileLoading.Invoke(gpe, EventArgs.Empty);
                }

                return true;
            }

            return false;
        }


        public void LoadCallback(IAsyncResult result)
        {
            StorageContainer container = StorageDevice.EndOpenContainer(result);

            LoadGameRequest loadGameRequest = (LoadGameRequest)result.AsyncState;

            // Check to see whether the save exists.
            if (!container.FileExists(loadGameRequest.SaveName))
            {
                // If not, dispose of the container and return.
                container.Dispose();
                loadGameRequest.LoadFailed = true;
                return;
            }

            try
            {
                // Open the file.
                Stream stream = container.OpenFile(loadGameRequest.SaveName, FileMode.Open);

                // Read the data from the file.
                XmlSerializer serializer = new XmlSerializer(loadGameRequest.SaveType);
                loadGameRequest.Data = serializer.Deserialize(stream);
                loadGameRequest.IsLoaded = true;

                // Close the file.
                stream.Close();
            }
            catch (Exception e)
            {
                loadGameRequest.LoadFailed = true;
                loadGameRequest.LoadFailException = e;
            }

            // Dispose the container.
            container.Dispose();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StorageThread.Abort();
            }

            base.Dispose(disposing);
        }
        
    }
}
