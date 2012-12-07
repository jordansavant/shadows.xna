using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Map
{
    public class ChunkedMap
    {
        public ChunkedMap(String mapFilename, Rectangle mapRectangle, Vector2 chunkSize, Vector2 rowsColumns, float parallaxFactor)
        {
            MapFilename = mapFilename;
            MapRectangle = mapRectangle;
            ChunkSize = chunkSize;
            RowsColumns = rowsColumns;
            ParallaxFactor = Math.Max(0.1f, parallaxFactor);
            Color = Color.White;
        }

        public Rectangle MapRectangle;
        private Vector2 chunkSize;
        public Vector2 ChunkSize { get { return chunkSize / ParallaxFactor; } set { chunkSize = value; } }
        public Vector2 RowsColumns;
        public string MapFilename;
        public float ParallaxFactor;
        public Color Color { get; set; }
    
    }
}
