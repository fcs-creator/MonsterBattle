using UnityEngine;
using System.Threading.Tasks;

public class WeaponSword : Weapon
{
    async protected override Task Attack() 
    {
        await Rotate(0, 120, 0.1f);

        await Rotate(140, -180, 0.1f);

    }
}
