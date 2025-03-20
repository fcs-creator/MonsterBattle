using UnityEngine;
using System.Threading.Tasks;

public class Lv4イビルサモナー_Weapon_4ead54819ae342f9ba238607655cf86a : Weapon
{
	//フライングアイの動き
	async protected override Task Attack(int number)
	{
		await Drawing();

		await ShotDirection(new Vector2(Owner.EnemyDirection.x, 0.2f).normalized , 30);
    }
}
