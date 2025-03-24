using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting.YamlDotNet.Core;

public class こた天才_Weapon_27283dad5acd48d4ac7464882d7eb36f : Weapon
{
	async protected override Task Attack(int number)
	{
		await Drawing();
		//await Shot(3,3);
		
		await Move(1000, 0, 0.5f);

    }
}