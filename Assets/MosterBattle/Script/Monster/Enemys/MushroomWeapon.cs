using UnityEngine;
using System.Threading.Tasks;

public class MushroomWeapon : Weapon
{
    async protected override Task Attack(int number) 
    {
        await Rotate(-30, 120, 1.0f);

        await Rotate(120, -150, 1.0f);
    }
}
