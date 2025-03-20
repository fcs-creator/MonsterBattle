using UnityEngine;
using System.Threading.Tasks;

public class DarkWizard : Monster
{
    protected override async Task ActionLoop()
    {
        if (IsFloating)
        {
            await Move(-EnemyDirection.x, 0.5f, 20f);

            await LookAtEnemy();

            await MagicBook.FireBall(this, 3, 50f);

            if (EnemyDistance > 10)
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

                await Move(0, 0.5f, 20f);
            }
            else 
            {
                await MagicBook.Thunder(this);

                await Backward(30);
            }

        }       
    }
}
