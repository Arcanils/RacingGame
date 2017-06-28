using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour {

	public RectTransform ContainerUI;
	public float DurationTransitionUI = 1f;
	public RectTransform ContainerUpgrade;
	public GameObject PrefabPanelUpgrade;


	private List<UIPanelUpgrade> _listPanelUpgrade;

	public void Start()
	{
		ConfigManager.Init();
		SaveManager.Init();
		LoadData();
		ResetUpgradeMenu();
	}

	public void Play()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}

	public void Upgrade()
	{
		StartCoroutine(AnimationTransitionMenuToUpgrade(true));
	}
	public void Back()
	{
		StartCoroutine(AnimationTransitionMenuToUpgrade(false));
	}

	public void Quit()
	{
		Application.Quit();
	}


	private IEnumerator AnimationTransitionMenuToUpgrade(bool In)
	{
		Vector2 BegPos = ContainerUI.anchoredPosition;
		Vector2 EndPos = new Vector2(In ? -ContainerUI.rect.size.x : 0f, 0f);

		for (float t = 0f, perc = 0f; perc < 1f; t += Time.deltaTime)
		{
			perc = t / DurationTransitionUI;
			ContainerUI.anchoredPosition = Vector2.Lerp(BegPos, EndPos, Mathf.SmoothStep(0, 1f, perc));
			yield return null;
		}
	}

	private void ResetUpgradeMenu()
	{
		ContainerUpgrade.gameObject.SetActive(false);
	}

	private void LoadData()
	{
		GameObject go;
		var data = ConfigManager.Instance.Config.PlayerData.Config.ListTypeUpgrade;
		_listPanelUpgrade = new List<UIPanelUpgrade>();
		for (int i = 0, iLength = data.Count; i < iLength; i++)
		{
			go = GameObject.Instantiate(PrefabPanelUpgrade, ContainerUpgrade, false) as GameObject;
			var script = go.GetComponent<UIPanelUpgrade>();
			script.InitData(data[i].ListUpgrade);
			_listPanelUpgrade.Add(script);
		}
	}

	public void ShowSubMenuForCategory(int IndexCategory)
	{
		ContainerUpgrade.gameObject.SetActive(true);
		for (int i = 0, iLength = _listPanelUpgrade.Count; i < iLength; i++)
		{
			_listPanelUpgrade[i].gameObject.SetActive(i == IndexCategory);
		}
	}
	

	private IEnumerator AnimationPodiumEnum()
	{
		yield break;
	}
}
