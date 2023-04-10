using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaPersonagem : MonoBehaviour
{
    public float velocidade = 15.0f, duracaoBala = 200.0f;
    public GameObject projetil;
    public bool rotacaoTiro = false;
    public GameObject projetilRotacao, fonteTiro;
    public float velocidadeRotacao = 200.0f, danoProjetil;

    
    void Update()
    {
        MovimentoProjetil();

        DestroiBala();
    }

    void DestroiBala()
    {
        Destroy(projetil, duracaoBala * Time.deltaTime);
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
}
