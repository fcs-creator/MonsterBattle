using UnityEngine;
using System.Threading.Tasks;

public class LvMaxデスマスター_Weapon_1fcf10934dcc40d59aa8978b07425f9a : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Task.Yield();
	}
}
