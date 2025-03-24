using UnityEngine;
using System.Threading.Tasks;

public class ペン_Weapon_8968269b3de2466bb4310ad6c8a84a82 : Weapon
{
	async protected override Task Attack(int number)
	{

		await Move(3, 0, 0);

        for (int i = 0; i < 3;i++)
		{
			await Shot(0, 300);

		}

	}
}
