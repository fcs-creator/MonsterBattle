using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MonsterSummonChildWindow : EditorWindow
{
	public string monsterType;
	private string monsterName = string.Empty;

	private string monsterScriptName = string.Empty;
	private string weaponScriptName = string.Empty;

	private bool waitForAssetsImport = false;

	public static void ShowWindow(string monsterType)
	{
		var window = GetWindow<MonsterSummonChildWindow>("召喚ウィンドウ");
		window.monsterType = monsterType;
		window.Show();
	}

	private void OnGUI()
	{
		GUILayout.Space(10);
		GUILayout.Label($"{monsterType}を召喚します", EditorStyles.boldLabel);

		GUILayout.Space(10);

		monsterName = EditorGUILayout.TextField("モンスターの名前", monsterName);

		GUILayout.Space(5);

		if (monsterScriptName != string.Empty && !waitForAssetsImport)
		{
			if (GUILayout.Button("召喚！"))
			{
				MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(monsterScriptName);

				GameObject go = new GameObject(monsterName);
				Monster monster = (Monster)go.AddComponent(script.GetClass());

				go.transform.position = Vector3.zero;
				go.transform.rotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;

				// Scriptファイルを保存
				monster.ScriptFile = monsterScriptName;

				// XYのscaleを1.75に
				go.transform.localScale = new Vector3(1.75f, 1.75f, 1);

				// Bodyオブジェクトを追加
				GameObject bodyObj = new GameObject("Body");
				bodyObj.transform.SetParent(go.transform);
				bodyObj.transform.localPosition = Vector3.zero;
				bodyObj.transform.localRotation = Quaternion.identity;
				bodyObj.transform.localScale = Vector3.one;

				// SpriteRendererを追加
				{
					SpriteRenderer spriteRenderer = bodyObj.AddComponent<SpriteRenderer>();
					spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/MosterBattle/Textures/Monster/" + monsterType + ".png");
					// ソーティングレイヤーをBodyに設定
					spriteRenderer.sortingLayerName = "Body";
					spriteRenderer.sortingOrder = 0;

					monster.Sprite = spriteRenderer;
				}

				//Guardオブジェクトを追加
				GameObject guardObj = new GameObject("Guard");
				guardObj.transform.SetParent(go.transform);
				guardObj.transform.localPosition = Vector3.zero;
				guardObj.transform.localRotation = Quaternion.identity;
				guardObj.transform.localScale = Vector3.one;

				// SpriteRendererを追加
				{
					SpriteRenderer spriteRenderer = guardObj.AddComponent<SpriteRenderer>();
					// ソーティングレイヤーをGuardに設定
					spriteRenderer.sortingLayerName = "Guard";
					spriteRenderer.sortingOrder = 0;
				}

				// Guardを追加
				go.AddComponent<Guard>();

				// Weaponを追加
				CreateNewWeaponScript.CreateWeaponObject(monster, weaponScriptName);
				/*
				GameObject weaponObj = new GameObject("Weapon");
				weaponObj.transform.SetParent(go.transform);    // 子にする意味はあんまりないかもね
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
				*/

				monsterScriptName = string.Empty;
				weaponScriptName = string.Empty;

				Close();
			}
		}

		if (waitForAssetsImport)
		{
			EditorGUILayout.HelpBox("読み込み中", MessageType.Info);
		}

		if (monsterScriptName == string.Empty && !waitForAssetsImport)
		{
			if (monsterName == string.Empty)
			{
				EditorGUILayout.HelpBox("モンスターの名前を入力してください", MessageType.Warning);
			}
			else
			{
				if (GUILayout.Button("決定"))
				{
					//同じ名前のMonsterがいる場合はNG
					if (GameObject.Find(monsterName) != null)
					{
						EditorUtility.DisplayDialog("エラー", "同じゲームオブジェクト名のモンスターが存在します", "OK");
						return;
					}

					CreateNewMonsterScript(monsterName, monsterType);
					var util = new CreateNewWeaponScript();
					util.Create(monsterName);

					weaponScriptName = util.WeaponScriptName;

					AssetDatabase.ImportAsset(monsterScriptName, ImportAssetOptions.ForceUpdate);
					AssetDatabase.ImportAsset(weaponScriptName, ImportAssetOptions.ForceUpdate);

					waitForAssetsImport = true;
					EditorApplication.update += WaitForSeconds;

				}
			}
		}

	}

	private void CreateNewMonsterScript(string monsterName, string monsterType)
	{
		string className = "MonsterTemplate";
		string fileName = monsterName + ".cs";

		string monsterDir = Path.Combine(EditorConst.ScriptPath, "MonsterScripts");

#if false // ここを生かすとタイプ別のサンプルをコピーすることもできるよ
		// Monster名がDefaultMonsterの場合
		if (monsterType == EditorConst.DefaultMonsterName01 ||
			monsterType == EditorConst.DefaultMonsterName02 ||
			monsterType == EditorConst.DefaultMonsterName03)
		{
			className = monsterType;
			// monsterName にランダムな値をつけて重複を避ける
			fileName = monsterType + "_" + Guid.NewGuid().ToString("N") + ".cs";
		}
		else
		{
			// 他のタイプは今のところ受け付けない
			EditorUtility.DisplayDialog("エラー", "不正なモンスタータイプです", "OK");
			return;
		}
#endif

		string destinationFilePath = Path.Combine(monsterDir, fileName);

		// 保存先フォルダがなければ作成
		if (!Directory.Exists(monsterDir))
		{
			Directory.CreateDirectory(monsterDir);
		}

		File.Copy(Path.Combine(EditorConst.MonsterSourceFilePath, className + ".cs"), destinationFilePath);

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

		monsterScriptName = destinationFilePath;
	}

	private static float waitTime = 2.0f; // 待機時間（秒）
	private static float elapsedTime = 0.0f;

	// この待ちが意味があるかは謎
	private void WaitForSeconds()
	{
		elapsedTime += Time.deltaTime;
		if (elapsedTime >= waitTime)
		{
			// 待機時間が経過したら次の処理を実行
			EditorApplication.update -= WaitForSeconds;
			elapsedTime = 0.0f;

			waitForAssetsImport = false;
		}
	}

}