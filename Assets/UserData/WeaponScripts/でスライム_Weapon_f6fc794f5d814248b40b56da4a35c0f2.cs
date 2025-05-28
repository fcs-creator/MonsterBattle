using UnityEngine;
using System.Threading.Tasks;

public class でスライム_Weapon_f6fc794f5d814248b40b56da4a35c0f2 : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く
		await ShotDirection(Owner.EnemyDirection, 30f);

        await Task.Yield();
	}
}
