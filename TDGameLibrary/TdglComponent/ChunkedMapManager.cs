using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Map;
using Microsoft.Xna.Framework;
using TDGameLibrary;
using System.Threading;

namespace TDGameLibrary.Components
{
    public class ChunkedMapManager : TdglComponent
    {
        public ChunkedMapManager(WorldManager worldManager, ChunkedMap chunkedMap, MapCamera mapCamera, int threadSleepMilliseconds, SpriteBatch spriteBatch)
        {
            WorldManager = worldManager;
            ChunkedMap = chunkedMap;
            MapSpriteBatch = spriteBatch;
            MapCamera = mapCamera;

            ChunkedMapEngine = new UnboundedMapEngine(GameEnvironment.ScreenRectangle, ChunkedMap, MapCamera, threadSleepMilliseconds, false);
            MapEngineThread = new Thread(UpdateMapEngine);
            MapCamera.PanSpeed = 10f;
            ChunkedMapEngine.LoadContent();
        }

        private SpriteBatch MapSpriteBatch;
        private Thread MapEngineThread;
        public ChunkedMap ChunkedMap { get; set; }
        public WorldManager WorldManager { get; private set; }
        public ChunkedMapEngine ChunkedMapEngine { get; private set; }
        public MapCamera MapCamera { get; private set; }

        public override void Update(GameTime gameTime)
        {
            if (MapEngineThread.ThreadState == ThreadState.Unstarted)
            {
                MapEngineThread.Start();
            }

            ChunkedMapEngine.MapCamera.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            MapSpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                Matrix.Identity);

            ChunkedMapEngine.Draw(MapSpriteBatch);

            MapSpriteBatch.End();

            base.Draw(gameTime);
        }

        public void UpdateMapEngine()
        {
            #if XBOX
                MapEngineThread.SetProcessorAffinity(3);
            #endif

            while (true)
            {
                ChunkedMapEngine.Update();
            }
        }

        public override void Destruct()
        {
            ChunkedMapEngine.Destruct();
            MapEngineThread.Abort();
            base.Destruct();
        }
    }
}
