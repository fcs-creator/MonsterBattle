using UnityEngine;
using System.Threading.Tasks;

public class Dagger: Weapon
{
    async protected override Task Attack() 
    {
        await Drawing();
    }
}
