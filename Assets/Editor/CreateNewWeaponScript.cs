
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class CreateNewWeaponScript
{

	private string weaponScriptName = string.Empty;

	public string WeaponScriptName
	{
		get { return weaponScriptName; }
	}

	public bool IsScriptLoaded
	{
		get
		{
			MonoScript weaponScript = AssetDatabase.LoadAssetAtPath<MonoScript>(weaponScriptName);
			return weaponScript != null;
		}
	}

	public void Create(string monsterName)
	{
		string className = "WeaponTemplate";
		string fileName = monsterName + "_Weapon_" + Guid.NewGuid().ToString("N") + ".cs";

		string monsterDir = Path.Combine(EditorConst.ScriptPath, "WeaponScripts");

		string destinationFilePath = Path.Combine(monsterDir, fileName);

		// 保存先フォルダがなければ作成
		if (!Directory.Exists(monsterDir))
		{
			Directory.CreateDirectory(monsterDir);
		}

		File.Copy(Path.Combine(EditorConst.WeaponSourceFilePath, className + ".cs"), destinationFilePath);

		string[] lines = File.ReadAllLines(destinationFilePath);
		for (int i = 0; i < lines.Length; i++)
		{
			if (lines[i].Contains("class " + className))
			{
				lines[i] = lines[i].Replace(className, Path.GetFileNameWithoutExtension(fileName));
				break;
			}
		}
		File.WriteAllLines(destinationFilePath, lines);

		weaponScriptName = destinationFilePath;
	}

	public static void CreateWeaponObject(Monster monster, string weaponScriptName)
	{

		string name = monster.name + "_Weapon" + monster.Weapons.Count.ToString("00");

		GameObject weaponObj = new GameObject(name);
		weaponObj.transform.SetParent(monster.gameObject.transform);    // 子にする意味はあんまりないかもね
		weaponObj.transform.localPosition = Vector3.zero;
		weaponObj.transform.localRotation = Quaternion.identity;
		weaponObj.transform.localScale = Vector3.one;

		{
			// SpriteRendererを追加
			SpriteRenderer spriteRenderer = weaponObj.AddComponent<SpriteRenderer>();
			// ソーティングレイヤーをWeaponに設定
			spriteRenderer.sortingLayerName = "Weapon";
			spriteRenderer.sortingOrder = 0;

			// Weaponを追加
			MonoScript weaponScript = AssetDatabase.LoadAssetAtPath<MonoScript>(weaponScriptName);
			var w = (Weapon)weaponObj.AddComponent(weaponScript.GetClass());
			w.ScriptFile = weaponScriptName;
			w.Sprite = spriteRenderer;

			monster.AddWeapon(w);
		}
	}

}