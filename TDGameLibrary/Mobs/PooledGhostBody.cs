using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDGameLibrary.DataStructures;

namespace TDGameLibrary.Mobs
{
    public class PooledGhostBody : GhostPhysicalBody
    {
        public PooledGhostBody(GhostBodyManager ghostBodyManager, ResourcePool<PhysicalBody> pool)
            : base(ghostBodyManager)
        {
            _pool = pool;
        }

        private ResourcePool<PhysicalBody> _pool;

        public override void Destruct()
        {
            _pool.Release(this);
            //do NOT want to call base here since the pooled ghost could not reregister once unregistered from GhostBodyManager
        }
    }
}
