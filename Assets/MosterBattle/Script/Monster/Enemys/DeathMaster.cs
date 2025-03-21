using UnityEngine;
using System.Threading.Tasks;

public class DeathMaster : Monster
{
    protected override async Task ActionLoop()
    {
        await MagicBook.IceNeedle(this, -0.5f, 30f);

        await SwitchWeapon(2);
       
        await Attack();
       
        await SwitchWeapon(0);
        await Forward(EnemyDistance);
        await Attack();
       
        await SwitchWeapon(1);
       
        await Attack();
    }
}
