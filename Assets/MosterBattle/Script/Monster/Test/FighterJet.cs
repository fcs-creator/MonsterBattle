using UnityEngine;
using System.Threading.Tasks;

public class FighterJet : Monster
{
    protected override async Task ActionLoop()
    {
        await Floating(true);

        await Move(EnemyDirection.x, 0, EnemyDistance * 1f);

        await Attack();

    }

}
