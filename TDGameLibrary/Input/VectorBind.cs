using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Input
{
    internal sealed class VectorBind : IVectorBind
    {
        public VectorBind()
        {
            DirectionalDevices = new List<IDirectionalDevice>();
        }

        public List<IDirectionalDevice> DirectionalDevices { get; private set; }

        public bool IsNeutral
        {
            get
            {
                return DirectionalDevices.All(a => a.IsNeutral);
            }
        }

        public Vector2 GetCurrentVector()
        {
            Vector2 direction = Vector2.Zero;
            foreach(IDirectionalDevice d in DirectionalDevices)
            {
                direction += d.GetCurrentVector();
            }

            if (direction != Vector2.Zero)
                direction.Normalize();

            return direction;
        }

        public Vector2 GetLastVector()
        {
            Vector2 direction = Vector2.Zero;
            foreach (IDirectionalDevice d in DirectionalDevices)
            {
                direction += d.GetLastVector();
            }

            if (direction != Vector2.Zero)
                direction.Normalize();

            return direction;
        }

        public void Update()
        {
            foreach (IDirectionalDevice d in DirectionalDevices)
            {
                d.Update();
            }
        }
    }
}
