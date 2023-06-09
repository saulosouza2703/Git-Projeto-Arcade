using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoBoss : MonoBehaviour
{
    private GameObject alvo;
    // Controle rota�ao
    public GameObject cabecaPiramide, corpoPiramide;
    public float velocidadeRotacao = 2.0f;
    // Controle movimento cabeca quando perde o corpo
    public float velocidadeCabeca = 1.0f, tempoParado = 1.0f;
    private Vector3 posAlvo;
    // arma cabeca
    public GameObject[] armasBoss;
    // pets boss
    public GameObject[] petsBoss;
    // vidas do boss
    private bool tomaDano = false;
    public int vidaCorpo = 40, vidaCabeca = 40;
    public bool bossIsDead = false, armaDestroyd = false;
    // tiro Boss
    public float cooldown = 1.5f;
    private int quantidadeTiros = 1;
    private float contadorCooldown;
    public GameObject bastaoBoss, centro, centroEsq, centroDir, lateralEsq, lateralDir;
    private GameObject spawnsBatDrone, uiVitoria;
    // Materiais
    private MeshRenderer[] renderers;
    private Material[] materiais;
    // Animator
    private Animator animator;
    // explosao
    public GameObject fxExplosionPrefab, fxExpHit, fxExpHitPet;
    // salvar progresso
    private GameObject progressoPlayer;

    private void Start()
    {
        alvo = GameObject.FindGameObjectWithTag("Player");
        posAlvo = new Vector3(0, 0, 0);
        progressoPlayer = GameObject.FindWithTag("ProgressoPlayer");
        // Busca materiais do inimigo
        renderers = GetComponentsInChildren<MeshRenderer>();
        materiais = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materiais[i] = renderers[i].material;
        }
        // busca animator
        animator = GetComponent<Animator>();
        uiVitoria = ControladorGame.instancia.uiVitoria;
        spawnsBatDrone = ControladorGame.instancia.nivel10;
        contadorCooldown = 5.0f;
    }


    private void Update()
    {
        if (Time.timeScale == 0) return;

        MovimentaBossPiramide();

        if (petsBoss[0] == null && petsBoss[1] == null && !spawnsBatDrone.activeSelf)
        {
            Invoke(nameof(AtivaDrones), 2.0f);
        }

        if (armasBoss[0] == null && armasBoss[1] == null && corpoPiramide != null)
        {
            // Cooldown e controle tiro
            Utilidades.CalculaCooldown(contadorCooldown);
            contadorCooldown = Utilidades.CalculaCooldown(contadorCooldown);
            if (contadorCooldown == 0)
            {
                DisparoBastoes();
                contadorCooldown = cooldown;
            }
            if (!armaDestroyd)
            {
                armaDestroyd = true;
                spawnsBatDrone.SetActive(true);
                spawnsBatDrone.transform.GetChild(0).gameObject.SetActive(false);
                Invoke(nameof(AtivaSegundoBatDrone), 4.0f);
                tomaDano = true;
            }
        }

        if(corpoPiramide == null && animator.enabled)
        {
            animator.enabled = false;
        }
    }
    private void AtivaDrones()
    {
        spawnsBatDrone.SetActive(true);
    }
    private void AtivaSegundoBatDrone()
    {
        spawnsBatDrone.transform.GetChild(1).gameObject.SetActive(true);
    }
    
    private void MovimentaBossPiramide()
    {
        Vector3 direcao = alvo.transform.position - transform.position;
        direcao = direcao.normalized;

        if (corpoPiramide != null)
        {
            // Rotacao corpo
            corpoPiramide.transform.up = Vector3.Slerp(corpoPiramide.transform.up, - direcao, velocidadeRotacao * Time.deltaTime);
            cabecaPiramide.transform.up = Vector3.Slerp(cabecaPiramide.transform.up, - direcao, velocidadeRotacao * Time.deltaTime * 2f);
        }
        if (cabecaPiramide != null)
        {
            // Mira cabeca
            Vector3 direcaoCabeca = alvo.transform.position - cabecaPiramide.transform.position;
            
            if (corpoPiramide == null && posAlvo != new Vector3(0f, 0f, 0f))
            {
                cabecaPiramide.transform.rotation = Quaternion.LookRotation(cabecaPiramide.transform.forward, - direcaoCabeca);
                Vector3 posCabeca = cabecaPiramide.transform.position;
                if (Vector3.Distance(posAlvo, posCabeca) > 0.8f)
                {
                    cabecaPiramide.transform.position = Vector3.Lerp(posCabeca, posAlvo, velocidadeCabeca * Time.deltaTime);
                    quantidadeTiros = 1;
                }
                if (Vector3.Distance(posAlvo, posCabeca) < 0.8f)
                {
                    if (quantidadeTiros > 0)
                    {
                        Invoke(nameof(DisparoBastoes), tempoParado / 32f);
                        quantidadeTiros--;
                    }
                    Invoke(nameof(BuscaNovaPosicaoPlayer), tempoParado);
                }
            }
        }
    }
    private void BuscaNovaPosicaoPlayer()
    {
        posAlvo = alvo.transform.position;
        CancelInvoke(nameof(BuscaNovaPosicaoPlayer));
    }

    private void DisparoBastoes()
    {
        Instantiate(bastaoBoss, centroEsq.transform.position, centroEsq.transform.rotation);
        GameObject instanciaLateral = Instantiate(bastaoBoss, lateralEsq.transform.position, lateralEsq.transform.rotation);
        instanciaLateral.GetComponent<BalaPersonagem>().velocidadeRotacao *= -1;
        GameObject instanciaCentro = Instantiate(bastaoBoss, centroDir.transform.position, centroDir.transform.rotation);
        instanciaCentro.GetComponent<BalaPersonagem>().velocidadeRotacao *= -1;
        Instantiate(bastaoBoss, lateralDir.transform.position, lateralDir.transform.rotation);
        Instantiate(bastaoBoss, centro.transform.position, centro.transform.rotation);
    }

    private IEnumerator AtivaMenuVitoria(GameObject uiVitoria, float delay)
    {
        yield return new WaitForSeconds(delay);
        uiVitoria.SetActive(true);
        Time.timeScale = 0.0f;
        StopAllCoroutines();
        yield break;
    }
    // controle vida boss
    private void MorteCorpo()
    {
        Instantiate(fxExplosionPrefab, corpoPiramide.transform.position, corpoPiramide.transform.rotation);
        Invoke(nameof(BuscaNovaPosicaoPlayer), tempoParado);
        Destroy(corpoPiramide);
    }
    private void MorteCabeca()
    {
        bossIsDead = true;
        StartCoroutine(AtivaMenuVitoria(uiVitoria, 4.0f));
        Instantiate(fxExplosionPrefab, cabecaPiramide.transform.position, cabecaPiramide.transform.rotation);
        progressoPlayer.GetComponent<ProgressoPlayer>().concluiuFase1 = true;
        Destroy(cabecaPiramide);
    }
    private void FXExplosionHit(GameObject fxExpHit, Collision colisor)
    {
        ContactPoint point = colisor.GetContact(0);
        Vector3 pos = point.point;
        Instantiate(fxExpHit, pos, colisor.transform.rotation);
    }
    private void OnCollisionEnter(Collision colisor)
    {
        if (tomaDano && corpoPiramide != null)
        {
            if (colisor.gameObject.CompareTag("BalaPersonagem"))
            {
                FXExplosionHit(fxExpHit, colisor);

                Destroy(colisor.gameObject);
                int dano = alvo.GetComponent<ControlaPersonagem>().danoArmaPrincipal;
                if (vidaCorpo > 0)
                {
                    vidaCorpo -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCorpo <= 0)
                {
                    MorteCorpo();
                }
            }
            if (colisor.gameObject.CompareTag("BalaPet"))
            {
                FXExplosionHit(fxExpHitPet, colisor);

                Destroy(colisor.gameObject);
                int dano = alvo.GetComponent<DisparoArmaPet>().danoArmaPet;
                if (vidaCorpo > 0)
                {
                    vidaCorpo -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCorpo <= 0)
                {
                    MorteCorpo();
                }
            }
            if (colisor.gameObject.CompareTag("OrbeGiratorio"))
            {
                int dano = alvo.GetComponent<RespostaOrbeGiratorio>().danoOrbeGiratorio;
                if (vidaCorpo > 0)
                {
                    vidaCorpo -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCorpo <= 0)
                {
                    MorteCorpo();
                }
            }
            if (colisor.gameObject.CompareTag("ProjetilSerra"))
            {
                int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;
                if (vidaCorpo > 0)
                {
                    vidaCorpo -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCorpo <= 0)
                {
                    MorteCorpo();
                }
            }
            if (colisor.gameObject.CompareTag("Player"))
            {
                int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;
                if (vidaCorpo > 0)
                {
                    vidaCorpo -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCorpo <= 0)
                {
                    MorteCorpo();
                }
            }
            if (colisor.gameObject.CompareTag("Escudo"))
            {
                int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;
                if (vidaCorpo > 0)
                {
                    vidaCorpo -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCorpo <= 0)
                {
                    MorteCorpo();
                }
            }
        }
        if (tomaDano && corpoPiramide == null)
        {
            if (colisor.gameObject.CompareTag("BalaPersonagem"))
            {
                FXExplosionHit(fxExpHit, colisor);

                Destroy(colisor.gameObject);
                int dano = alvo.GetComponent<ControlaPersonagem>().danoArmaPrincipal;
                if (vidaCabeca > 0)
                {
                    vidaCabeca -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCabeca <= 0)
                {
                    MorteCabeca();
                }
            }
            if (colisor.gameObject.CompareTag("BalaPet"))
            {
                FXExplosionHit(fxExpHitPet, colisor);

                Destroy(colisor.gameObject);
                int dano = alvo.GetComponent<DisparoArmaPet>().danoArmaPet;
                if (vidaCabeca > 0)
                {
                    vidaCabeca -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCabeca <= 0)
                {
                    MorteCabeca();
                }
            }
            if (colisor.gameObject.CompareTag("OrbeGiratorio"))
            {
                int dano = alvo.GetComponent<RespostaOrbeGiratorio>().danoOrbeGiratorio;
                if (vidaCabeca > 0)
                {
                    vidaCabeca -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCabeca <= 0)
                {
                    MorteCabeca();
                }
            }
            if (colisor.gameObject.CompareTag("ProjetilSerra"))
            {
                int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;
                if (vidaCabeca > 0)
                {
                    vidaCabeca -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCabeca <= 0)
                {
                    MorteCabeca();
                }
            }
            if (colisor.gameObject.CompareTag("Player"))
            {
                int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;
                if (vidaCabeca > 0)
                {
                    vidaCabeca -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCabeca <= 0)
                {
                    MorteCabeca();
                }
            }
            if (colisor.gameObject.CompareTag("Escudo"))
            {
                int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;
                if (vidaCabeca > 0)
                {
                    vidaCabeca -= dano;

                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCabeca <= 0)
                {
                    MorteCabeca();
                }
            }
        }
    }
    private void OnCollisionStay(Collision colisor)
    {
        if (tomaDano && corpoPiramide != null)
        {
            float contadorCooldown, cooldown = 0.5f;
            contadorCooldown = cooldown;
            Utilidades.CalculaCooldown(contadorCooldown);
            contadorCooldown = Utilidades.CalculaCooldown(contadorCooldown);
            if (colisor.gameObject.CompareTag("ProjetilSerra"))
            {
                int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;
                if (vidaCorpo > 0 && contadorCooldown == 0)
                {
                    vidaCorpo -= dano;
                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCorpo <= 0)
                {
                    MorteCorpo();
                }
            }
        }
        if (tomaDano && corpoPiramide == null)
        {
            float contadorCooldown, cooldown = 0.5f;
            contadorCooldown = cooldown;
            Utilidades.CalculaCooldown(contadorCooldown);
            contadorCooldown = Utilidades.CalculaCooldown(contadorCooldown);
            if (colisor.gameObject.CompareTag("ProjetilSerra"))
            {
                int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;
                if (vidaCabeca > 0 && contadorCooldown == 0)
                {
                    vidaCabeca -= dano;
                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCabeca <= 0)
                {
                    MorteCabeca();
                }
            }
        }
    }

    private void OnCollisionExit(Collision colisor)
    {
        if (tomaDano && corpoPiramide != null)
        {
            contadorCooldown = Utilidades.CalculaCooldown(contadorCooldown);
            if (colisor.gameObject.CompareTag("ProjetilSerra"))
            {
                int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;
                if (vidaCorpo > 0 && contadorCooldown == 0)
                {
                    vidaCorpo -= dano;
                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCorpo <= 0)
                {
                    MorteCorpo();
                }
            }
        }
        if (tomaDano && corpoPiramide == null)
        {
            contadorCooldown = Utilidades.CalculaCooldown(contadorCooldown);
            if (colisor.gameObject.CompareTag("ProjetilSerra"))
            {
                int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;
                if (vidaCabeca > 0 && contadorCooldown == 0)
                {
                    vidaCabeca -= dano;
                    foreach (Material material in materiais)
                    {
                        StartCoroutine(Utilidades.PiscaCorRoutine(material));
                    }
                }
                if (vidaCabeca <= 0)
                {
                    MorteCabeca();
                }
            }
        }
    }
}

