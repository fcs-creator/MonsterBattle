using UnityEngine;
using System.Threading.Tasks;

public class DarkWizard : Monster
{
    protected override async Task ActionLoop()
    {
        await Floating(true);

        await Move(EnemyDirection.x, 1, 80f);

        if (IsFloating)
        {
            if (10 < Position.y && Position.y < 15)
            {
                await Attack(0);
            }
            else 
            {
                await MagicBook.FireBall(this, 5, 20f);
            }

            if (Position.x > 0)
                await Move(-1, 0.2f, 100f);
            else
                await Move(1, 0.2f, 100f);            
            
            await LookAtEnemy();

            if (Enemy.Position.x - Position.x > 20)
            {
                await Move(0, 1, 100f);

                await Floating(false);
            }

        }
        else
        {
            if (EnemyDistance > 10)
            {
                await Attack(1);

                await Floating(true);

                await Move(EnemyDirection.x, 1, 80f);
            }
        }       
    }
}
