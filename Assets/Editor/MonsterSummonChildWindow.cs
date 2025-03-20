using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MonsterSummonChildWindow : EditorWindow
{
	public string monsterType;
	private string monsterName = string.Empty;

	private string scriptName = string.Empty;

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

		if (scriptName != string.Empty)
		{
			if (GUILayout.Button("召喚！"))
			{
				Debug.Log(scriptName);
				MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptName);

				GameObject go = new GameObject(monsterName);
				Monster monster = (Monster)go.AddComponent(script.GetClass());

				go.transform.position = Vector3.zero;
				go.transform.rotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;

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

				scriptName = string.Empty;

				Close();
			}
		}

		if (scriptName == string.Empty)
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

					CreateNewUnitScript(monsterName, monsterType);
				}
			}
		}

	}

	private void CreateNewUnitScript(string monsterName, string monsterType)
	{
		string className = "Monster_CB";
		string fileName = monsterName + ".cs";

		string monsterDir = Path.Combine(EditorConst.ScriptPath, "MonsterScripts");

		// Monster名がDefaultMonsterの場合
		if (monsterType == EditorConst.DefaultMonsterName01 ||
			monsterType == EditorConst.DefaultMonsterName02 ||
			monsterType == EditorConst.DefaultMonsterName03)
		{
			className = monsterType;
			// monsterName にランダムな値をつけて重複を避ける
			fileName = monsterType + "_" + Guid.NewGuid().ToString("N") + ".cs";
		}

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

		scriptName = destinationFilePath;

		AssetDatabase.Refresh();

		AssetDatabase.ImportAsset(scriptName, ImportAssetOptions.ForceUpdate);

	}

}