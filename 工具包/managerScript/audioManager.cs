using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class audioManager : MonoBehaviour 
{
	private static audioManager instance;
	private static GameObject myobj;
	private static AudioSource[] myAS;
	string PlayingMainAudioName;
	//private static bool isDT=true;
	//private static bool isbf=false;

	private static float Zvol=0.4f;
	private static float Fvol=0.6f;

	private static List<AudioSource> TmpAudios;
	void Awake()
	{
		TmpAudios= new List<AudioSource> ();
		myAS = new AudioSource[2];
		myAS [0] =gameObject.AddComponent<AudioSource> () ;
		myAS [1] = gameObject.AddComponent<AudioSource> ();
		instance = this;
		DontDestroyOnLoad (this.gameObject);
		//myAS = gameObject.GetComponents<AudioSource> ();
	}

	public static audioManager GetInstance()
	{
		if (instance == null) 
		{
            myobj = new GameObject();
            myobj.name = "audioManager";
            instance = myobj.AddComponent(typeof(audioManager)) as audioManager;
           
		}
		return instance;
	}
	public void CLoseAndOpenVol(bool KG,int b)
	{
		if (myAS [b] == null)
			return;
		
		if (KG) 
		{
			myAS [b].Stop ();
		}
		else 
		{
			myAS [b].Play();
		}
	}



	public void PlayMainMuisc(string audioname)
	{
		if (myAS [0] == null)
			return;
		if (PlayingMainAudioName == audioname)
			return;
		PlayingMainAudioName = audioname;
		myAS [0].Stop ();
		AudioClip MyClip = new AudioClip ();
		MyClip=Resources.Load (audioname) as AudioClip;
		myAS[0].clip=MyClip;
		myAS[0].loop = true;
		myAS[0].Play ();
		SetVolume (Zvol, true);
		SetVolume (Fvol, false);
	}
		
	public void SetVolume(float vol,bool IsBGMusic)
	{
		if (myAS [0] == null||myAS [1] == null)
			return;
		
		if (IsBGMusic) 
		{
			myAS [0].volume = vol;
			Zvol = vol;
		}
		 else 
		{
			myAS[1].volume=vol;
			Fvol = vol;
		}
	}


	public float GetVolume(bool IsBGMusic)
	{
		if (IsBGMusic) 
		{
			return Zvol;
		}
		else 
		{
			return Fvol;
		}
	}

	public void StopLoopSoundEffect()
	{
		if (myAS [1] == null)
			return;
		myAS [1].Stop ();
		myAS [1].loop = false;
	}


	public void PlaySingleSoundEffect(string seName ,bool loop)
	{
		if (myAS [1] == null)
			return;
		AudioClip MyClip = new AudioClip ();
		MyClip = Resources.Load (seName) as AudioClip;
		myAS [1].volume = Fvol;
		myAS [1].loop = false;
		myAS [1].clip = MyClip;
		myAS [1].loop = loop;
		myAS [1].Play();
	}
		
	public void PlaySoundEffect(string seName)
	{
		if (myAS [1] == null)
			return;
		AudioClip MyClip = new AudioClip ();
		MyClip = Resources.Load (seName) as AudioClip;
		if (!myAS [1].isPlaying) 
		{
			myAS [1].volume = Fvol;
			myAS [1].loop = false;
			myAS [1].clip = MyClip;
			myAS [1].Play();
		}
		else 
		{
			AudioSource tmpAS=myobj.AddComponent<AudioSource>();
			TmpAudios.Add (tmpAS);
			tmpAS.clip = MyClip;
			tmpAS.loop = false;
			tmpAS.volume = Fvol;
			tmpAS.Play();

		}
	}

	void Update() 
	{
        if (TmpAudios != null)
        {
            if (TmpAudios.Count != 0)
            {
                List<AudioSource> Tmp = new List<AudioSource>();
                foreach (AudioSource Tmpas in TmpAudios)
                {
                    if (!Tmpas.isPlaying)
                    {
                        Destroy(Tmpas);
                        Tmp.Add(Tmpas);
                    }
                }
                foreach (AudioSource Tmpas in Tmp)
                {
                    TmpAudios.Remove(Tmpas);
                }
            }
        }	
	}
}
