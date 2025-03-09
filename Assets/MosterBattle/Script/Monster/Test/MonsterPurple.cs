using UnityEngine;
using System.Threading.Tasks;

public class MonsterPurple : Monster
{
    protected override async Task ActionLoop()
    {
        await Forward(10.0f);

        await Wait(0.5f);

        await Attack();

        await BackStep(10.0f);

        await Wait(0.5f);
    }
}
