using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLywnow;
//using SimpleJSON;

namespace AutoLangSLywnow
{
	public class StartingSLAL
	{

		static ALSL_ToSaveJSON keys= new ALSL_ToSaveJSON();
		static bool editor = false;
		public static int SSALevel = 0;

		public static void Restart()
		{
			OnRuntimeMethodLoad();
		}

		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void OnRuntimeMethodLoad()
		{
			//FilesSet.SaveStream(Application.streamingAssetsPath + "/ALSL", "keys", "alsldata", JsonUtility.ToJson(keys), false);
			SaveSystemAlt.StartWork(SSALevel);
			BetterStreamingAssets.Initialize();
			//SaveSystemAlt.DeleteKey("LangPath");
			FirstParamALSL fp = new FirstParamALSL();
			fp = JsonUtility.FromJson<FirstParamALSL>(LoadStreamSAP("ALSL", "params", "alsldata", false, false));
#if UNITY_EDITOR
			editor = true;
			fp.langpath = fp.langpath.Replace("#dp", Application.persistentDataPath);
			//Debug.Log(fp.langpath);

			SaveSystemAlt.SetString("LangPath", fp.langpath);
#endif

			if (SaveSystemAlt.GetInt("versionT") < fp.verison) { SaveSystemAlt.SetInt("versionT", fp.verison); editor = true; }
			if (!SaveSystemAlt.HasKey("LangPath") || editor)
			{
				fp.langpath = fp.langpath.Replace("#dp", Application.persistentDataPath);
				//Debug.Log(fp.langpath);

				SaveSystemAlt.SetString("LangPath", fp.langpath);
				SaveSystemAlt.SetInt("LangFromSystem",fp.LangFromSystem);
			}

			if (!SaveSystemAlt.HasKey("OutPutFiles") || editor)
			{
				if (string.IsNullOrEmpty(fp.output)) SaveSystemAlt.SetString("OutPutFiles", null);
				else
				{
					fp.output = fp.output.Replace("#sf", FastFind.GetDefaultPath());
					fp.output = fp.output.Replace("#project", Application.productName);
					SaveSystemAlt.SetString("OutPutFiles", fp.output);
				}
			}

			string path = SaveSystemAlt.GetString("LangPath");
			if (FilesSet.CheckFile(path, "keys", "alsldata", false) && !editor)
			{
				keys = new ALSL_ToSaveJSON();
				keys = JsonUtility.FromJson<ALSL_ToSaveJSON>(FilesSet.LoadStream(path, "keys", "alsldata", false, false));

				SetUp(path);
			}
			else
			{
				FilesSet.SaveStream(path, "keys", "alsldata", LoadStreamSAP("ALSL", "keys", "alsldata", false), false);

				keys = JsonUtility.FromJson<ALSL_ToSaveJSON>(FilesSet.LoadStream(path, "keys", "alsldata", false, false));
				for (int i=0;i<keys.alllangs.Count;i++)
					FilesSet.SaveStream(path+ "/Langs Files", keys.alllangs[i], "json", LoadStreamSAP("ALSL/Langs Files", keys.alllangs[i], "json", false), false);

				SetUp(path);
			}

			SaveSystemAlt.SaveUpdatesNotClose();
		}

		static string LoadStreamSAP(string path, string name, string razr, bool hz, bool onlyoneline)
		{
			return System.Text.Encoding.UTF8.GetString(BetterStreamingAssets.ReadAllBytes(path+"/"+name+"."+razr));
		}

		static string[] LoadStreamSAP(string path, string name, string razr, bool hz)
		{
			string[] ret=new string[1];
			ret[0] = System.Text.Encoding.UTF8.GetString(BetterStreamingAssets.ReadAllBytes(path + "/" + name + "." + razr));
			return ret;
		}

