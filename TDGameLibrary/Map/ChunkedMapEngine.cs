using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System;

namespace TDGameLibrary.Map
{
    public class ChunkedMapEngine
    {
        public ChunkedMapEngine(Rectangle ScreenRectangle, ChunkedMap map, MapCamera mapCamera, int threadSleepMilliseconds)
        {
            Map = map;

            MaxChunkIndices = new Vector2((Map.MapRectangle.Width / Map.ChunkSize.X) - 1, (Map.MapRectangle.Height / Map.ChunkSize.Y) - 1);

            ExclusiveContentManager = new ExclusiveContentManager(GameEnvironment.Game.Content.ServiceProvider, GameEnvironment.Game.Content.RootDirectory);

            MapCamera = mapCamera;

            Chunks = new List<int>();

            ChunkIndices = new List<Vector2>();

            ChunkIndicesToDraw = new List<Vector2>();

            ChunkTextures = new Dictionary<int, Texture2D>();

            LastLoadedChunks = new List<int>();

            CentralChunkPosition = new Vector2();

            CentralChunkIndices = new Vector2();

            LastCentralChunkIndices = new Vector2();

            HasUpdated = false;

            ThreadSleepMilliseconds = threadSleepMilliseconds;
        }



        protected volatile bool HasUpdated;

        protected ExclusiveContentManager ExclusiveContentManager;

        public ChunkedMap Map;
        public MapCamera MapCamera;
        protected Vector2 MaxChunkIndices;
        public int MaxChunk
        {
            get
            {
                return ConvertToLinearIndex((int)MaxChunkIndices.X, (int)MaxChunkIndices.Y);
            }
        }
        private int ThreadSleepMilliseconds;


        protected List<int> Chunks;
        protected List<Vector2> ChunkIndices;
        protected List<Vector2> ChunkIndicesToDraw;

        protected Dictionary<int, Texture2D> ChunkTextures;

        protected List<int> LastLoadedChunks;
        protected Vector2 CentralChunkPosition;
        protected Vector2 CentralChunkIndices;
        protected Vector2 LastCentralChunkIndices;

        protected FileStream MapFileStream;
        protected BinaryReader MapFileBinaryReader;


