using UnityEngine;
using System.Threading.Tasks;

public class Blue : Monster
{
    protected override async Task ActionLoop()
    {
        await Dash(50);

        await Attack();

        await BackStep(20);

        await Jump(50);

        await Attack();

        await Wait(1);
    }
}
