using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleVidaArmasBoss : MonoBehaviour
{
    private GameObject alvo, controladorGame;
    public GameObject boss;
    // vida
    public int vidaArma = 20;
    // Tiro
    public GameObject arma, pontArma, bala;
    [Range(0, 5)] public float cooldown = 0.3f, tempoDisparo = 3.0f;
    private float contadorCooldown, contadorDisparos;
    private bool ativaArma = false;
    public int numeroDisparos = 10;
    // Materiais
    private MeshRenderer[] renderers;
    private Material[] materiais;
    // box collider
    private BoxCollider colisor;
    // explosao
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
        // busca colisor
        colisor = GetComponent<BoxCollider>();
        colisor.enabled = false;
    }
    private void Start()
    {
        ativaArma = false;
        StartCoroutine(IntervaloDisparo(12.0f));
        StartCoroutine(AtrasaColisorArma(colisor, 10.0f));
    }
    private void Update()
    {
        MiraArmaBoss();

        // disparo armas boss
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
    private void Tiro()
    {
        Instantiate(bala, pontArma.transform.position, pontArma.transform.rotation);
        somTiro.Play();
    }
    private void MiraArmaBoss()
    {
        // Rotacao armas
        Vector3 dir = alvo.transform.position - arma.transform.position;
        dir = dir.normalized;
        arma.transform.rotation = Quaternion.LookRotation(arma.transform.forward, dir);
    }
    private IEnumerator IntervaloDisparo(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        ativaArma = true;
    }

    private IEnumerator AtrasaColisorArma(BoxCollider colisor, float delay)
    {
        yield return new WaitForSeconds(delay);
        colisor.enabled = true;
    }
    private void CausaDanosNasArmas(int dano)
    {
        if (vidaArma > 0)
        {
            vidaArma -= dano;

            foreach (Material material in materiais)
            {
                StartCoroutine(Utilidades.PiscaCorRoutine(material));
            }
        }
        if (vidaArma <= 0)
        {
            Instantiate(fxExplosionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
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
            CausaDanosNasArmas(dano);
        }
        if (colisor.gameObject.CompareTag("BalaPet"))
        {
            Destroy(colisor.gameObject);
            int dano = alvo.GetComponent<DisparoArmaPet>().danoArmaPet;

            FXExplosionHit(fxExpHitPet, colisor);
            CausaDanosNasArmas(dano);
        }
        if (colisor.gameObject.CompareTag("OrbeGiratorio"))
        {
            int dano = alvo.GetComponent<RespostaOrbeGiratorio>().danoOrbeGiratorio;

            CausaDanosNasArmas(dano);
        }
        if (colisor.gameObject.CompareTag("ProjetilSerra"))
        {
            int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;

            CausaDanosNasArmas(dano);
        }
        if (colisor.gameObject.CompareTag("Player"))
        {
            int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;

            CausaDanosNasArmas(dano);
        }
    }

    private void OnCollisionExit(Collision colisor)
    {
        if (colisor.gameObject.CompareTag("ProjetilSerra"))
        {
            int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;

            CausaDanosNasArmas(dano);
        }
    }
}
