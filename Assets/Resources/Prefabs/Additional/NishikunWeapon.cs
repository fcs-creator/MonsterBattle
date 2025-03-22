using UnityEngine;
using System.Threading.Tasks;

public class NishikunWeapon :Weapon
{
    async protected override Task Attack(int number) 
    {
        await Drawing();

        await Move(3, 0, 0.5f);

        await Move(-3, 0, 0.5f);

        await Move(3, 0, 0.5f);

        await Move(-3, 0, 0.5f);

        await Shot(0.3f, 50);
    }
}