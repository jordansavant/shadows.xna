using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Weapons;
using FarseerPhysics.Dynamics.Contacts;
using TDGameLibrary.Mobs;

namespace TDGameLibrary.Components
{
    public class WeaponManager : TdglComponent
    {
        public WeaponManager(MobManager mobManager)
        {
            MobManager = mobManager;

            Weapons = new List<Weapon>();

            WeaponEffects = new List<WeaponEffect>();
            WeaponEffectsToAdd = new List<WeaponEffect>();
            WeaponEffectsToRemove = new List<WeaponEffect>();

            Projectiles = new List<Projectile>();
        }

        public MobManager MobManager { get; private set; }
        public List<Weapon> Weapons { get; private set; }

        public List<WeaponEffect> WeaponEffects { get; private set; }
        private List<WeaponEffect> WeaponEffectsToAdd;
        private List<WeaponEffect> WeaponEffectsToRemove;

        public List<Projectile> Projectiles { get; private set; }
        
        public override void Update(GameTime gameTime)
        {
            foreach (WeaponEffect effect in WeaponEffects)
            {
                effect.Update(gameTime);
            }

            foreach (WeaponEffect effect in WeaponEffectsToRemove)
            {
                WeaponEffects.Remove(effect);
            }
            WeaponEffectsToRemove.Clear();

            foreach (WeaponEffect effect in WeaponEffectsToAdd)
            {
                WeaponEffects.Add(effect);
            }
            WeaponEffectsToAdd.Clear();
        }


        public void RegisterWeapon(Weapon weapon)
        {
            if (weapon != null && !Weapons.Contains(weapon))
            {
                Weapons.Add(weapon);
            }
        }

        public void UnregisterWeapon(Weapon weapon)
        {
            Weapons.Remove(weapon);
        }

        public void RegisterProjectile(Projectile projectile)
        {
            if (projectile != null && !Projectiles.Contains(projectile))
            {
                Projectiles.Add(projectile);
            }
        }

        public void UnregisterProjectile(Projectile projectile)
        {
            Projectiles.Remove(projectile);
        }

        public void RegisterWeaponEffect(WeaponEffect weaponEffect)
        {
            if (weaponEffect != null && !WeaponEffects.Contains(weaponEffect) && !WeaponEffectsToAdd.Contains(weaponEffect))
            {
                WeaponEffectsToAdd.Add(weaponEffect);
            }
        }

        public void UnregisterWeaponEffect(WeaponEffect weaponEffect)
        {
            WeaponEffectsToRemove.Add(weaponEffect);
        }

    }
}
