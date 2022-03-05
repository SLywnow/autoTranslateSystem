using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AutoLangEditorSLywnow
{
	[CustomEditor(typeof(ALSL_KeyManager))]
	public class ALSL_KeyManager : EditorWindow
	{
		string nowselected = "";
		string search = "";
		string filter = "";
		Vector2 scpos;
		Vector2 scposW;

		string renameCurrent;
		string renameNew;
		string newKey;
		List<bool> langsOn;
		List<string> texts;

		string error;
		bool langsUpdated;
		bool colorful;
		//bool sort;
		bool autosave=true;

		public enum tpe
		{
			none,add, rename, delete, showW, setText
		};
		public tpe nowtpe = tpe.none;

		void OnGUI()
		{
			GUIStyle style = new GUIStyle();
			style.richText = true;

			GUIStyle styleB = new GUIStyle();
			styleB = GUI.skin.GetStyle("Button");
			styleB.richText = true;

			if (!langsUpdated)
			{
				langsOn = new List<bool>();
				texts = new List<string>();
				for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
				{
					langsOn.Add(false);
					texts.Add("");
				}
				langsUpdated = true;
			}

			GUILayout.BeginHorizontal();
			search = EditorGUILayout.TextField("", search);
			if (GUILayout.Button("Search")) filter = search;
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical();
			colorful = EditorGUILayout.Toggle("Colorful", colorful);
			//sort = EditorGUILayout.Toggle("Sort", sort);
			autosave = EditorGUILayout.Toggle("Autosave", autosave);
			if (!string.IsNullOrEmpty(nowselected))
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("▲x5")) { ALSL_Editor_System.Move(nowselected, true, 5); if (autosave) ALSL_Editor_System.SaveFiles(); };
				if (GUILayout.Button("▲")) { ALSL_Editor_System.Move(nowselected, true); if (autosave) ALSL_Editor_System.SaveFiles(); };
				if (GUILayout.Button("▼")) { ALSL_Editor_System.Move(nowselected, false); if (autosave) ALSL_Editor_System.SaveFiles(); };
				if (GUILayout.Button("▼x5")) { ALSL_Editor_System.Move(nowselected, false, 5); if (autosave) ALSL_Editor_System.SaveFiles(); };
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			EditorGUILayout.Space();

			scpos = GUILayout.BeginScrollView(scpos, style);
			List<string> showkeys = ALSL_Editor_System.keys_alsl;

			for (int i = 0; i < showkeys.Count; i++)
			{
				string pref = "";
				string suf = "";

				if (showkeys[i].IndexOf(filter) >= 0)
				{
					if (colorful)
					{
						int c = 0;
						suf = "</color>";
						for (int a = 0; a < ALSL_Editor_System.alllangs.Count; a++)
						{
							if (!string.IsNullOrEmpty(ALSL_Editor_System.allwords[a].words[i]))
								c++;
						}
						if (c == 0)
							pref = "<color=red>";
						else if (c == ALSL_Editor_System.alllangs.Count)
							pref = "<color=green>";
						else
							pref = "<color=orange>";
					}
					else
					{
						if (nowselected == showkeys[i])
						{
							pref = "<color=green>";
							suf = "</color>";
						}
					}

					if (GUILayout.Button
							   ((nowselected == showkeys[i] ?
							   (pref + "<b>" + showkeys[i] + " </b>" + suf) : (pref + showkeys[i] + suf)), styleB))
					{
						nowselected = (nowselected == showkeys[i] ? "" : showkeys[i]);
						if (!string.IsNullOrEmpty(nowselected))
						{
							if (nowtpe == tpe.rename)
								renameCurrent = nowselected;
							if (nowtpe == tpe.showW)
							{
								for (int a = 0; a < ALSL_Editor_System.alllangs.Count; a++)
								{
									texts[a] = ALSL_Editor_System.allwords[a].words[showkeys.IndexOf(nowselected)];
								}
							}
						}
					}
				}
			}
			GUILayout.EndScrollView();

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button((nowtpe == tpe.add ? ("<b>Add</b>") : "Add"), styleB)) nowtpe = (nowtpe == tpe.add ? tpe.none : tpe.add);
			if (!string.IsNullOrEmpty(nowselected))
			{
				if (GUILayout.Button((nowtpe == tpe.rename ? ("<b>Rename</b>") : "Rename"), styleB))
				{
					if (nowtpe == tpe.rename)
					{
						nowtpe = tpe.none;
						renameCurrent = "";
						renameNew = "";
					}
					else
					{
						nowtpe = tpe.rename;
						renameCurrent = nowselected;
						renameNew = "";
					}
				}
				if (GUILayout.Button((nowtpe == tpe.delete ? ("<b>Delete</b>") : "Delete"), styleB)) nowtpe = (nowtpe == tpe.delete ? tpe.none : tpe.delete);
				if (GUILayout.Button((nowtpe == tpe.showW || nowtpe == tpe.setText ? ("<b>Set Texts</b>") : "Set Texts"), styleB))
				{
					nowtpe = (nowtpe == tpe.showW ? tpe.none : tpe.showW);
					for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
					{
						texts[i] = ALSL_Editor_System.allwords[i].words[ALSL_Editor_System.keys_alsl.IndexOf(nowselected)];
					}
				}
			}
			else
			{
				if (nowtpe == tpe.rename || nowtpe == tpe.delete || nowtpe == tpe.showW || nowtpe == tpe.setText)
					nowtpe = tpe.none;
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();

			if (!string.IsNullOrEmpty(error))
				EditorGUILayout.LabelField("<color=red>" + error + "</color>", style);

			EditorGUILayout.Space();
			if (nowtpe == tpe.add)
			{
				EditorGUILayout.Space();
				newKey = EditorGUILayout.TextField("New key", newKey);
				if (GUILayout.Button("Add"))
				{
					if (string.IsNullOrEmpty(newKey)) error = "Enter some key";
					else if (ALSL_Editor_System.CheckParam(newKey, ALSL_Editor_System.CheckType.keys)) error = "This key is already exist";
					else
					{
						ALSL_Editor_System.SaveToAsset(newKey, ALSL_Editor_System.TypeOfSave.keys);
						if (autosave) ALSL_Editor_System.SaveFiles();
						newKey = "";
					}
				}
			}
			if (nowtpe == tpe.rename)
			{
				GUILayout.BeginVertical();
				renameCurrent = EditorGUILayout.TextField("Current name", renameCurrent);
				renameNew = EditorGUILayout.TextField("New name", renameNew);
				if (GUILayout.Button("Rename"))
				{
					if (string.IsNullOrEmpty(renameNew)) error = "Enter new name";
					else if (ALSL_Editor_System.CheckParam(renameNew, ALSL_Editor_System.CheckType.keys)) error = "Key with this name is already exist";
					else
					{
						ALSL_Editor_System.RenameToAsset(renameCurrent, renameNew, ALSL_Editor_System.TypeOfSave.keys);
						if (autosave) ALSL_Editor_System.SaveFiles();
						nowselected = renameNew;
						renameCurrent = nowselected;
						renameNew = "";
					}
				}
				GUILayout.EndVertical();
			}
			if (nowtpe == tpe.delete)
			{
				GUILayout.BeginVertical();
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Key to delete: <b>" + nowselected + "</b>", style);
				EditorGUILayout.LabelField("Are you sure you want to <b>delete</b> this key?", style);
				EditorGUILayout.Space();

				GUILayout.BeginHorizontal();
				if (GUILayout.Button("No")) nowtpe = tpe.none;
				if (GUILayout.Button("Yes"))
				{
					ALSL_Editor_System.RemoveToAsset(nowselected, ALSL_Editor_System.TypeOfSave.keys);
					if (autosave) ALSL_Editor_System.SaveFiles();
					nowtpe = tpe.none;
				}
				GUILayout.EndHorizontal();

				GUILayout.EndVertical();
			}
			if (nowtpe == tpe.showW)
			{
				EditorGUILayout.Space();
				GUILayout.BeginVertical();
				scposW = GUILayout.BeginScrollView(scposW, style);

				for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
				{
					string prefix = "";
					string suffix = "";
					if (string.IsNullOrEmpty(ALSL_Editor_System.allwords[i].words[ALSL_Editor_System.keys_alsl.IndexOf(nowselected)]))
					{
						prefix = "<color=red>";
						suffix = "</color>";
					}
					if (GUILayout.Button
							   ((langsOn[i] ? (prefix + "<b>" + ALSL_Editor_System.alllangs[i] + " </b>" + suffix) : (prefix + ALSL_Editor_System.alllangs[i] + suffix)), styleB))
						langsOn[i] = langsOn[i] ? false : true;

					if (langsOn[i])
					{
						texts[i] = GUILayout.TextArea(texts[i]);
						EditorGUILayout.Space();
					}
				}

				GUILayout.EndScrollView();
				EditorGUILayout.Space();
				if (GUILayout.Button("Save"))
				{
					for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
					{
						ALSL_Editor_System.allwords[i].words[ALSL_Editor_System.keys_alsl.IndexOf(nowselected)] = texts[i];
					}
					if (autosave) ALSL_Editor_System.SaveFiles();
				}
				GUILayout.EndVertical();
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if (!autosave)
				if (GUILayout.Button("Save all"))
				{
					ALSL_Editor_System.SaveFiles();
				}
		}

		static bool needToReOrder(string s1, string s2)
		{
			for (int i = 0; i < (s1.Length > s2.Length ? s2.Length : s1.Length); i++)
			{
				if (s1.ToCharArray()[i] < s2.ToCharArray()[i]) return false;
				if (s1.ToCharArray()[i] > s2.ToCharArray()[i]) return true;
			}
			return false;
		}
	}


	public class ALSLOpenWindManager : Editor
	{
		[MenuItem("SLywnow/AutoLang Key Manager (beta)")]
		static void SetDirection()
		{
			EditorWindow.GetWindow(typeof(ALSL_KeyManager), false, "AutoLang Key Manager", true);
		}
	}
}