using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AutoLangSLywnow;
using UnityEngine.Events;
using SLywnow;

[RequireComponent(typeof(Dropdown))]
public class ALSLDropDown : MonoBehaviour {

	public bool UpdateLanguages;
	public bool UpdateWithoutRestart = true;

	bool upds;

	static Dropdown dropdw;

	void Start () {
		dropdw = GetComponent<Dropdown>();
		dropdw.onValueChanged.AddListener(delegate { ValueCh(); });
		UpdLg();
	}
	void Update () {
		if (upds==ALSL_Main.forseupdateall)
		{
			//Debug.Log(upds);
			UpdLg();
			UpdateLanguages = false;
			upds = !upds;
		}
		if (UpdateLanguages) { UpdLg(); UpdateLanguages = false; }
	}

	public void UpdLg()
	{
		dropdw.ClearOptions();
		List<Dropdown.OptionData> dp = new List<Dropdown.OptionData>();
		for (int i=0;i<ALSL_Main.alllangs.Count;i++)
		{
			dp.Add(new Dropdown.OptionData());
			dp[i].text = ALSL_Main.langsvis[i];
		}
		dropdw.AddOptions(dp);
		dropdw.value = ALSL_Main.currentlang;
	}

	public void ValueCh()
	{
		int ss = SaveSystemAlt.IsIndex();
		SaveSystemAlt.StopWorkAndClose();
		SaveSystemAlt.StartWork(StartingSLAL.SSALevel);

		if (UpdateWithoutRestart)
		{
			ALSL_Main.SetLanguage(ALSL_Main.alllangs[dropdw.value]);
			ALSL_Main.currentlang = dropdw.value;
		}
		else
		{
			SaveSystemAlt.SetInt("currentlang", dropdw.value);
		}

		SaveSystemAlt.StopWorkAndClose();
		SaveSystemAlt.StartWork(ss);
	}
}
