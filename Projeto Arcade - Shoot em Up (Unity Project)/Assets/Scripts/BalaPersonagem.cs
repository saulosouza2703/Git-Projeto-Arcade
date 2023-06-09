using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaPersonagem : MonoBehaviour
{
    public float velocidade = 15.0f;
    public GameObject projetil;
    public bool destruidoPeloOrbe = false, destruidoPeloEscudo = false, rotacaoTiro = false;
    public GameObject projetilRotacao;
    public float velocidadeRotacao = 200.0f;


    void Update()
    {
        if (Time.timeScale == 0) return;

        MovimentoProjetil();

        Utilidades.DestroyOutOfScreen(transform.position, gameObject);
    }

    // Movimento bala
    void MovimentoProjetil()
    {
        projetil.transform.Translate(0, velocidade * Time.deltaTime, 0);

        if (rotacaoTiro)
        {
            projetilRotacao.transform.Rotate(Time.deltaTime * velocidadeRotacao * transform.forward);
        }
    }

    private void OnCollisionEnter(Collision colidido)
    {
        if(colidido.gameObject.CompareTag("OrbeGiratorio") && destruidoPeloOrbe)
        {
            Destroy(gameObject);
        }
        if (colidido.gameObject.CompareTag("Escudo") && destruidoPeloEscudo)
        {
            Destroy(gameObject);
        }
    }
}
