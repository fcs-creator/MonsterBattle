using UnityEngine;
using System.Threading.Tasks;

public class MushroomWeapon : Weapon
{
    async protected override Task Attack(int number) 
    {
        await Rotate(-10, 120, 0.75f);

        await Rotate(-10, 120, 0.75f);
    }
}