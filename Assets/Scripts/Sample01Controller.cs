using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sample01Controller : MonoBehaviour
{
	[SerializeField]
	private AudioSource field;
	[SerializeField]
	private AudioSource battle;

	[SerializeField]
	private AudioSource boy;
	[SerializeField]
	private AudioClip boyStartClip;
	[SerializeField]
	private AudioClip boyWinClip;
	[SerializeField]
	private AudioSource girl;
	[SerializeField]
	private AudioClip girlStartClip;
	[SerializeField]
	private AudioClip girlWinClip;

	[SerializeField]
	private AudioSource encounter;
	[SerializeField]
	private AudioSource resultWin;
	[SerializeField]
	private AudioSource openMenu;

	[SerializeField]
	private Toggle battleToggle;
	[SerializeField]
	private Toggle fieldToggle;
	[SerializeField]
	private Toggle menuToggle;


	[SerializeField]
	private float fieldTransitionTime = 0;
	[SerializeField]
	private float menuTransitionTime = 1;

	[SerializeField]
	private AudioMixerSnapshot grasslandSnapshot;
	[SerializeField]
	private AudioMixerSnapshot caveSnapshot;
	[SerializeField]
	private AudioMixerSnapshot menuSnapshot;

	// 逆再生、順再生用
	private float[] openMenuSamples;
	private float[] openMenuInvSamples;

	void Start()
	{
		openMenuSamples = new float[openMenu.clip.samples * openMenu.clip.channels];
		openMenu.clip.GetData(openMenuSamples, 0);

		openMenuInvSamples = new float[openMenuSamples.Length];
		System.Array.Copy(openMenuSamples, openMenuInvSamples, openMenuSamples.Length);
		System.Array.Reverse(openMenuInvSamples);
	}

	public void ChangedBattleToggle()
	{
		if (battleToggle.isOn)
		{
			StartBattle();
		}
		else
		{
			FinishBattle();
		}
	}

	public void ChangedFieldToggle()
	{
		if (fieldToggle.isOn)
		{
			caveSnapshot.TransitionTo(fieldTransitionTime);
		}
		else
		{
			grasslandSnapshot.TransitionTo(fieldTransitionTime);
		}
	}

	public void ChangedMenuToggle()
	{
		if (menuToggle.isOn)
		{
			// 逆再生の方が開いた音っぽい
			openMenu.clip.SetData(openMenuInvSamples, 0);
			openMenu.Play();
			menuSnapshot.TransitionTo(menuTransitionTime);
		}
		else
		{
			openMenu.clip.SetData(openMenuSamples, 0);
			openMenu.Play();
			grasslandSnapshot.TransitionTo(menuTransitionTime);
		}
	}

	public void StartBattle()
	{
		StartCoroutine(StartBattleCoroutine());
	}

	IEnumerator StartBattleCoroutine()
	{
		// エンカウントSE
		encounter.Play();

		// BGM
		yield return DoCrossFadeBGM(battle, field, 2);

		// ボイス
		if (Random.Range(0, 2) == 0)
		{
			boy.clip = boyStartClip;
			boy.Play();
		}
		else
		{
			girl.clip = girlStartClip;
			girl.Play();
		}
	}

	public void FinishBattle()
	{
		StartCoroutine(FinishBattleCoroutine());
	}

	IEnumerator FinishBattleCoroutine()
	{
		// WinSE
		resultWin.Play();

		// BGM
		yield return DoFadeOutBGM(battle, 1);

		// ボイス
		if (Random.Range(0, 2) == 0)
		{
			boy.clip = boyWinClip;
			boy.PlayDelayed(2);
		}
		else
		{
			girl.clip = girlWinClip;
			girl.PlayDelayed(2);
		}

		while (true)
		{
			if (resultWin.isPlaying == false) break;
			yield return null;
		}

		// BGM
		yield return DoFadeInBGM(field, 1);
	}

	IEnumerator DoCrossFadeBGM(AudioSource fadeIn, AudioSource fadeOut, float duration)
	{
		fadeIn.Play();

		float time = 0;

		while (true)
		{
			fadeIn.volume = Mathf.Lerp(0, 1, time / duration);
			fadeOut.volume = Mathf.Lerp(1, 0, time / duration);

			if (time >= duration) break;
			time += Time.deltaTime;
			yield return null;
		}

		fadeOut.Stop();
	}

	IEnumerator DoFadeOutBGM(AudioSource fadeOut, float duration)
	{
		float time = 0;

		while (true)
		{
			fadeOut.volume = Mathf.Lerp(1, 0, time / duration);

			if (time >= duration) break;
			time += Time.deltaTime;
			yield return null;
		}

		fadeOut.Stop();
	}

	IEnumerator DoFadeInBGM(AudioSource fadeIn, float duration)
	{
		fadeIn.Play();

		float time = 0;

		while (true)
		{
			fadeIn.volume = Mathf.Lerp(0, 1, time / duration);

			if (time >= duration) break;
			time += Time.deltaTime;
			yield return null;
		}
	}

}