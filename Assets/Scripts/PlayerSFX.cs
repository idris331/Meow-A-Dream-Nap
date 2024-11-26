using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private AudioSource _footStepAudio;

    public void PlayFootStep(bool play)
    {
        if (play)
            _footStepAudio.Play();
        else
            _footStepAudio.Stop();
    }
}
