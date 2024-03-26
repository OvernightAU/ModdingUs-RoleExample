using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TrolleyTest
{
    public static class ModMain
    {
        public static string ModName = "Trolley";
        public static string ModDescription = "Adds a new role: Trolley";
        public static Sprite ModImage; 

        public static void LoadMod()
        {
            ModImage = Utils.LoadSprite("TrolleyTest.Resources.trolley.png", 100f);
            if (ModImage == null) Debug.Log("are you serious right now");
            RoleManager.Instance.AddRole<TrolleyRole>();
        }
    }
}
