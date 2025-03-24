using UnityEngine;
using System.Threading.Tasks;

public class んな_Weapon_95be51246a5a454da74906309ad1bd9a : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Drawing();
		await Shot(0,100);
	}
}
