using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    private AudioManager audioManager;
    public bool muteAudio = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = GameManager.Instance.AudioManager;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
