using UnityEngine;
using System.Threading.Tasks;

public class ポッチャマ_Weapon_9c63eb98f3ce4434b94a5a2438bcd30c : Weapon
{
	async protected override Task Attack(int number)
	{
        await Drawing();
        await Move(30000,0,3f);
		
	}
	
}
