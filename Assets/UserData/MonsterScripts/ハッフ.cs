using UnityEngine;
using System.Threading.Tasks;

public class ハッフ : Monster
{
	protected override async Task ActionLoop()
	{
		await Forward(200);

		await Attack(200);
		await Guard();
        await this.MagicBook.FireBall(this, 3, 30);
        if (Position.x > 20)
		{
			await LookAtEnemy();
			await Forward(100);
			await this.MagicBook.FireBall(this,3,30);
			await this.MagicBook.IceNeedle(this,3,30);
		}

	}
}

