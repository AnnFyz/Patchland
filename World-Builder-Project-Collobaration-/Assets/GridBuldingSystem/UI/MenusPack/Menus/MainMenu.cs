using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] int sceneToLoad = 1;
    [SerializeField] GameObject controls;
    [SerializeField] AudioSource audio;
    [SerializeField] float speed = 0.01f;
    private void Start()
    {
        controls.SetActive(false);
    }
    public void Play()
    {
        StartCoroutine(PlayWithSoundFadeOut());
     
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowControls()
    {
        controls.SetActive(true);
    }

    public void GoToMenu()
    {
        controls.SetActive(false);
    }

   public IEnumerator PlayWithSoundFadeOut()
    {
        while(audio.volume > 0)
        {
            audio.volume -= speed;
            Debug.Log(audio.volume);
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}
