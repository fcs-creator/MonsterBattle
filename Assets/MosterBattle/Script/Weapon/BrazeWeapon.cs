using UnityEngine;
using System.Threading.Tasks;

public class BrazeWeapon: Weapon
{
    async protected override Task Attack(int number) 
    {
        await Drawing();

        await Move(5, 0, 0.25f);

        await Move(5, 0, 0.25f);
    }
}