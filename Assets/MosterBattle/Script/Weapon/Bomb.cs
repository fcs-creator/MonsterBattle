using UnityEngine;
using System.Threading.Tasks;

public class Bomb: Weapon
{
    async protected override Task Attack(int number)
    {
        var clones = await Clone(100);

        for (int i = 0; i < clones.Length; i++)
        {
            await clones[i].Shot(Random.Range(-1,1), 10);
        }

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