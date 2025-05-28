using UnityEngine;
using System.Threading.Tasks;

public class でスライム_Weapon_cafe1e2ecf484ef1abd9f432413eb994 : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Task.Yield();
	}
}
