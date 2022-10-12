using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public static AudioSource AudioPlayer;
    public AudioClip[] Sounds;
    bool CanPlaySound;
    public Image SoundImage;
    public Sprite OffSound;
    public Sprite OnSound;


    // Use this for initialization
    void Awake ()
    {
        Instance = this;
        AudioPlayer = GetComponent<AudioSource>();
        //Sound Controller
        if (PlayerPrefs.HasKey("CanPlaySound"))
        {
            bool.TryParse(PlayerPrefs.GetString("CanPlaySound"), out CanPlaySound);
        }
        else
        {
            PlayerPrefs.SetString("CanPlaySound", "true");
            CanPlaySound = true;
        }
        AudioPlayer.loop = true;
        AudioPlayer.enabled = CanPlaySound;
        if (CanPlaySound)
            SoundImage.sprite = OnSound;
        else
            SoundImage.sprite = OffSound;
        PlayMusic("menu_music");
    }

    public void PlaySound(string sound)
    {
        AudioClip clip = Sounds.FirstOrDefault(x => x.name.ToUpper() == sound.ToUpper());
        if (CanPlaySound && AudioPlayer != null)
            AudioPlayer.PlayOneShot(clip);
    }

    public void PlayMusic(string sound)
    {
        if (AudioPlayer.isPlaying && AudioPlayer.clip.name == sound) return; //Prevents starting a music track over if its already playing

        AudioClip clip = Sounds.FirstOrDefault(x => x.name.ToUpper() == sound.ToUpper());
        AudioPlayer.clip = clip;
        if (CanPlaySound)
            AudioPlayer.Play();
        else
            AudioPlayer.Stop();
    }

    public void ToggleSound()
    {
        // OtherPlayer.PlayOneShot(ButtonClick);
        CanPlaySound = !CanPlaySound;
        if (CanPlaySound)
            SoundImage.sprite = OnSound;
        else
            SoundImage.sprite = OffSound;
        PlayerPrefs.SetString("CanPlaySound", CanPlaySound.ToString());
        AudioPlayer.enabled = CanPlaySound;
        PlayMusic("menu_music");
    }


}

