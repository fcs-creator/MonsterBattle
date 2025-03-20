using UnityEngine;
using System.Threading.Tasks;

public class LvMaxデスマスター_Weapon_3c8d2bbd90ce400494b992d60294148d : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Task.Yield();
	}
}
