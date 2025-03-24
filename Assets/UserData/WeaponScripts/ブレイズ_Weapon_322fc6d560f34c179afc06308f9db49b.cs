using UnityEngine;
using System.Threading.Tasks;

public class ブレイズ_Weapon_322fc6d560f34c179afc06308f9db49b : Weapon
{
	async protected override Task Attack(int number)
	{
		await Drawing();

		await Move(3, 0, 0.5f);
	}
}
