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
		Vector2 size = new Vector2(ContainerUpgrade.rect.size.x, ContainerUpgrade.rect.y / 4);
		GameObject go;
		RectTransform rt;
		for (int i = 0, iLength = Upgrades.Count; i < iLength; i++)
		{
			go = GameObject.Instantiate(PrefabUpgrade, ContainerUpgrade, false) as GameObject;
			rt = go.transform as RectTransform;

			rt.sizeDelta = size;
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
