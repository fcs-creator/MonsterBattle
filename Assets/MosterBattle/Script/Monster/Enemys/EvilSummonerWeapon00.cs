using UnityEngine;
using System.Threading.Tasks;

public class Lv4イビルサモナー_Weapon_66cb18fe78594033a573fbba5116c62c : Weapon
{
    //ワーウルフの動き
	async protected override Task Attack(int number)
	{
        await Drawing();

        await Move(0, 2, 0.25f);

        await Move(0, -2, 0.25f);

        await Move(0, 2, 0.25f);

        await Move(0, -2, 0.25f);

        await Move(5, 0, 0.4f);

        await Move(-5, 0, 0.4f);
	}
}
