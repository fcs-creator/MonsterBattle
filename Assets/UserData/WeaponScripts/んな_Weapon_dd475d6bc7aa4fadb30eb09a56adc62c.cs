using UnityEngine;
using System.Threading.Tasks;

public class んな_Weapon_dd475d6bc7aa4fadb30eb09a56adc62c : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Shot(0, 100);
	}
}
