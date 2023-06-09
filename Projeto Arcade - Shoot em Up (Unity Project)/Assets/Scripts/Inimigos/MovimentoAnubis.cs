using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoAnubis : MonoBehaviour
{
    private GameObject alvo, controladorGame;
    public GameObject anubis, pontaArmaEsq, pontaArmaDir, balaAnubisPrefab;
    // Controle rota�ao
    public float velocidadeMovimento = 4.0f, velocidadeRotacao = 2.0f, minY = 25.0f;
    // Pontos de vida
    public int pontosVida = 20;
    // XP quando morre
    public int xpInimigo = 100;
    // Tiro
    [Range(0, 5)] public float cooldown = 0.3f, tempoDisparo = 2.0f;
    private float contadorCooldown, contadorDisparos;
    private bool ativaArma = false;
    public int numeroDisparos = 3;
    public float atrasaDisparos = 0.0f, velocidadeProjetil = 40.0f;
    // brilho olho tiro
    public MeshRenderer render;
    private Material material;
    private Color corEmissionOriginal;
    // materiais inimgo
    private MeshRenderer[] renderers;
    private Material[] materiais;
    // efeito explos�o
    public GameObject fxExplosionPrefab, fxExpHit, fxExpHitPet;
    // SFX tiro
    public AudioSource somTiro;
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
        material = render.material;
        corEmissionOriginal = material.GetColor("_EmissionColor");
    }
    private void Start()
    {
        ativaArma = false;
        StartCoroutine(IntervaloDisparo(tempoDisparo));
    }
    private void Update()
    {
        if (Time.timeScale == 0) return;


        MovimentaInimigoAnubis();

        if (atrasaDisparos > 0)
        {
            atrasaDisparos -= Time.deltaTime;
            return;
        }

        // disparo armas anubis
        if (ativaArma)
        {
            // Cooldown e controle tiro
            Utilidades.CalculaCooldown(contadorCooldown);
            contadorCooldown = Utilidades.CalculaCooldown(contadorCooldown);
            if (contadorCooldown == 0)
            {
                Tiro();
                contadorCooldown = cooldown;
                contadorDisparos++;
                if (contadorDisparos < numeroDisparos) return;
                else
                {
                    contadorDisparos = 0;
                    ativaArma = false;
                    StartCoroutine(IntervaloDisparo(tempoDisparo));
                }
            }
        }
    }
    private IEnumerator IntervaloDisparo(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        ativaArma = true;
    }
    private void MovimentaInimigoAnubis()
    {
        if (anubis.transform.position.y > minY)
        {
            // deslocamento Entrada
            anubis.transform.Translate(0, -velocidadeMovimento * Time.deltaTime, 0, Space.World);
        }
        // rotacao cabeca
        Vector3 direcao = alvo.transform.position - anubis.transform.position;
        direcao = direcao.normalized;
        anubis.transform.up = Vector3.Slerp(anubis.transform.up, -1f * direcao, velocidadeRotacao * Time.deltaTime);
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
    private void OnCollisionExit(Collision colisor)
    {
        if (colisor.gameObject.CompareTag("ProjetilSerra"))
        {
            int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;

            CaluclaDanoInimigo(dano);
        }
    }

    // Tiro
    private void Tiro()
    {
        somTiro.Play();
        StartCoroutine(BrilhaOlhoTiro());
        GameObject instaciaEsq = Instantiate(balaAnubisPrefab, pontaArmaEsq.transform.position, pontaArmaEsq.transform.rotation);
        GameObject instaciaDir = Instantiate(balaAnubisPrefab, pontaArmaDir.transform.position, pontaArmaDir.transform.rotation);
        BalaPersonagem statusEsq = instaciaEsq.GetComponent<BalaPersonagem>();
        BalaPersonagem statusDir = instaciaDir.GetComponent<BalaPersonagem>();
        statusEsq.velocidade = velocidadeProjetil;
        statusDir.velocidade = velocidadeProjetil;
    }

    private IEnumerator BrilhaOlhoTiro()
    {
        material.SetColor("_EmissionColor", new Color(0.0f, 7.5f, 0.6f));
        yield return new WaitForSeconds(cooldown/2);
        material.SetColor("_EmissionColor", corEmissionOriginal);
    }
}