        protected int ChunkCount = 0;
        protected int[] ChunkFilePositions;
        public virtual void LoadContent()
        {
            // Open the map file
            MapFileStream = File.Open(ExclusiveContentManager.RootDirectory + @"\" + Map.MapFilename, FileMode.Open, FileAccess.Read);

            // Load Map File headers
            MapFileBinaryReader = new BinaryReader(MapFileStream);
            ChunkCount = MapFileBinaryReader.ReadInt32();
            ChunkFilePositions = new int[ChunkCount];

            for (int i = 0; i < ChunkCount; i++)
            {
                ChunkFilePositions[MapFileBinaryReader.ReadInt32()] = MapFileBinaryReader.ReadInt32();
            }
        }



        protected IEnumerable<int> ChunksToKeep;
        public virtual void Update()
        {
            // Update our list of chunks we want to load
            LastLoadedChunks.Clear();
            foreach (int chunk in Chunks)
            {
                LastLoadedChunks.Add(chunk);
            }


            // Store last located Central Chunk, and update.
            LastCentralChunkIndices.X = CentralChunkIndices.X;
            LastCentralChunkIndices.Y = CentralChunkIndices.Y;
            UpdateCentralChunk();


            // If the chunk has not updated, there is no need to recalculate relative chunks to load.
            if (!HasUpdated || LastCentralChunkIndices.X != CentralChunkIndices.X || LastCentralChunkIndices.Y != CentralChunkIndices.Y)
            {
                // Update which chunks to load.
                UpdateChunks();


                // Get the chunks that are already loaded to avoid reloading them.
                ChunksToKeep = LastLoadedChunks.Intersect(Chunks);


                // Load all the chunks that are not already loaded.
                foreach (int chunkToLoad in Chunks)
                {
                    if (!ChunksToKeep.Contains(chunkToLoad))
                    {
                        LoadChunk(chunkToLoad);
                    }
                }


                // Copy our new chunk indices to our drawable list.
                lock (this)
                {
                    ChunkIndicesToDraw.Clear();
                    foreach (Vector2 chunkIndex in ChunkIndices)
                    {
                        ChunkIndicesToDraw.Add(chunkIndex);
                    }
                }


                // Unload the chunks that aren't needed any more.
                foreach (int loadedChunk in LastLoadedChunks)
                {
                    if (!Chunks.Contains(loadedChunk))
                    {
                        UnloadChunk(loadedChunk);
                    }
                }
            }

            HasUpdated = true;

            // Sleep our update to keep Draw from starving.
            Thread.Sleep(ThreadSleepMilliseconds);
        }


        protected Vector2 workingChunkPosition = new Vector2();
        public void Draw(SpriteBatch spriteBatch)
        {
            // Update is threaded so don't draw unless we have updated at least once.
            if (!HasUpdated) 
                return;

            lock (this)
            {
                foreach (Vector2 chunkIndex in ChunkIndicesToDraw)
                {
                    int drawingLinearIndex = ConvertToLinearIndex((int)chunkIndex.X, (int)chunkIndex.Y);

                    workingChunkPosition = (ConvertChunkIndicesToMapPosition((int)chunkIndex.X, (int)chunkIndex.Y)- MapCamera.Origin) * Map.ParallaxFactor;

                    if (EnsureChunkDrawable(ref drawingLinearIndex))
                    {
                        spriteBatch.Draw(ChunkTextures[drawingLinearIndex], workingChunkPosition, null, Map.Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
                    }
                }
            }
        }


        protected virtual bool EnsureChunkDrawable(ref int chunkIndex)
        {
            return ChunkTextures.ContainsKey(chunkIndex);
        }



        protected void UpdateCentralChunk()
        {
            // Get central chunk position by rounding up and left to chunk size.
            CentralChunkPosition.X = (int)Math.Ceiling(MapCamera.Center.X - (MapCamera.Center.X % Map.ChunkSize.X));
            CentralChunkPosition.Y = (int)Math.Ceiling(MapCamera.Center.Y - (MapCamera.Center.Y % Map.ChunkSize.Y));

            // Convert our map coordinates to chunk indices.
            CentralChunkIndices.X = (int)(CentralChunkPosition.X / Map.ChunkSize.X);
            CentralChunkIndices.Y = (int)(CentralChunkPosition.Y / Map.ChunkSize.Y);
        }



        protected void UpdateChunks()
        {
            // Clear our list to make it only hold enough as needed.
            Chunks.Clear();
            ChunkIndices.Clear();

            // From our primary chunk determine all surrounding chunks that must be loaded. Plus a couple for buffering and if ints are odd.
            HorizontalChunkCount = (int)(GameEnvironment.ScreenRectangle.Width / Map.ChunkSize.X);
            VerticalChunkCount = (int)(GameEnvironment.ScreenRectangle.Height / Map.ChunkSize.Y);
            int negXpad = -2;
            int posXpad = 5;
            int negYpad = -2;
            int posYpad = 3;

            // Collect all chunks that need to be loaded to fit the camera. This includes the already located Central Chunk.
            // Int division favors upper left coordinates, to pad the lower right with 1.
            for (int x = (int)(CentralChunkIndices.X - (HorizontalChunkCount / 2)) + negXpad; x <= (int)(CentralChunkIndices.X + (HorizontalChunkCount / 2)) + posXpad; x++)
            {
                for (int y = (int)(CentralChunkIndices.Y - (VerticalChunkCount / 2)) + negYpad; y <= (int)(CentralChunkIndices.Y + (VerticalChunkCount / 2)) + posYpad; y++)
                {
                    Vector2 LimitedChunkIndices = LimitChunkIndices(x, y);
                    ChunkIndices.Add(LimitedChunkIndices);

                    // Convert to a linear index.
                    workingLinearIndex = ConvertToLinearIndex((int)LimitedChunkIndices.X, (int)LimitedChunkIndices.Y);

                    // Add the linear index to a list identifying chunks that should be loaded.
                    Chunks.Add(workingLinearIndex);
                }
            }
        }
        protected int HorizontalChunkCount = 0;
        protected int VerticalChunkCount = 0;
        protected int workingLinearIndex = 0;



        protected virtual Vector2 LimitChunkIndices(int x, int y)
        {
            return new Vector2(MathHelper.Clamp(x, 0, MaxChunkIndices.X), MathHelper.Clamp(y, 0, MaxChunkIndices.Y));
        }

        protected Vector2 ConvertChunkIndicesToMapPosition(int x, int y)
        {
            return new Vector2((x * Map.ChunkSize.X), (y * Map.ChunkSize.Y));
        }

        protected int ConvertToLinearIndex(int x, int y)
        {
            return (int)(x + (y * Map.RowsColumns.Y));
        }



        public virtual void LoadChunk(int chunkIndex)
        {
            // Jump to our image location in our map file.
            MapFileStream.Seek(ChunkFilePositions[chunkIndex], SeekOrigin.Begin);

            // Calculate how many bytes this image consists of.
            int byteCountSize = 0;
            if (chunkIndex == ChunkCount - 1)
            {
                byteCountSize = (int)MapFileStream.Length - ChunkFilePositions[chunkIndex];
            }
            else
            {
                byteCountSize = ChunkFilePositions[chunkIndex + 1] - ChunkFilePositions[chunkIndex];
            }

            // Read our image into a Memory Stream from the map file.
            using (MemoryStream ms = new MemoryStream(byteCountSize))
            {
                var bytes = new byte[byteCountSize];

                MapFileStream.Read(bytes, 0, byteCountSize);
                ms.Write(bytes, 0, byteCountSize);
                ms.Seek(0, SeekOrigin.Begin);

                // Pass the Memory Stream into our Texture2D loader and update our list of map textures.
                Texture2D t = Texture2D.FromStream(GameEnvironment.Game.GraphicsDevice, ms, (int)(Map.ChunkSize.X * Map.ParallaxFactor), (int)(Map.ChunkSize.Y * Map.ParallaxFactor), false);
                ChunkTextures[chunkIndex] = t;
            }

        }


        public virtual void UnloadChunk(int chunkIndex)
        {
            // Unload our chunk from our list.
            if (ChunkTextures.ContainsKey(chunkIndex))
            {
                ChunkTextures[chunkIndex].Dispose();
                ChunkTextures.Remove(chunkIndex);
            }
        }



        public void Destruct()
        {
            // Close our connection to the map file.
            MapFileBinaryReader.Close();
            MapFileStream.Close();
        }
    }
}
