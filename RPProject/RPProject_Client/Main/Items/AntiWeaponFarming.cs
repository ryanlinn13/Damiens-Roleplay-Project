﻿using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace client.Main.Items
{
    public class AntiWeaponFarming : BaseScript
    {
        public AntiWeaponFarming()
        {
            DeleteWeapons();
        }

        public async Task DeleteWeapons()
        {
            while (true)
            {
                API.RemoveAllPickupsOfType(0xDF711959);
                API.RemoveAllPickupsOfType(0xF9AFB48F);
                API.RemoveAllPickupsOfType(0xA9355DCD);
               await Delay(0);
            }
        }
    }
}
