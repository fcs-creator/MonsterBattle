using UnityEngine;
using System.Threading.Tasks;

public class こた天才_Weapon_a3b1927872ee4a5c894b174b9a653a61 : Weapon
{
	async protected override Task Attack(int number)
	{
		await Drawing();
		await Spin(10000000000000000,1000000000);
	}
}
