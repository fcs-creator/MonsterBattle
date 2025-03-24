using UnityEngine;
using System.Threading.Tasks;

public class でスライム : Monster
{
	protected override async Task ActionLoop()
	{
        //ここにプログラムを書く

        await Floating(true);

        await Forward(100);

        await SwitchWeapon(0);

        await Attack();

        await SwitchWeapon(1);

        await Attack();

        await Jump(30);
    }


}
