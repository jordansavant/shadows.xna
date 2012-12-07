using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Ai.Pathfinding;
using TDGameLibrary.Components;
using TDGameLibrary.Input;
using TDGameLibrary.Utility;
using TDGameLibrary.Audio;
using TDGameLibrary.XnaComponents;
using Microsoft.Xna.Framework.GamerServices;
using TDGameLibrary.Storage;

namespace TDGameLibrary
{
    public class GameEnvironment : DrawableGameComponent
    {
        private GameEnvironment(Game game, ScreenManager screenManager, StorageManager storageManager, GraphicsDeviceManager graphicsDevice)
            : base(game)
        {
            Game = game;
            GameScreenManager = screenManager;
            StorageManager = storageManager;
            ScreenRectangle = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            
            GameStartTime = DateTime.Now;
            Random = new Random();

            Game.Components.Add(this);
            Game.Components.Add(screenManager);
            Game.Components.Add(storageManager);
            AddGlobalComponents();
        }

        private static GameEnvironment _gameManager;
        public static void StartServices(Game game, ScreenManager screenManager, StorageManager storageManager, GraphicsDeviceManager graphicsDevice)
        {
            if (_gameManager == null)
            {
                _gameManager = new GameEnvironment(game, screenManager, storageManager, graphicsDevice);
            }
        }

        private void AddGlobalComponents()
        {
            Game.Components.Add(new InputManager(Game));
        }

        public override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            SoundManager.Update(gameTime);
            TotalRuntime = DateTime.Now - GameStartTime;

            base.Update(gameTime);
        }

        public static new Game Game { get; private set; }
        public static DateTime GameStartTime { get; private set; }
        public static TimeSpan TotalRuntime { get; private set; }
        public static ScreenManager GameScreenManager { get; private set; }
        public static StorageManager StorageManager { get; private set; }
        public static GameTime GameTime { get; private set; }
        public static Rectangle ScreenRectangle { get; private set; }
        public static Random Random { get; private set; }
        public static EventHandler<PlayerIndexEventArgs> OnPlayerIndexChange;
        private static PlayerIndex _playerIndex = PlayerIndex.One;
        public static PlayerIndex PlayerIndex
        {
            get
            {
                return _playerIndex;
            }
            set
            {
                _playerIndex = value;
                if (OnPlayerIndexChange != null)
                {
                    OnPlayerIndexChange.Invoke(null, new PlayerIndexEventArgs(value));
                }
            }
        }
        public static bool IsDebugModeActive;
        public static bool IsDeveloperMode;

        public const int WorldLayerMinimum = 0;
        public const int WorldLayerMaximum = 6;
        private static int _worldLayerDefault = 16;
        public static int WorldLayerDefault
        {
            get
            {
                return _worldLayerDefault;
            }
            set
            {
                _worldLayerDefault = (int)MathHelper.Clamp(value, WorldLayerMinimum, WorldLayerMaximum);
            }
        }

        public static int GamePlayManagerDrawOrder = 1000;
        public static int ParticleManagerDrawOrder = 1100;
        public static int DrawUtilityDrawOrder = 2000;
        public static int MobInfoUtilityDrawOrder = 2000;
        public static int FrameRateCounterDrawOrder = 2100;
        public static int MousePointerDrawOrder = 3000;

        public static bool StopMobSpawns; //could move this to GamePlayManager but right now I don't care, lol -092912ss
    }
}
