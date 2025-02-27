using UnityEngine;
using System.Threading.Tasks;

public class WeaponSword : Weapon
{
    async protected override Task Attack() 
    {
        await Rotate(0, 120, 0.5f);

        WarpDefault();

        await Rotate(140, -180, 0.5f);

    }
}
