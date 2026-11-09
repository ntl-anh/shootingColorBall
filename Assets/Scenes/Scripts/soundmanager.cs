using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundmanager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource _As;
    public static soundmanager instance;
    public AudioClip levelcomplete,shooterClick,startSound;
    void Start()
    {
        if (instance)
        {
            DestroyImmediate(this);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        // Lấy trạng thái âm thanh từ PlayerPrefs
        bool isSoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        _As.mute = !isSoundOn;

        playmysound(startSound);
    }


    // Update is called once per frame
    public void playmysound(AudioClip sound)
    {
        if (_As && sound) _As.PlayOneShot(sound);
    }
}
