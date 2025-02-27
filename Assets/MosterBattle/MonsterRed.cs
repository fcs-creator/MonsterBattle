using UnityEngine;
using System.Threading.Tasks;

public class MonsterRed : Monster
{
    protected override async Task ActionLoop() 
    {
        await Dash(20);

        await Wait(0.5f);

        await Attack();

        await Wait(0.5f);

        await Jump(15);

        await Wait(0.5f);

        await Guard();

        await BackStep(8);

        await Wait(1.0f);
        
    }
}
