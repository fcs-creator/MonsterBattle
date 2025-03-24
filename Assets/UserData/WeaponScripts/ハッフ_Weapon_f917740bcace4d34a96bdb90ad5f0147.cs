using UnityEngine;
using System.Threading.Tasks;

public class ハッフ_Weapon_f917740bcace4d34a96bdb90ad5f0147 : Weapon
{
	async protected override Task Attack(int number)
	{
		await Drawing();
		await Move(0, 10,3);
	}
}