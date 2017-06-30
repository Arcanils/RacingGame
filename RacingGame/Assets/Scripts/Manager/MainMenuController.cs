using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
///		Main of the start menu and upgrade menu
/// </summary>
public class MainMenuController : MonoBehaviour {

	public static MainMenuController Instance { get; private set; }

	public RectTransform ContainerUI;
	public float DurationTransitionUI = 1f;
	public RectTransform ContainerUpgrade;
	public GameObject PrefabPanelUpgrade;

	public Transform Podium;
	public Transform[] TargetsUpgrade;
	public Transform[] SubPodiumTarget;
	public float SpeedPodium = 36f;

	public float DurationFade = 1f;
	public CanvasGroup CanvasFade;

	public Text CurrencyText;

	private List<UIPanelUpgrade> _listPanelUpgrade;
	private int _currentIndexTarget = -1;

	public void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		ConfigManager.Init();
		SaveManager.Init();
		LoadData();
		UpdateCurrency();
		ResetUpgradeMenu();
		StartCoroutine(FadeEnum(false, null));
		StartCoroutine(BehaviourPodiumEnum());	
	}

	public void Play()
	{
		StartCoroutine(FadeEnum(true,() => UnityEngine.SceneManagement.SceneManager.LoadScene(1)));
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
		StartCoroutine(FadeEnum(true, () => Application.Quit()));
		
	}

	public void UpdateCurrency()
	{
		CurrencyText.text = SaveManager.Instance.Data.Currency + " $";
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

		if (!In)
			ResetUpgradeMenu();
	}

	private void ResetUpgradeMenu()
	{
		ContainerUpgrade.gameObject.SetActive(false);
		_currentIndexTarget = -1;
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
		_currentIndexTarget = IndexCategory;
		ContainerUpgrade.gameObject.SetActive(true);
		for (int i = 0, iLength = _listPanelUpgrade.Count; i < iLength; i++)
		{
			_listPanelUpgrade[i].gameObject.SetActive(i == IndexCategory);
		}
	}
	

	private IEnumerator AnimationPodiumEnum(int indexTarget)
	{
		if (indexTarget == -1 || indexTarget == 3)
		{
			while (true)
			{
				Podium.Rotate(Podium.up, SpeedPodium * Time.deltaTime, Space.Self);
				yield return null;
			}
		}
		else
		{
			Quaternion BegRot = Podium.rotation;
			Quaternion EndRot = Quaternion.Euler((new Vector3(0, 120f, 0f)) * indexTarget);
			for (float t = 0f, perc = 0f; perc < 1f; t+= Time.deltaTime)
			{
				perc = Mathf.Clamp01(t / 0.75f);
				Podium.rotation = Quaternion.Slerp(BegRot, EndRot, perc);
				yield return null;
			}
		}
	}


	private IEnumerator BehaviourPodiumEnum ()
	{
		int previousTarget = 0;
		IEnumerator behaviour = null;
		while (true)
		{
			while (_currentIndexTarget == previousTarget)
			{
				behaviour.MoveNext();
				yield return null;
			}

			behaviour = AnimationPodiumEnum(_currentIndexTarget);
			previousTarget = _currentIndexTarget;
		}
	}

	private IEnumerator FadeEnum(bool In, System.Action FctAtEnd)
	{
		float begValue = CanvasFade.alpha;
		float endValue = In ? 1f : 0f;

		if (In)
			CanvasFade.gameObject.SetActive(true);

		for (float t = 0f, perc = 0f; perc < 1f; t += Time.unscaledDeltaTime)
		{
			perc = Mathf.Abs(t / DurationFade);
			CanvasFade.alpha = Mathf.Lerp(begValue, endValue, Mathf.SmoothStep(0f, 1f, perc));
			yield return null;
		}
		if (!In)
			CanvasFade.gameObject.SetActive(false);

		if (FctAtEnd != null)
			FctAtEnd.Invoke();
	}
}
