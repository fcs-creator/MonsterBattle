using UnityEngine;
using System.Threading.Tasks;

public class Goblin : Monster
{
    protected override async Task ActionLoop()
    {
        await Dash(50);

        await Attack();

        await BackStep(25);
    }
}
