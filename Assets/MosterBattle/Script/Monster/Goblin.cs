using UnityEngine;
using System.Threading.Tasks;

public class Goblin : Monster
{
    protected override async Task ActionLoop()
    {
        await Forward(50);

        await Attack();

        await BackStep(25);

        await Attack();
    }
}
