using UnityEngine;
using System.Threading.Tasks;

public class ブレイズ : Monster
{
	protected override async Task ActionLoop()
	{
		//await Attack();
		//
		//await Forward(100);

		await Floating(true);

		await Move(1,1, 100);

        await Floating(false);

		await this.MagicBook.FireBall(this, 3, 30);
    }
}
