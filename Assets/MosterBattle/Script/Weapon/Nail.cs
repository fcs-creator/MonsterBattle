using UnityEngine;
using System.Threading.Tasks;

public class Nail: Weapon
{
    async protected override Task Attack() 
    {
        await Drawing();
    }
}