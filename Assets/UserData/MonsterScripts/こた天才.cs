using UnityEngine;
using System.Threading.Tasks;

public class こた天才 : Monster
{
	protected override async Task ActionLoop()
	{
        //await Floating(true);

        await Forward(200);
		await Jump(30);
		await Attack();
		await Guard();
		await LookAtEnemy();

		await this.MagicBook.FireBall(this, 3, 30);

	}

}
