using UnityEngine;
using System.Threading.Tasks;

public class WeaponSword : Weapon
{
    async protected override Task Attack(int number) 
    {
        await Rotate(0, 120, 0.2f);

        await Rotate(140, -180, 0.2f);

    }
}
