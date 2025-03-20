using UnityEngine;
using System.Threading.Tasks;

public class Lv4イビルサモナー_Weapon_4ead54819ae342f9ba238607655cf86a : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Task.Yield();
	}
}
