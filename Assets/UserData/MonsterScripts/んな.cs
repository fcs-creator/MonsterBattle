using UnityEngine;
using System.Threading.Tasks;

public class んな : Monster
{
	protected override async Task ActionLoop()
	{
        // ここに処理を書く



        if (Enemy.IsStunned)
		{
            await LookAtEnemy();
            await SwitchWeapon(0);
            await Attack();
		}
		//else
		//{
		//	await Guard();
		//}
		
		if (EnemyDistance > 20)
		{
			//await Jump(50);
			await LookAtEnemy();
			await SwitchWeapon(1);
            await Attack();

            if (Enemy.IsAttacking)
            {
				await SwitchWeapon(3);
				await Attack();
            }
        } 

		if (EnemyDistance < 20) 
		{
            await LookAtEnemy();
            await Guard();

            //await MagicBook.FireBall(this, 10, 100);
        }

        if (Enemy.IsJumping) 
		{
            await LookAtEnemy();
            await SwitchWeapon(2);
			await Attack();

		}

    }
}
