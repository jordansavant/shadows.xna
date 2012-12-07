using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDGameLibrary.Components;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using TDGameLibrary.Mobs;
using TDGameLibrary.DataStructures;

namespace ShadowShootout.Components
{
    public class PooledBodyManager : TdglComponent
    {
        public PooledBodyManager(WorldManager worldManager, GhostBodyManager ghostBodyManager)
        {
            WorldManager = worldManager;
            GhostBodyManager = ghostBodyManager;

            CircularFarseerBodyPool = new ResourcePool<PhysicalBody>(CreateCircularFarseerBody, 10);
            CircularFarseerStaticBodyPool = new ResourcePool<PhysicalBody>(CreateCircularFarseerStaticBody, 10);
            CircularGhostBodyPool = new ResourcePool<PhysicalBody>(CreateCircularGhostBody, 10);
        }

        public WorldManager WorldManager { get; private set; }
        public GhostBodyManager GhostBodyManager { get; private set; }

        public ResourcePool<PhysicalBody> CircularFarseerBodyPool { get; private set; }
        public ResourcePool<PhysicalBody> CircularFarseerStaticBodyPool { get; private set; }
        public ResourcePool<PhysicalBody> CircularGhostBodyPool { get; private set; }

        int positionX = 0;

        private PhysicalBody CreateCircularFarseerBody()
        {
            Body body = new Body(WorldManager.World);
            FixtureFactory.AttachCircle(1f, 1f, body);
            body.BodyType = BodyType.Dynamic;
            body.Position = body.Position + new Vector2(body.Position.X + positionX, body.Position.Y);
            positionX += 100;

            return new PooledFarseerBody(body, CircularFarseerBodyPool);
        }

        private PooledFarseerBody CreateCircularFarseerStaticBody()
        {
            Body body = new Body(WorldManager.World);
            FixtureFactory.AttachCircle(1f, 1f, body);
            body.BodyType = BodyType.Static;
            body.Position = body.Position + new Vector2(body.Position.X + positionX, body.Position.Y);
            positionX += 100;

            return new PooledFarseerBody(body, CircularFarseerStaticBodyPool);
        }

        private GhostPhysicalBody CreateCircularGhostBody()
        {
            return new PooledGhostBody(GhostBodyManager, CircularGhostBodyPool);
        }

    }
}
