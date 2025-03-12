using UnityEngine;
using System.Threading.Tasks;

public class WeaponRed : Weapon
{
    async protected override Task Attack()
    {
        await Rotate(0, 150, 0.5f);

        await Rotate(150, -150, 0.5f);

        await Wait(1);

        await Shot(0, 70);
    }
}
