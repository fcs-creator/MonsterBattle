using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        //await Forward(10000000000000000);

        //await Forward(3000);

        await Forward(80);

        await LookAtEnemy();

        await Guard();

        await Forward(200);

        await Attack();
    }
}
