using UnityEngine;
using System.Threading.Tasks;

public class SquareSquare: Weapon
{
    async protected override Task Attack(int number) 
    {
        await Shot(0, 100);
    }
}