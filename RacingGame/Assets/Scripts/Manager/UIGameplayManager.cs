using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGameplayManager : MonoBehaviour {

	public static UIGameplayManager Instance { get; private set; }

	public Text Score;
	public Text Multiplicateur;
	public Text Life;
	public Image PowerCharge;
	public Text CenterScreenTxt;
	public Button PowerBut;
	public Button PauseBut;


	public void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		PlayerManager.Instance.InstanceP.EventChangeHP += OnChangeHP;
		PlayerManager.Instance.InstanceP.EventChangePowerCharge += OnChangePowerCharge;
		OnChangeHP(PlayerManager.Instance.InstanceP.GetHP());

		PowerBut.onClick.AddListener(PlayerManager.Instance.InstanceP.CallPowerUp);
		
	}


	public void OnChangeScore(float NewValue)
	{
		Score.text = "Score : " + NewValue.ToString();
	}

	public void OnChangeMultiplicateur(int NewValue)
	{
		Multiplicateur.text = "Multiplicateur : " + NewValue.ToString();
	}

	public void OnChangeHP(int NewValue)
	{
		Life.text = "HP : " + NewValue.ToString();
	}

	public void OnChangePowerCharge(float NewValue)
	{
		PowerCharge.fillAmount = Mathf.Clamp01(1 - NewValue);
		if (PowerCharge.fillAmount <= 0f)
		{
			//AnimationFX;
		}
	}

	public IEnumerator PrintTimerStart()
	{
		yield break;
	}

}