		static void SetUp(string path)
		{
			ALSL_Main.alllangs = keys.alllangs;
			ALSL_Main.keysR_alsl = keys.keysR_alsl;
			ALSL_Main.keys_alsl = keys.keys_alsl;
			ALSL_Main.repickR_alsl = keys.repickR_alsl;
			ALSL_Main.langsvis = keys.langsvis;
			ALSL_Main.deflang = keys.deflang;
			ALSL_Main.assotiate = keys.assotiate;

			ALSL_Main.isoutput = new List<bool>();
			foreach (string a in keys.alllangs)
				ALSL_Main.isoutput.Add(false);

			if (SaveSystemAlt.HasKey("currentlang"))
				ALSL_Main.currentlang = SaveSystemAlt.GetInt("currentlang");
			else
			{
				if (SaveSystemAlt.GetInt("LangFromSystem")==0)
					ALSL_Main.currentlang = keys.deflang;
				else
				{
					string sysL= Application.systemLanguage.ToString();
					ALSL_Main.currentlang = -1;
					for (int i = 0; i < ALSL_Main.assotiate.Count;i++) if (ALSL_Main.assotiate[i] == sysL) ALSL_Main.currentlang = i;
					if (ALSL_Main.currentlang==-1) ALSL_Main.currentlang = keys.deflang;
				}
				SaveSystemAlt.SetInt("currentlang", ALSL_Main.currentlang);
			}

			ALSL_Main.allwords = new List<ALSL_Language>();
			for (int i = 0; i < ALSL_Main.alllangs.Count; i++)
			{
				ALSL_Main.allwords.Add(new ALSL_Language());
				/*string json = FilesSet.LoadStream(path + "/Langs Files", ALSL_Main.alllangs[i], "json", false, false);
				var R = JSON.Parse(json);
				for (int a = 0; a < ALSL_Main.keys_alsl.Count; a++)
				{
					if (!string.IsNullOrEmpty(R[ALSL_Main.keys_alsl[a]].Value))
						ALSL_Main.allwords[i].words.Add(R[ALSL_Main.keys_alsl[a]].Value.Replace("▬", "\n"));
					else
						ALSL_Main.allwords[i].words.Add(R[ALSL_Main.keys_alsl[a]].Value);
				}*/
				string[] json = FilesSet.LoadStream(path + "/Langs Files", ALSL_Main.alllangs[i], "json", false);
				for (int a = 0; a < ALSL_Main.keys_alsl.Count; a++)
				{
					string getword = json[a + 1].Replace("\"" + ALSL_Main.keys_alsl[a] + "\": \"", "");
					getword = getword.Replace("\"", "");
					if (!string.IsNullOrEmpty(getword))
						ALSL_Main.allwords[i].words.Add(getword.Replace("▬", "\n"));
					else
						ALSL_Main.allwords[i].words.Add("");
				}
			}

			if (!string.IsNullOrEmpty(SaveSystemAlt.GetString("OutPutFiles")))
			{
				if (!FilesSet.CheckDirectory(SaveSystemAlt.GetString("OutPutFiles"))) FilesSet.CreateDirectory(SaveSystemAlt.GetString("OutPutFiles"));

				string[] names = FilesSet.GetFilesFromdirectories(SaveSystemAlt.GetString("OutPutFiles"), "LangJson",false,FilesSet.TypeOfGet.NamesOfFiles);
				if (names != null && names.Length > 0)
				{
					for (int i = 0; i < names.Length; i++)
					{
						if (names[i] != "Example")
						{
							try
							{
								ALSL_Language outlang = new ALSL_Language();

								/*string json = FilesSet.LoadStream(SaveSystemAlt.GetString("OutPutFiles"), names[i], "LangJson", false, false);
								var R = JSON.Parse(json);
								for (int a = 0; a < ALSL_Main.keys_alsl.Count; a++)
								{
									if (!string.IsNullOrEmpty(R[ALSL_Main.keys_alsl[a]].Value))
										outlang.words.Add(R[ALSL_Main.keys_alsl[a]].Value.Replace("#nl", "\n"));
									else
										outlang.words.Add(R[ALSL_Main.keys_alsl[a]].Value);
								}*/
								string[] json = FilesSet.LoadStream(SaveSystemAlt.GetString("OutPutFiles"), names[i], "LangJson", false);
								for (int a = 0; a < ALSL_Main.keys_alsl.Count; a++)
								{
									string getword = json[a + 1].Replace("\"" + ALSL_Main.keys_alsl[a] + "\": \"", "");
									getword = getword.Replace("\"", "");
									if (!string.IsNullOrEmpty(getword))
										outlang.words.Add(getword.Replace("▬", "\n"));
									else
										outlang.words.Add(getword);
								}

								if (outlang.words.Count == ALSL_Main.keys_alsl.Count)
								{

									ALSL_Main.allwords.Add(outlang);
									ALSL_Main.alllangs.Add(names[i]);
									ALSL_Main.isoutput.Add(true);
									ALSL_Main.assotiate.Add("none");
									ALSL_Main.langsvis.Add(names[i]);
								} else
									Debug.LogError("Failed to load " + names[i]);
							}
							catch
							{
								Debug.LogError("Failed to load " + names[i]);
							}
						}
					}
				} else
				{
					string[] tocopy = FilesSet.LoadStream(path + "/Langs Files", ALSL_Main.alllangs[keys.deflang], "json", false);

					for (int i = 0; i < tocopy.Length; i++) tocopy[i] = tocopy[i].Replace("▬", "#nl"); 
					FilesSet.SaveStream(SaveSystemAlt.GetString("OutPutFiles"), "Example", "LangJson",
						tocopy,
						false, false);
				}

				if (editor)
				{
					string[] tocopy = FilesSet.LoadStream(path + "/Langs Files", ALSL_Main.alllangs[keys.deflang], "json", false);

					for (int i = 0; i < tocopy.Length; i++) tocopy[i] = tocopy[i].Replace("▬", "#nl");
					FilesSet.SaveStream(SaveSystemAlt.GetString("OutPutFiles"), "Example", "LangJson",
						tocopy,
						false, false);
				}
			}

			ALSL_Main.languagebydefault = ALSL_Main.alllangs.Count;
			ALSL_Main.SetLanguage(ALSL_Main.alllangs[ALSL_Main.currentlang]); 

		}

	}

	public class FirstParamALSL
	{
		public string langpath = "";
		public int verison = 0;
		public int LangFromSystem = 0;
		public string output = "";
	}

	public class ALSL_ToSaveJSON
	{
		public List<string> alllangs = new List<string>();
		public List<string> keys_alsl = new List<string>();
		public List<string> keysR_alsl = new List<string>();
		public List<string> repickR_alsl = new List<string>();
		public List<string> langsvis = new List<string>();
		public List<string> assotiate = new List<string>();
		public int deflang = 0;
	}
}