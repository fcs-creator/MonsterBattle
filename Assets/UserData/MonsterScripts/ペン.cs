using UnityEngine;
using System.Threading.Tasks;

public class ペン : Monster
{
	protected override async Task ActionLoop()
	{
		await Forward(100);

		await Attack(100);

		await JumpForward(30);

		await SwitchWeapon(0);

		if (EnemyDistance < 10)
		{
			await Guard();
		}

		await LookAtEnemy();

        for (int i = 0; i < 3; i++)
        {
			await Move(0, 10, 0);
        }

		if (Enemy.IsStunned)
		{
			await Attack(100);
		}

		await SwitchWeapon(1);
		await SwitchWeapon(0);
    }

}
