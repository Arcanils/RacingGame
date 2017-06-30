using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
///		Manager of the ui on the gameplay scene
/// </summary>
public class UIGameplayManager : MonoBehaviour
{

	public static UIGameplayManager Instance { get; private set; }

	public bool IsPaused { get; private set; }

	public Text Score;
	public Text DistanceText;
	public RectTransform ContainerLife;
	public string IDBlockLife = "BlockHP";
	public Image PowerCharge;
	public Text CenterScreenTxt;
	public CanvasGroup CanvasFadeCenterTxt;
	public Button PowerBut;
	public Button PauseBut;

	public RectTransform ContainerMenuHome;
	public GameObject Mask;
	public Button QuitBut;
	public Button ResumeBut;
	public Button HomeBut;

	public CanvasGroup CanvasFade;
	public float DurationFade = 1f;
	public float DurationDecompte = 3f;

	public RectTransform ContainerMenuEnd;
	public Button QuitEndBut;
	public Button ContinueEndBut;

	public RectTransform ContainerEndMenu;

	public Text CurrencyWinTxt;
	public Text CurrencyTotalTxt;
	public Button EndGameBut;

	public CanvasGroup CanvasSlide1;
	public CanvasGroup CanvasSlide2;
	public GameObject PanelTuto;

	private List<Image> _listImageBlockLife = new List<Image>();
	private float _oldTimeScale;
	public void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		PlayerManager.Instance.InstanceP.EventChangeHP += OnChangeHP;
		PlayerManager.Instance.InstanceP.EventNewMaxHP += FillHP;
		PlayerManager.Instance.InstanceP.EventChangePowerCharge += OnChangePowerCharge;
		PlayerManager.Instance.InstanceP.EventChangeDistance += OnChangeDistance;
		PlayerManager.Instance.InstanceP.EventChangeScore += OnChangeScore;
		FillHP(PlayerManager.Instance.InstanceP.GetHP());

		PowerBut.onClick.AddListener(PlayerManager.Instance.InstanceP.CallPowerUp);
		QuitBut.onClick.AddListener(Quit);
		ResumeBut.onClick.AddListener(() => SetActiveMenu(ContainerMenuHome, false));
		HomeBut.onClick.AddListener(() => SetActiveMenu(ContainerMenuHome, true));

		QuitEndBut.onClick.AddListener(() => {StartCoroutine(AnimationCurrencyDisplay()); QuitEndBut.onClick.RemoveAllListeners(); });
		ContinueEndBut.onClick.AddListener(ContinueAfterLoose);


