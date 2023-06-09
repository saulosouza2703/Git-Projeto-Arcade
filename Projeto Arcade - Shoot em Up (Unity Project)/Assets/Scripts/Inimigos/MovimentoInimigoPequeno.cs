using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MovimentoInimigoPequeno : MonoBehaviour
{
    private GameObject alvo, controladorGame;
    public float velocidadeDeslocamento = 5.0f, velocidadeRotacao = 5.0f;
    // Pontos de vida
    public int pontosVida = 1;
    // XP quando morre
    public int xpInimigo = 10;
    // materiais inimgo
    private MeshRenderer[] renderers;
    private Material[] materiais;
    // efeito explosão
    public GameObject fxExplosionPrefab, fxExpHit, fxExpHitPet;
    private void Awake()
    {
        controladorGame = GameObject.FindGameObjectWithTag("ControladorGame");
        alvo = GameObject.FindGameObjectWithTag("Player");
        // Busca materiais do inimigo
        renderers = GetComponentsInChildren<MeshRenderer>();
        materiais = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materiais[i] = renderers[i].material;
        }
    }

    private void Start()
    {
        velocidadeDeslocamento *= Mathf.Pow(1.03f, controladorGame.GetComponent<ControladorGame>().nivel);
    }

    private void OnEnable()
    {
        if (controladorGame.GetComponent<ControladorGame>().nivel >= 8)
        {
            xpInimigo = 15;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        MovimentaInimigoPequeno();
    }

    private void CaluclaDanoInimigo(int dano)
    {
        if (pontosVida > 0)
        {
            pontosVida -= dano;

            foreach (Material material in materiais)
            {
                StartCoroutine(Utilidades.PiscaCorRoutine(material));
            }
        }
        if (pontosVida <= 0)
        {
            Instantiate(fxExplosionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            ControladorGame.instancia.SomaXP(xpInimigo);
        }
    }

    private void FXExplosionHit(GameObject fxExpHit, Collision colisor)
    {
        ContactPoint point = colisor.GetContact(0);
        Vector3 pos = point.point;
        Instantiate(fxExpHit, pos, colisor.transform.rotation);
    }
    private void OnCollisionEnter(Collision colisor)
    {
        if (colisor.gameObject.CompareTag("BalaPersonagem"))
        {
            Destroy(colisor.gameObject);
            int dano = alvo.GetComponent<ControlaPersonagem>().danoArmaPrincipal;

            FXExplosionHit(fxExpHit, colisor);
            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("BalaPet"))
        {
            Destroy(colisor.gameObject);
            int dano = alvo.GetComponent<DisparoArmaPet>().danoArmaPet;

            FXExplosionHit(fxExpHitPet, colisor);
            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("OrbeGiratorio"))
        {
            int dano = alvo.GetComponent<RespostaOrbeGiratorio>().danoOrbeGiratorio;

            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("ProjetilSerra"))
        {
            int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;

            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("Player"))
        {
            int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;

            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("Escudo"))
        {
            int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;

            CaluclaDanoInimigo(dano);
        }
    }

    private void MovimentaInimigoPequeno()
    {
        //Movimento de seguir jogador
        Vector3 dir = alvo.transform.position - transform.position;
        transform.position += Time.deltaTime * velocidadeDeslocamento * dir.normalized;
        //rotaçao
        transform.up = Vector3.Slerp(transform.up, -1 * dir, velocidadeRotacao * Time.deltaTime);
    }
}
