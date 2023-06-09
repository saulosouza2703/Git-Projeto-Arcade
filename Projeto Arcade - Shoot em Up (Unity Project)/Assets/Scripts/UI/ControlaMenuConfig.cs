using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ControlaMenuConfig : MonoBehaviour
{
    public GameObject[] botoesFase;
    public AudioMixer mixer;
    // som musica
    public Slider sliderMusica;
    // som SFX
    public Slider sliderSFX;

    private void OnEnable()
    {
        // ajusta valor slider ao abrir
        mixer.GetFloat("MusicVol", out float valueMusic);
        sliderMusica.value = Mathf.Pow(10.0f, valueMusic / 20);
        mixer.GetFloat("SFXVol", out float valueSFX);
        sliderSFX.value = valueSFX;
        sliderSFX.value = Mathf.Pow(10.0f, valueSFX / 20);

        if (SceneManager.GetActiveScene().name == "Menu Inicial")
        {
            foreach(GameObject go in botoesFase)
            {
                go.SetActive(false);
            }

            //qualidade resolução ativa
        }
        if (SceneManager.GetActiveScene().name == "Fase 1" || SceneManager.GetActiveScene().name == "Fase 2")
        {
            foreach (GameObject go in botoesFase)
            {
                go.SetActive(true);
            }

            //qualidade resolução desativa
        }

    }
    private void Update()
    {
        
    }
    public void CarregaCena(string nomeCena)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(nomeCena);
    }

    public void FecharJogo()
    {
        Application.Quit();
    }

    public void PausaJogo(bool pausa)
    {
        if (pausa)
        {
            Time.timeScale = 0;
        }
        else 
        {
            Time.timeScale = 1;
        }
        
    }
    // Controla soms
    public void ControlaMusica()
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderMusica.value) * 20.0f);
    }
    public void ControlaSFX()
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(sliderSFX.value) * 20.0f);
    }

    // configurações de qualidade
    public void QualidadeBaixa()
    {
        QualitySettings.SetQualityLevel(1, true);
    }
    public void QualidadeMedia()
    {
        QualitySettings.SetQualityLevel(3, true);
    }
    public void QualidadeAlta()
    {
        QualitySettings.SetQualityLevel(5, true);
    }
}
