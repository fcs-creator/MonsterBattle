using UnityEngine;
using System.Threading.Tasks;

public class ぽっちゃろう_Weapon_8f83fa19487444029783b4d80f65d2e4 : Weapon
{
	async protected override Task Attack(int number)
	{
		await Move(0, -500, 1f);
        await Drawing();
    }
}
