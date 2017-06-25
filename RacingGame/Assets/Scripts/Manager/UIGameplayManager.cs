using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGameplayManager : MonoBehaviour {

	public static UIGameplayManager Instance { get; private set; }

	public Text Score;
	public Text Multiplicateur;
	public Text Life;
	public Text PowerCharge;
	public Text CenterScreenTxt;
	public Button PowerBut;
	public Button PauseBut;


	public void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		PlayerBehaviour.Instance.EventChangeHP += OnChangeHP;
		OnChangeHP(PlayerBehaviour.Instance.GetHP());
		PowerBut.onClick.AddListener(PlayerBehaviour.Instance.CallPowerUp);
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
		PowerCharge.text = "PowerCharge : " + NewValue.ToString();
	}

	public IEnumerator PrintTimerStart()
	{
		yield break;
	}

}
