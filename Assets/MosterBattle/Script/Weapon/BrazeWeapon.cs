using UnityEngine;
using System.Threading.Tasks;

public class BrazeWeapon: Weapon
{
    async protected override Task Attack(int number) 
    {
        await Drawing();

        await Move(2, 0, 0.2f);

        await Move(-2, 0, 0.2f);

        await Move(2, 0, 0.2f);

        await Move(-2, 0, 0.2f);
    }
}