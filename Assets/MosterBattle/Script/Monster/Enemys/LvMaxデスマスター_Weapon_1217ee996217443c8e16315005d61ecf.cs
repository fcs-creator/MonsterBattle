using UnityEngine;
using System.Threading.Tasks;

public class LvMaxデスマスター_Weapon_1217ee996217443c8e16315005d61ecf : Weapon
{
	//3
	async protected override Task Attack(int number)
	{
		// ここに武器の処理を書く

		//var clones = await Clone(10);
		//
		//float x = transform.position.x + 10;
		//Vector2 position = transform.position;
		//
		//for ( int i = 0; i < 10; i++ ) 
		//{
		//	clones[i].transform.position = new Vector2(x,position.y);
		//	var dirction  = (clones[i].transform.position - transform.position).normalized;
		//	await clones[i].ShotDirection(dirction, 30);
		//}

		await Task.Yield();
	}
}
