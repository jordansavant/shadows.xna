using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TDGameLibrary.Sound;


namespace TDGameLibrary
{
    public static class TDGL
    {
        public static Game Game { get; set; }

        private static GameProfile _gameProfile;
        public static GameProfile GameProfile
        {
            get
            {
                if (_gameProfile == null)
                {
                    _gameProfile = new GameProfile() { ContentManager = Game.Content, GraphicsDevice = Game.GraphicsDevice };
                }
                return _gameProfile;
            }
            set
            {
                _gameProfile = value;
            }
        }

        public static void Update(GameTime gameTime)
        {
            GameProfile.GameTime = gameTime;

            foreach (IGameService service in Services.Keys)
            {
                if (Services[service] == null)
                {
                    service.Unregister();
                }
                else
                {
                    service.Update();
                }
            }
        }

        public static void Draw(GameTime gameTime)
        {
            GameProfile.GameTime = gameTime; //TODO: unless GameTime is a snapshot, might someday use this to measure time between update and draw, etc
            RenderedServices.ForEach(delegate(IRenderedService s) { s.Draw(); });
        }

        private static Dictionary<IGameService, Object> _services;
        public static Dictionary<IGameService, Object> Services { get { return _services ?? (_services = new Dictionary<IGameService, Object>()); } }

        internal static void Register(this IGameService gameService, Object host)
        {
            Services.Add(gameService, host);
            if (gameService.GetType() == typeof(IRenderedService))
            {
                RenderedServices.Add((IRenderedService)gameService);
            }
        }

        internal static void Unregister(this IGameService gameService)
        {
            Services.Remove(gameService);
            if (gameService.GetType() == typeof(IRenderedService))
            {
                RenderedServices.Remove((IRenderedService)gameService);
            }
        }

        private static List<IRenderedService> _renderedServices;
        private static List<IRenderedService> RenderedServices { get { return _renderedServices ?? (_renderedServices = new List<IRenderedService>()); } }

    }
}
