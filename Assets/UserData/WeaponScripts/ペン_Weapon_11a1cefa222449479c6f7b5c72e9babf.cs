using UnityEngine;
using System.Threading.Tasks;

public class ペン_Weapon_11a1cefa222449479c6f7b5c72e9babf : Weapon
{
	async protected override Task Attack(int number)
	{
        for (int i = 0; i < 3; i++)
        {
            await Shot(0, 500);

            await Shot(0, 500);

            await Shot(0, 500);

        }

        for (int i = 0; i < 3; i++)
        {
            await Shot(-1, 500);

           await Shot(-1, 500);

    }
    }
}
