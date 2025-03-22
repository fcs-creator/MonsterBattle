using UnityEngine;
using System.Threading.Tasks;

public class EvilSummonerWeapon01 : Weapon
{
	//1番の武器 フライングアイの動き
	async protected override Task Attack(int number)
	{
		await Drawing();

		await ShotDirection(new Vector2(Owner.EnemyDirection.x, 0.2f).normalized , 30);
    }
}
