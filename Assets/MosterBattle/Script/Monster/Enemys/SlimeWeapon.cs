using UnityEngine;
using System.Threading.Tasks;

public class SlimeWeapon : Weapon
{
    async protected override Task Attack(int number) 
    {
        await Drawing();
    }
}
