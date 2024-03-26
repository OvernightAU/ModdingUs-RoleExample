using System;
using System.Collections.Generic;
using System.Text;

namespace TrolleyTest
{
    public static class ModMain
    {
        public static void LoadMod()
        {
            RoleManager.Instance.AddRole<TrolleyRole>();
        }
    }
}
