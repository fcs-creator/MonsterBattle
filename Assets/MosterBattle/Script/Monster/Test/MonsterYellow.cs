using UnityEngine;
using System.Threading.Tasks;

public class MonsterYellow : Monster
{
    protected override async Task ActionLoop()
    {
        await Dash(10.0f);

        await Wait(0.75f);

        await Attack();

        await BackStep(10.0f);
        
        await Wait(0.5f);
    }

}
