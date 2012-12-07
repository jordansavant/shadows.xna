using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Ui;
using System.Diagnostics;
using TDGameLibrary.DataStructures;
using TDGameLibrary.Components;
using Microsoft.Xna.Framework.GamerServices;

namespace TDGameLibrary.XnaComponents
{
    /// <summary>
    /// This class provides the following critical pieces of functionality:
    ///     1) It manages the lifecycle events (LoadContent, Draw, Update, etc) of custom, non-XNA game components.
    ///     2) It ensures the in-game UI always has a higher priority than all other game components, allowing the game to be paused.
    /// This class also simplifies management of Xna GameComponents that should exist only during gameplay by automatically
    /// calling Game.Components.Add(), Dispose(), etc.
    /// </summary>
    public class TdglComponentManager : DrawableGameComponent
    {
        public TdglComponentManager(Game game, UiComponentManager uiManager)
            : base(game)
        {
            //Game.Components.Add(this);
            
            _xnaComponents = new List<GameComponent>();
            _components = new MutableDistinctSet<TdglComponent>();

            _componentsSortedByDrawOrder = new List<TdglComponent>();
            _componentDrawOrders = new Dictionary<TdglComponent, int>();

            DrawOrder = GameEnvironment.GamePlayManagerDrawOrder;

            UiManager = AddComponent(uiManager);
            UiManager.DrawOrder = 1;
        }

        public UiComponentManager UiManager { get; private set; }
        public bool IsGamePaused;
        private bool ComponentsStillLoading;
        public bool ComponentsFinishedLoading { get; private set; }
        public TimeSpan ElapsedUnpausedTime;

        private List<GameComponent> _xnaComponents;
        private MutableDistinctSet<TdglComponent> _components;

        private List<TdglComponent> _componentsSortedByDrawOrder;
        private Dictionary<TdglComponent, int> _componentDrawOrders;
        private bool needToSortComponentsByDrawOrder;


        public override sealed void Draw(GameTime gameTime)
        {
            // Moved here from draw because it was not syncing due to updates being paused and draws not.
            if (needToSortComponentsByDrawOrder)
            {
                _componentsSortedByDrawOrder = _components.OrderBy(component => component.DrawOrder).ToList();
                needToSortComponentsByDrawOrder = false;
            }

            foreach (TdglComponent component in _componentsSortedByDrawOrder)
            {
                if (ComponentsFinishedLoading)
                {
                    component.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }


        public override sealed void Update(GameTime gameTime)
        {
            //if (Guide.IsVisible)
            //{
            //    return;
            //}

            UiManager.Update(gameTime);

            if (!IsGamePaused)
            {
                ElapsedUnpausedTime += gameTime.ElapsedGameTime;
            }

            _components.Commit(); //Commit recent changes to the mutable set

            bool someComponentStillLoading = false;
            foreach (TdglComponent component in _components)
            {
                if (ComponentsStillLoading)
                {
                    if (!component.IsLoaded)
                    {
                        someComponentStillLoading = true; //keep trying to load components
                        component.LoadWait(gameTime);
                    }
                }
                else if (component != UiManager) //UiManager was already updated earlier
                {
                    if (!IsGamePaused || component.IsPauseIgnored)
                    {
                        component.Update(gameTime);
                    }
                }

                //If the draw order for any component has changed, need to re-sort all components by draw index in the next update
                if (!_componentDrawOrders.ContainsKey(component) || _componentDrawOrders[component] != component.DrawOrder)
                {
                    needToSortComponentsByDrawOrder = true;
                }
                _componentDrawOrders[component] = component.DrawOrder;
            }

            if (ComponentsStillLoading && !someComponentStillLoading)
            {
                ComponentsStillLoading = false; //done loading
            }
            else if (!ComponentsStillLoading && !ComponentsFinishedLoading)
            {
                ComponentsFinishedLoading = true; //done loading, finished updating each component exactly once
            }
        }


        public T AddComponent<T>(T component)
            where T : TdglComponent
        {
            if (component != null && !_components.Contains(component))
            {
                if (!component.IsLoaded)
                {
                    ComponentsStillLoading = true;
                }
                _components.Add(component);
            }

            return component;
        }


        public void RemoveComponent(TdglComponent component)
        {
            if (component != null)
            {
                _components.Remove(component);
                _componentsSortedByDrawOrder.Remove(component);
                component.Destruct();
            }
        }


        public GameComponent AddXnaComponent(GameComponent xnaComponent)
        {
            if (xnaComponent != null && !Game.Components.Contains(xnaComponent))
            {
                Game.Components.Add(xnaComponent);
            }

            return xnaComponent;
        }


        public void RemoveXnaComponent(GameComponent xnaComponent)
        {
            if (xnaComponent != null && Game.Components.Contains(xnaComponent))
            {
                Game.Components.Remove(xnaComponent);
                xnaComponent.Dispose();
            }
        }


        bool isDestroyed = false;
        public void Destruct()
        {
            if (!isDestroyed)
            {
                foreach (TdglComponent component in _components)
                {
                    _components.Remove(component);
                    component.Destruct();
                }
                _components.Commit();

                foreach (GameComponent xnaComponent in _xnaComponents)
                {
                    Game.Components.Remove(xnaComponent);
                    xnaComponent.Dispose();
                }
                _xnaComponents.Clear();

                Game.Components.Remove(this);
                Dispose();
                isDestroyed = true;
            }
        }


        public void GameOver()
        {
            ComponentsStillLoading = true;
            ComponentsFinishedLoading = false;
            foreach (TdglComponent component in _components)
            {
                component.GameOver();
            }
        }

    }
}
