using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TrolleyTest
{
    public static class ModMain
    {
        public static string ModName = "Bomber";
        public static string ModDescription = "<size=90%>A bomber can place bombs in the map.";
        public static Sprite ModImage; 

        public static void LoadMod()
        {
            ModImage = Utils.LoadSprite("TrolleyTest.Resources.explode.png", 100f);
            if (ModImage == null) Debug.Log("are you serious right now");
            RoleManager.Instance.AddRole<BomberRole>();
            RoleManager.Instance.AddSprite("explodeSprite", ModImage.texture, 720f);
        }
    }
}
