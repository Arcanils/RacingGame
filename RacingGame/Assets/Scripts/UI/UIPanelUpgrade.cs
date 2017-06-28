using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPanelUpgrade : MonoBehaviour {

	public GameObject PrefabUpgrade;
	public RectTransform ContainerUpgrade;

	private List<UIUpgrade> _listUI;

	public void InitData(List<UpgradePlayer> Upgrades)
	{
		_listUI = new List<UIUpgrade>();
		GameObject go;
		for (int i = 0, iLength = Upgrades.Count; i < iLength; i++)
		{
			go = GameObject.Instantiate(PrefabUpgrade, ContainerUpgrade, false) as GameObject;
			var script = go.GetComponent<UIUpgrade>();
			script.InitData(Upgrades[i]);
			_listUI.Add(script);
		}
		for (int i = 0; i < _listUI.Count; i++)
		{
			_listUI[i].UpdateData();
		}
	}
}
