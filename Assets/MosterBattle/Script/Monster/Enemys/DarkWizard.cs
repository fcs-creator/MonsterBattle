using UnityEngine;
using System.Threading.Tasks;

public class DarkWizard : Monster
{
    protected override async Task ActionLoop()
    {
        await MagicBook.Thunder(this);

        if (IsFloating)
        {
            await MagicBook.FireBall(this, 5, 30f);

            if(Position.x > 0)
                await Move(-1, 0.2f, 100f);
            else
                await Move(1, 0.2f, 100f);

            await LookAtEnemy();

            if (Enemy.Position.x - Position.x > 15)
            {
                await Floating(false);
            }
        }
        else
        {
            if (EnemyDistance > 10)
            {
                await Attack();

                await Floating(true);

                await Move(EnemyDirection.x, 0.5f, 20f);
            }
            else 
            {
                await MagicBook.Thunder(this);

                await Backward(30);
            }

        }       
    }
}
