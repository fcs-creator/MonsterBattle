using UnityEngine;
using System.Threading.Tasks;

public class EvilSummoner : Monster
{
    protected override async Task ActionLoop()
    {
        if (EnemyDistance < 12)
        {
            await Guard();

            await LookAtEnemy();

            await Attack();
        }
        else 
        {
            await JumpForward(45);

            await LookAtEnemy();

            await Attack();
        }
    }
}
