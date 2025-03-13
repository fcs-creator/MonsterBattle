using UnityEngine;
using System.Threading.Tasks;

public class SquareSquare: Weapon
{
    async protected override Task Attack() 
    {
        await Shot(0, 100);
    }
}