using UnityEngine;
using System.Threading.Tasks;

public class LvMaxデスマスター_Weapon_9f78b7436a3b419ea55df8a2825b38a4 : Weapon
{
	//デスマスター武器2
	async protected override Task Attack(int number)
	{
		await Move(0, 3, 0.2f);

		await Drawing();
    }
}
