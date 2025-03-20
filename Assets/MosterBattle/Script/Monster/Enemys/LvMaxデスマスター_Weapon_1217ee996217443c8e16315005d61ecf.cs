using UnityEngine;
using System.Threading.Tasks;

public class LvMaxデスマスター_Weapon_1217ee996217443c8e16315005d61ecf : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Task.Yield();
	}
}
