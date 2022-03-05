using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SLywnow;
//using SimpleJSON;

namespace AutoLangEditorSLywnow
{
	[InitializeOnLoad]
	public class ALSL_Editor_OnStartProject
	{
		
		static ALSL_Editor_OnStartProject()
		{
			
			ALSL_ToSaveJSON dw = new ALSL_ToSaveJSON();
			ALSL_Params pw = new ALSL_Params();
			if (FilesSet.CheckFile(Application.streamingAssetsPath + "/ALSL", "keys", "alsldata", false))
				dw = JsonUtility.FromJson<ALSL_ToSaveJSON>(FilesSet.LoadStream(Application.streamingAssetsPath + "/ALSL", "keys", "alsldata", false, false));
			else
				FilesSet.SaveStream(Application.streamingAssetsPath + "/ALSL", "keys", "alsldata", new string[0], false);

			if (FilesSet.CheckFile(Application.streamingAssetsPath + "/ALSL", "params", "alsldata", false))
				pw = JsonUtility.FromJson<ALSL_Params>(FilesSet.LoadStream(Application.streamingAssetsPath + "/ALSL", "params", "alsldata", false, false));
			else
				FilesSet.SaveStream(Application.streamingAssetsPath + "/ALSL", "params", "alsldata", JsonUtility.ToJson(pw, true), false);

			ALSL_Editor_System.alllangs = dw.alllangs;
			ALSL_Editor_System.keysR_alsl = dw.keysR_alsl;
			ALSL_Editor_System.keys_alsl = dw.keys_alsl;
			ALSL_Editor_System.repickR_alsl = dw.repickR_alsl;
			ALSL_Editor_System.langsvis = dw.langsvis;
			ALSL_Editor_System.deflang = dw.deflang;
			ALSL_Editor_System.assotiate = dw.assotiate;

			ALSL_Editor_System.options = pw;

			ALSL_Editor_System.allwords = new List<AutoLangSLywnow.ALSL_Language>();
			for (int i=0;i< ALSL_Editor_System.alllangs.Count;i++)
			{
				ALSL_Editor_System.allwords.Add(new AutoLangSLywnow.ALSL_Language());
				if (FilesSet.CheckFile(Application.streamingAssetsPath + "/ALSL/Langs Files", ALSL_Editor_System.alllangs[i], "json", false))
				{
					/*var R = JSON.Parse(FilesSet.LoadStream(Application.streamingAssetsPath + "/ALSL/Langs Files", ALSL_Editor_System.alllangs[i], "json", false, false));
					for (int a = 0; a < ALSL_Editor_System.keys_alsl.Count; a++)
					{
						ALSL_Editor_System.allwords[i].words.Add(R[ALSL_Editor_System.keys_alsl[a]].Value.Replace("▬", "\n"));
					}*/
					string[] loadjson = FilesSet.LoadStream(Application.streamingAssetsPath + "/ALSL/Langs Files", ALSL_Editor_System.alllangs[i], "json", false);
					for (int a = 0; a < ALSL_Editor_System.keys_alsl.Count; a++)
					{
						string getword = loadjson[a + 1].Replace("\"" + ALSL_Editor_System.keys_alsl[a] + "\": \"", "");
						getword = getword.Replace("\"", "");
						ALSL_Editor_System.allwords[i].words.Add(getword.Replace("▬", "\n"));
					}
				}
				else Debug.LogError("File not found: "+ Application.streamingAssetsPath + "/ALSL/Langs Files/"+ ALSL_Editor_System.alllangs[i]+".json");
			}
		}
	}

	
}