		EndGameBut.onClick.AddListener(Quit);

	}


	public void OnChangeScore(int NewValue)
	{
		Score.text = "Score : " + NewValue.ToString();
	}

	public void OnChangeDistance(int NewValue)
	{
		DistanceText.text = NewValue.ToString() + " km";
	}

	public void OnChangeHP(int NewValue)
	{
		for (int i = 0, iLength = _listImageBlockLife.Count; i < iLength; ++i)
		{
			_listImageBlockLife[i].color = (iLength - i) <= NewValue ? Color.green : Color.gray;
		}
	}

	public void OnChangePowerCharge(float NewValue)
	{
		PowerCharge.fillAmount = Mathf.Clamp01(1 - NewValue);
		if (PowerCharge.fillAmount <= 0f)
		{
			//AnimationFX;
		}
	}

	private void FillHP(int HP)
	{
		//Debug.LogError(HP);
		EmptyHP();
		GameObject go;
		Transform goTrans;
		for (int i = 0; i < HP; i++)
		{
			if (!PoolManager.Instance.GetObject(IDBlockLife, out go))
				throw new System.Exception();

			goTrans = go.transform;
			goTrans.SetParent(ContainerLife);

			_listImageBlockLife.Add(go.GetComponent<Image>());
		}
		OnChangeHP(HP);
	}

	private void EmptyHP()
	{
		for (int i = 0; i < _listImageBlockLife.Count; i++)
		{
			var script = _listImageBlockLife[i].GetComponent<PoolComponent>();
			if (script != null)
				script.BackToPool();
			else
				Destroy(_listImageBlockLife[i].gameObject);
		}

		_listImageBlockLife.Clear();
	}

	public IEnumerator PrintTimerStart()
	{
		yield break;
	}


	public void SetActiveMenu(RectTransform Container, bool SetActive, System.Action ActionAtEnd = null)
	{
		StartCoroutine(ShowMenu(Container, SetActive, ActionAtEnd));
	}

	public void Quit()
	{
		StartCoroutine(QuitEnum());
	}

	private IEnumerator QuitEnum()
	{
		yield return FadeEnum(true);
		Time.timeScale = 1f;
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

	public IEnumerator ShowMenu(RectTransform Container, bool In, System.Action ActionAtEnd)
	{
		if (In)
		{
			IsPaused = true;
			_oldTimeScale = Time.timeScale;
			Time.timeScale = 0f;
			Mask.SetActive(true);
			Container.gameObject.SetActive(true);
		}

		Vector2 BegPosition = Container.anchoredPosition;
		Vector2 EndPosition = In ? new Vector2(0f, Screen.height) : Vector2.zero;

		for (float t = 0f, perc = 0f; perc < 1f; t += Time.unscaledDeltaTime)
		{
			perc = Mathf.Clamp01(t / 1f);

			Container.anchoredPosition = Vector2.Lerp(BegPosition, EndPosition, Mathf.SmoothStep(0f, 1f, perc));
			yield return null;
		}

		if (!In)
		{
			Time.timeScale = _oldTimeScale;
			IsPaused = false;
			Mask.SetActive(false);
			Container.gameObject.SetActive(false);
		}
		if (ActionAtEnd != null)
			ActionAtEnd.Invoke();
	}

	private IEnumerator FadeEnum(bool In)
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

	}

	private IEnumerator FadeCanvas(CanvasGroup AlphaSetter, bool In, float Duration)
	{
		float begValue = AlphaSetter.alpha;
		float endValue = In ? 1f : 0f;
		for (float t = 0f, perc = 0f; perc < 1f; t += Time.unscaledDeltaTime)
		{
			perc = Mathf.Clamp01(t / Duration);
			AlphaSetter.alpha = Mathf.Lerp(begValue, endValue, perc);
			yield return null;
		}
	}
	private IEnumerator WaitUntilOrInput(float Duration)
	{
		for (float t = 0f, perc = 0f; perc < 1f && !Input.anyKeyDown; t += Time.unscaledDeltaTime)
		{
			perc = Mathf.Clamp01(t / Duration);
			yield return null;
		}
	}


	public IEnumerator StartGame()
	{
		yield return FadeEnum(false);

		yield return FadeCanvas(CanvasSlide1, true, 1);
		yield return WaitUntilOrInput(5f);
		yield return FadeCanvas(CanvasSlide1, false, 1);
		yield return FadeCanvas(CanvasSlide2, true, 1);
		yield return WaitUntilOrInput(5f);
		yield return FadeCanvas(CanvasSlide2, false, 1);
		PanelTuto.SetActive(false);
		CanvasFadeCenterTxt.alpha = 1f;
		for (float t = 0f; t < 3f; t += Time.unscaledDeltaTime)
		{
			CenterScreenTxt.text = Mathf.Clamp((DurationDecompte - t), 0f, DurationDecompte).ToString("0.00");
			yield return null;
		}
		CanvasFadeCenterTxt.alpha = 0f;
		CenterScreenTxt.text = "";
		CanvasFade.gameObject.SetActive(false);
	}

	public void ContinueAfterLoose()
	{
		SetActiveMenu(ContainerMenuEnd, false, PlayerManager.Instance.InstanceP.ResetHP);
	}
	public void ShowEndScreen()
	{
		SetActiveMenu(ContainerMenuEnd, true);
	}

	private IEnumerator AnimationCurrencyDisplay()
	{
		Vector2 BegPosition = ContainerEndMenu.anchoredPosition;
		Vector2 EndPosition = new Vector2(-Screen.width, 0f);

		EndGameBut.gameObject.SetActive(false);
		CurrencyTotalTxt.text = "";
		CurrencyWinTxt.text = "";

		for (float t = 0f, perc = 0f; perc < 1f; t += Time.unscaledDeltaTime)
		{
			perc = Mathf.Clamp01(t / 1f);

			ContainerEndMenu.anchoredPosition = Vector2.Lerp(BegPosition, EndPosition, Mathf.SmoothStep(0f, 1f, perc));
			yield return null;
		}
		for (float t = 0f; t < 0.5f && !Input.anyKeyDown; t += Time.unscaledDeltaTime)
		{
			yield return null;
		}
		var value = Mathf.RoundToInt(PlayerManager.Instance.InstanceP.CurrentScore / 3f);
		CurrencyWinTxt.text = "CurrencyWin  " + value;

		for (float t = 0f; t < 1f &&!Input.anyKeyDown; t += Time.unscaledDeltaTime)
		{
			yield return null;
		}
		CurrencyTotalTxt.text = "CurrencyTotal " + (value + SaveManager.Instance.Data.Currency);
		SaveManager.Instance.Data.AddCurrency(value);

		for (float t = 0f; t < 1f && !Input.anyKeyDown; t += Time.unscaledDeltaTime)
		{
			yield return null;
		}
		EndGameBut.gameObject.SetActive(true);
	}
}
