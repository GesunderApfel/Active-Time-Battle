using UnityEngine;

/// <summary>
/// Simple Singleton implementation. Holds and plays sounds for menu navigation and Win/Lose Fanfares.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BattleSoundManager : MonoBehaviour
{
    public static BattleSoundManager Instance;

    private AudioSource _source;

    [SerializeField] private AudioClip confirm;
    [SerializeField] private AudioClip cancel;
    [SerializeField] private AudioClip navigate;

    [SerializeField] private AudioClip winFanfare;
    [SerializeField] private AudioClip loseFanfare;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _source = GetComponent<AudioSource>();
        }
        else
            Destroy(gameObject);
    }

    public void PlayConfirm()
    {
        if (confirm == null)
            return;
        _source.clip = confirm;
        _source.Play();
    }

    public void PlayCancel()
    {
        if (confirm == null)
            return;
        _source.clip = cancel;
        _source.Play();
    }

    public void PlayNavigate()
    {
        if (confirm == null)
            return;
        _source.clip = navigate;
        _source.Play();
    }

    public void PlayWinFanfare()
    {
        if (confirm == null)
            return;
        _source.clip = winFanfare;
        _source.Play();
    }

    public void PlayLoseFanfare()
    {
        if (confirm == null)
            return;
        _source.clip = loseFanfare;
        _source.Play();
    }
}

