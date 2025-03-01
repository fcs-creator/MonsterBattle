using UnityEngine;
using System.Threading.Tasks;

public class WeaponRed : Weapon
{
    async protected override Task Attack()
    {
        await RotateAround(0, 120, 0.5f);

        await RotateAround(120, -120, 0.5f);
    }
}
