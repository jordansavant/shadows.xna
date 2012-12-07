using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace TDGameLibrary.Map
{
    public class UnboundedMapEngine : ChunkedMapEngine
    {
        public UnboundedMapEngine(Rectangle ScreenRectangle, ChunkedMap map, MapCamera mapCamera, int threadSleepMilliseconds, bool isRepeatRandom)
            : base(ScreenRectangle, map, mapCamera, threadSleepMilliseconds)
        {
            IsRepeatRandom = isRepeatRandom;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            // Go ahead and load all chunks into memory.
            for (int i = 0; i < ChunkFilePositions.Length; i++)
            {
                LoadChunk(i);
            }
        }

        private bool IsRepeatRandom;

        // ChunkIndex can be a negative number
        public override void LoadChunk(int chunkIndex)
        {
            if (IsRepeatRandom)
            {
                // Jump to a random chunk, if chunk not cool
                if (chunkIndex >= ChunkFilePositions.Length || chunkIndex < 0)
                    chunkIndex = GameEnvironment.Random.Next(ChunkFilePositions.Length);

            }
            else
            {
                // Jump to repeat map
                if (chunkIndex >= ChunkFilePositions.Length || chunkIndex < 0)
                {
                    chunkIndex = chunkIndex % ChunkFilePositions.Length;
                }

                if (chunkIndex < 0)
                {
                    chunkIndex = (ChunkFilePositions.Length + chunkIndex);
                }
            }

            if (!ChunkTextures.ContainsKey(chunkIndex))
            {
                base.LoadChunk(chunkIndex);
            }
        }


        public override void UnloadChunk(int chunkIndex)
        {
            // Do not unload our chunks
        }


        protected override Vector2 LimitChunkIndices(int x, int y)
        {
            return new Vector2(x, y); // no limitation
        }

        protected override bool EnsureChunkDrawable(ref int chunkIndex)
        {
            if (IsRepeatRandom)
            {
                // Ensure randomly loaded chunk is valid
                chunkIndex = Math.Abs(chunkIndex % (ChunkTextures.Count() - 1));
            }
            else
            {
                // Ensure wrapped chunk is valid
                if (chunkIndex >= ChunkFilePositions.Length || chunkIndex < 0)
                {
                    chunkIndex = chunkIndex % ChunkFilePositions.Length;
                }

                if (chunkIndex < 0)
                {
                    chunkIndex = (ChunkFilePositions.Length + chunkIndex);
                }
            }

            return true; // no matter the index, draw the chunks
        }
    }
}
