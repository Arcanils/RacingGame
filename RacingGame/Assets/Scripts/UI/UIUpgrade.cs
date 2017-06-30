using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIUpgrade : MonoBehaviour {

	public RectTransform ContainerLvlUpgrade;
	public GameObject BlockLvlToDuplicate;
	public Text TextBonus;
	public Text PriceUpgrade;
	public Button BuyBut;

	private Image[] _tabImageBlockLvl;
	private UpgradePlayer _upgrade;

	public void InitData(UpgradePlayer Data)
	{
		_upgrade = Data;
		BuyBut.onClick.AddListener(Buy);
		TextBonus.text = Data.NameAbility;
		_tabImageBlockLvl = new Image[Data.Values.Length];
		_tabImageBlockLvl[0] = BlockLvlToDuplicate.GetComponentInChildren<Image>();
		for (int i = 1, iLength = Data.Values.Length; i < iLength; i++)
		{
			GameObject go = GameObject.Instantiate(BlockLvlToDuplicate, ContainerLvlUpgrade, false) as GameObject;
			_tabImageBlockLvl[i] = go.GetComponentInChildren<Image>();
		}
	}

	public void UpdateData()
	{
		for (int i = 0, iLength = _upgrade.Values.Length; i < iLength; i++)
		{
			_tabImageBlockLvl[i].color = i < _upgrade.CurrentIndexValue ? Color.green : Color.gray;
		}

		PriceUpgrade.text = _upgrade.CurrentIndexValue < _upgrade.Values.Length ? _upgrade.Values[_upgrade.CurrentIndexValue].Price.ToString() + "$" : "NONE";
	}

	public void Buy()
	{
		if (_upgrade.CanUpgrade() && SaveManager.Instance.Data.RemoveCurrency((int)_upgrade.GetValueNextUpgrade()))
		{
			_upgrade.Upgrade();
			UpdateData();
			MainMenuController.Instance.UpdateCurrency();
		}
	}
}
