using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDGameLibrary.Components;
using Microsoft.Xna.Framework;
using TDGameLibrary.DataStructures;

namespace TDGameLibrary.Mobs
{
    public class GhostBodyManager : TdglComponent
    {
        public GhostBodyManager()
            : base()
        {
            Ghosts = new List<GhostPhysicalBody>();
            GhostsToAdd = new List<GhostPhysicalBody>();
            GhostsToRemove = new List<GhostPhysicalBody>();
        }

        public List<GhostPhysicalBody> Ghosts;
        public List<GhostPhysicalBody> GhostsToAdd;
        public List<GhostPhysicalBody> GhostsToRemove;

        public override void Update(GameTime gameTime)
        {
            foreach (GhostPhysicalBody ghost in GhostsToRemove)
            {
                Ghosts.Remove(ghost);
            }
            GhostsToRemove.Clear();

            foreach (GhostPhysicalBody ghost in GhostsToAdd)
            {
                Ghosts.Add(ghost);
            }
            GhostsToAdd.Clear();

            foreach (GhostPhysicalBody ghost in Ghosts)
            {
                ghost.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public void RegisterGhost(GhostPhysicalBody ghost)
        {
            GhostsToAdd.Add(ghost);
        }

        public void UnregisterGhost(GhostPhysicalBody ghost)
        {
            GhostsToRemove.Add(ghost);
        }

    }
}
