using UnityEngine;
using System.Threading.Tasks;

public class Bomb: Weapon
{
    async protected override Task Attack()
    {
        await Clone(100);

        //await Move(0, -10, 0.5f);
        //
        //await Move(0, 10, 0.5f);
        //
        //await Move(0, -10, 0.5f);
        //
        //await Move(0, 10, 0.5f);
        //
        //await Move(0, -10, 0.5f);
        //
        //await Move(0, 10, 0.5f);
    }
}