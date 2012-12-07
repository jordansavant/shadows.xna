using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDGameLibrary.Map
{
    public class LayeredMapEngine
    {
        public LayeredMapEngine()
        {
            MapLayers = new List<MapLayer>();
        }

        public LayeredMapEngine(List<MapLayer> mapLayers)
        {
            MapLayers = new List<MapLayer>();
            this.MapLayers = mapLayers;
        }


        public List<MapLayer> MapLayers { get; set; }


        public virtual void Update(GameTime gameTime, Rectangle screenRectangle)
        {
            foreach (MapLayer mapLayer in this.MapLayers)
            {
                mapLayer.Update(gameTime, screenRectangle);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle screenRectangle)
        {
            // we want to draw all map layers at their original resolution, and centered on the screen or panel rectangle
            // - we must offset the x and y in doing so

            // loop through the map layers and draw the images
            foreach (MapLayer mapLayer in this.MapLayers)
            {
                mapLayer.Draw(gameTime, spriteBatch, screenRectangle);
            }
        }

        public virtual void AddMapLayer(MapLayer mapLayer)
        {
            MapLayers.Add(mapLayer);
        }

    }
}
