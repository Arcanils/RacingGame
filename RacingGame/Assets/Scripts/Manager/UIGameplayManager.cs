using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIGameplayManager : MonoBehaviour {

	public static UIGameplayManager Instance { get; private set; }

	public bool IsPaused { get; private set; }

	public Text Score;
	public Text DistanceText;
	public RectTransform ContainerLife;
	public string IDBlockLife = "BlockHP";
	public Image PowerCharge;
	public Text CenterScreenTxt;
	public Button PowerBut;
	public Button PauseBut;

	public RectTransform ContainerMenuHome;
	public GameObject Mask;
	public Button QuitBut;
	public Button ResumeBut;
	public Button HomeBut;

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
		ResumeBut.onClick.AddListener(() => SetActiveMenu(false));
		HomeBut.onClick.AddListener(() => SetActiveMenu(true));

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
		Debug.LogError(HP);
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
			Destroy(_listImageBlockLife[i].gameObject);
		}

		_listImageBlockLife.Clear();
	}

	public IEnumerator PrintTimerStart()
	{
		yield break;
	}


	public void SetActiveMenu(bool SetActive)
	{
		StartCoroutine(ShowMenu(SetActive));
	}

	public void Quit()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

	public IEnumerator ShowMenu(bool In)
	{
		if (In)
		{
			IsPaused = true;
			_oldTimeScale = Time.timeScale;
			Time.timeScale = 0f;
			Mask.SetActive(true);
			ContainerMenuHome.gameObject.SetActive(true);
		}

		Vector2 BegPosition = ContainerMenuHome.anchoredPosition;
		Vector2 EndPosition = In ? new Vector2(0f, Screen.height) : Vector2.zero;

		for (float t = 0f, perc = 0f; perc < 1f; t += Time.unscaledDeltaTime)
		{
			perc = Mathf.Clamp01(t / 1f);

			ContainerMenuHome.anchoredPosition = Vector2.Lerp(BegPosition, EndPosition, Mathf.SmoothStep(0f, 1f, perc));
			yield return null;
		}

		if (!In)
		{
			Time.timeScale = _oldTimeScale;
			IsPaused = false;
			Mask.SetActive(false);
			ContainerMenuHome.gameObject.SetActive(false);
		}
	}
}
