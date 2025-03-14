using UnityEngine;
using System.Threading.Tasks;

public class FighterJet : Monster
{
    protected override async Task ActionLoop()
    {
        await Floating(true);

        await Move(Enemy.Direction.x, 0, Enemy.Distance * 1f);

        await Attack();

    }

}
