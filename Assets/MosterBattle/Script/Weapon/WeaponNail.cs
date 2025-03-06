using UnityEngine;
using System.Threading.Tasks;

public class WeaponNail: Weapon
{
    async protected override Task Attack() 
    {
        await Move(0, 0, 0.5f);
    }
}
