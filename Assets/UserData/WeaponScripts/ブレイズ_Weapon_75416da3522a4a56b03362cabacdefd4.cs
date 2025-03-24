using UnityEngine;
using System.Threading.Tasks;

public class ブレイズ_Weapon_75416da3522a4a56b03362cabacdefd4 : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Drawing();
		await Spin(2, 3);


		await Shot(0, 100);

		await Move(1, 1, 100);
	}
}