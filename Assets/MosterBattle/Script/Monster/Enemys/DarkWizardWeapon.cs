using UnityEngine;
using System.Threading.Tasks;

public class DarkWizardWeapon: Weapon
{
    async protected override Task Attack(int number) 
    {
        await Drawing();

        await Move(3, 0, 0.5f);

        await Spin(720, 0.5f);
    }
}
