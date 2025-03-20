using UnityEngine;
using System.Threading.Tasks;

public class Lv5ダークウィザード_Weapon_ceb24aa38a28418ba5ac5d3cf8c02843 : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Task.Yield();
	}
}
