using UnityEngine;
using System.Threading.Tasks;

public class Lv4イビルサモナー_Weapon_66cb18fe78594033a573fbba5116c62c : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Task.Yield();
	}
}
