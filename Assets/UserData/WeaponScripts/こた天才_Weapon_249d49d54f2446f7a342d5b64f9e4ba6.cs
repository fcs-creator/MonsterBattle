using UnityEngine;
using System.Threading.Tasks;

public class こた天才_Weapon_249d49d54f2446f7a342d5b64f9e4ba6 : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Task.Yield();
	}
}
