using UnityEngine;
using System.Threading.Tasks;

public class LvMaxデスマスター_Weapon_9f78b7436a3b419ea55df8a2825b38a4 : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Task.Yield();
	}
}
