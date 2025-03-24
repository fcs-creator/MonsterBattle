using UnityEngine;
using System.Threading.Tasks;

public class んな_Weapon_e832cb7d77524c839f960696c9b553b7 : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Shot(0,100);
		await Shot(0,100);
		await Shot(0,100);

	}
}
