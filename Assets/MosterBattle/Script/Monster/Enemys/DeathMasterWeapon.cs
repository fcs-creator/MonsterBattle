using UnityEngine;
using System.Threading.Tasks;

public class DeathMasterWeapon: Weapon
{
    async protected override Task Attack(int number) 
    {
        await Task.Yield();
    }
}
