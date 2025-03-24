using UnityEngine;
using System.Threading.Tasks;

public class んな_Weapon_17dca02737f8475495d58321f680735a : Weapon
{
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		await Drawing();
	}
}
