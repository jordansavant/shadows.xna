using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShadowShootout.Components;
using TDGameLibrary;
using TDGameLibrary.Components;
using TDGameLibrary.Map;
using TDGameLibrary.Mobs;
using TDGameLibrary.Utility;
using TDGameLibrary.XnaComponents;

namespace ShadowShootout
{
    public class GamePlayManager
    {
        public static TdglComponentManager GamePlayComponentManager { get; private set; }
        public static UiComponentManager UiManager { get; private set; }
        public static MobManager MobManager { get; private set; }
        public static GhostBodyManager GhostBodyManager { get; private set; }
        public static PooledBodyManager PooledBodyManager { get; private set; }
        public static WorldManager WorldManager { get; private set; }
        public static ChunkedMapManager MapManager { get; private set; }
        public static MapCamera MapCamera { get; private set; }
        public static DrawUtility DrawUtility { get; private set; }
        public static SpriteBatch SpriteBatch;

        public static void StartServices()
        {
            if (GamePlayComponentManager != null)
            {
                throw new InvalidOperationException("Service already started.");
            }
            else
            {
                // Top level services
                UiManager = new UiComponentManager(SpriteBatch) { IsPauseIgnored = true };
                GamePlayComponentManager = new TdglComponentManager(GameEnvironment.Game, UiManager);
                WorldManager = GamePlayComponentManager.AddComponent(new WorldManager(new World(Vector2.Zero)));
                GhostBodyManager = GamePlayComponentManager.AddComponent(new GhostBodyManager());
                PooledBodyManager = GamePlayComponentManager.AddComponent(new PooledBodyManager(WorldManager, GhostBodyManager));
                MobManager = GamePlayComponentManager.AddComponent(new MobManager(WorldManager));
                DrawUtility = GamePlayComponentManager.AddComponent(new DrawUtility(SpriteBatch));
            }
        }

        public static void StopServices()
        {
            if (GamePlayComponentManager != null)
            {
                GamePlayComponentManager.Destruct();
            }
        }

        public static void GameOver()
        {
        }

        public static void SaveTheGame()
        {
        }
    }
}
