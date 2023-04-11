using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlaMenuInicial : MonoBehaviour
{
    public void CarregaCena(string nomeCena)
    {
        SceneManager.LoadScene(nomeCena);
    }

    public void FecharJogo()
    {
        Application.Quit();
    }
}