using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using TDGameLibrary.DataStructures;

namespace TDGameLibrary.Mobs
{
    public class PooledFarseerBody : FarseerPhysicalBody
    {
        public PooledFarseerBody(Body body, ResourcePool<PhysicalBody> pool)
            : base(body)
        {
            _pool = pool;
        }

        private ResourcePool<PhysicalBody> _pool;

        public override void Destruct()
        {
            _pool.Release(this);
        }

    }
}
