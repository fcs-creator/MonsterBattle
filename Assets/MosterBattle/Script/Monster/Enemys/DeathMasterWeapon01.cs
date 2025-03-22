using UnityEngine;
using System.Threading.Tasks;

public class DeathMasterWeapon01 : Weapon
{
	// 1番の武器　ダークハンド
	async protected override Task Attack(int number)
	{
		await Move(0, 3, 1.5f);

		await Drawing();
    }
}
