using UnityEngine;
using System.Threading.Tasks;

public class ポッチャマ : Monster
{
	protected override async Task ActionLoop()
	{

        

        await Forward(1000);
		await Attack();
       
        await SwitchWeapon(0);
        await SwitchWeapon(1);
		await SwitchWeapon(2);
        await LookAtEnemy();
		await Guard();
        await this.MagicBook.FireBall(this, 3, 30);
        await this.MagicBook.IceNeedle(this, 3, 30);
        await this.MagicBook.Thunder(this);

    }


}
