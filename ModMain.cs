﻿using UnityEngine;

namespace TrolleyTest
{
    public class ModMain : ModBehaviour
    {
        public override string ModName => "Bomber";
        public override string ModDescription => "<size=90%>A bomber can place bombs in the map.";
        public override Sprite ModImage => Utils.LoadSprite("RoleExample.Resources.explode.png", 100f);

        public override void ApplyMod()
        {
            // Here you load all your mod features (e.g Roles and hats)
            // You can also use harmony, not recommended but possible

            RoleManager.Instance.AddRole<BomberRole>();
            RoleManager.Instance.AddSprite("explodeSprite", ModImage.texture, 720f);
            Utils.AddHat("bomb", ModImage, ModImage, true);
        }
    }
}
