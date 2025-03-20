using UnityEngine;
using System.Threading.Tasks;

public class DarkWizardWeapon: Weapon
{
    async protected override Task Attack(int number) 
    {

        if (number == 0) 
        {
            await Rotate(100, 160, 0.5f);

            await Rotate(260, -160, 0.5f);
        }
        else if (number == 1) 
        {
            await Drawing();

            await Move(3, 0, 0.5f);

            await Spin(720, 0.5f);

            await Move(0, 0, 0.2f);
        }
        
    }
}
