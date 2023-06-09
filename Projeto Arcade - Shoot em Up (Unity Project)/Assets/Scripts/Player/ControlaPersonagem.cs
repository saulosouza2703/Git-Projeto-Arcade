//using OpenCover.Framework.Model; (deu erro no build)
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView; (deu erro no build)
using UnityEngine;

public class ControlaPersonagem : MonoBehaviour
{
    // Controle movimento personagem e armas
    private float x, y;
    public float velocidadeMovimento = 1.0f, velocidadeCorMaterial = 2.0f, timerInvulneravel = 0.0f;
    public GameObject personagem, armaPrincipal, armaPets, petEsq, petDir, pontaPetEsq, pontaPetDir, armaOrbeGiratorioBase, armaOrbeGiratorio, upgradeOrbe1, armaSerra;
    private GameObject alvoPet;
    public float velocidadeRotacaoPet = 2.0f, distanciaMinPetAtirar = 20.0f, velocidadeRotacaoOrbeGiratorio = 15.0f;
    // Pontos de vida
    public int pontosVida = 3;
    public int danoContato = 5;
    // Dano arma
    public int danoArmaPrincipal = 2;
    // Particulas
    public ParticleSystem particulasDano;
    // Materias player
    private Material[] materiais;
    private Color[] coresOriginais;
    // sons player
    public AudioSource tomaDano;
    public bool isInvulneravel = false;
    // cheat codes
    private bool cheatVida = false;

    void Start()
    {
        // Cursor Config
        // Cursor.lockState = CursorLockMode.Confined;

        // Busca materiais do personagem
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        materiais = new Material[renderers.Length];
        coresOriginais = new Color[renderers.Length];
        for(int i = 0; i < renderers.Length; i++) 
        {
            materiais[i] = renderers[i].material;
            coresOriginais[i] = materiais[i].color;
        }
    }

    void Update()
    {
        // cheat vida
        if (Input.GetButtonDown("cheat1"))
        {
            if (!cheatVida)
            {
                cheatVida = true;
            }
            else
            {
                cheatVida = false;
            }
        }

        if (Time.timeScale == 0) return;

        if (Input.GetMouseButtonDown(1))
        {
            pontosVida += 1;
        }

        ControleMovimentoPersonagem();

        armaPrincipal = GameObject.FindWithTag("ArmaPrincipal");
        ControleArmaPrincipal(armaPrincipal);

        //mudança da cor do material
        RetornaCorOriginal();

        if (armaPets.activeSelf)
        {
            ControleArmaPets();
        }

        if (armaOrbeGiratorioBase.activeSelf)
        {
            ControleOrbeGiratorio();
        }
    }

    // Dano Inimigos
    private void OnCollisionEnter(Collision colisor)
    {
        if (cheatVida) return;

        if (colisor.gameObject.CompareTag("Inimigo") || colisor.gameObject.CompareTag("BalaPiramide") || colisor.gameObject.CompareTag("BalaBossPiramide") ||
            colisor.gameObject.CompareTag("BalaAnubis") || colisor.gameObject.CompareTag("LaserSparks") || colisor.gameObject.CompareTag("BalaBossFase2") || colisor.gameObject.CompareTag("Tornado")
             || colisor.gameObject.CompareTag("BalaBalanca"))
        {
            if (pontosVida > 0)
            {
                if (!isInvulneravel)
                {
                    ReceberDano();
                }
                
                if (colisor.gameObject.CompareTag("BalaPiramide") || colisor.gameObject.CompareTag("BalaBossPiramide") || colisor.gameObject.CompareTag("BalaAnubis") || 
                    colisor.gameObject.CompareTag("BalaBossFase2") || colisor.gameObject.CompareTag("Tornado") || colisor.gameObject.CompareTag("BalaBalanca"))
                {
                    Destroy(colisor.gameObject);
                }
            }
            isInvulneravel = true;
            StartCoroutine(Invulnerabilidade());
        }
    }

    // Morte Personagem
    public void MorteJogador()
    {
        EfeitoTomaDano();
        gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    // Controle movimento personagem
    private void ControleMovimentoPersonagem()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0)
        {
            personagem.transform.Translate(x * Time.deltaTime * velocidadeMovimento, y * Time.deltaTime * velocidadeMovimento, 0f, Space.World);
            personagem.transform.position = Utilidades.TravaPosicao(personagem.transform.position);
        }
    }

    // Controle rotacao arma principal
    private void ControleArmaPrincipal(GameObject armaPrincipalAtiva)
    {
        Vector3 position = Input.mousePosition;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, 30f));
        Vector3 dirMouse = mousePos - armaPrincipalAtiva.transform.position;
        dirMouse = dirMouse.normalized;
        armaPrincipalAtiva.transform.rotation = Quaternion.LookRotation(armaPrincipalAtiva.transform.forward, dirMouse);
    }

    // Achar inimigo mais perto
    public GameObject AcharInimigoMaisPerto()
    {
        GameObject[] todosInimigos;
        todosInimigos = GameObject.FindGameObjectsWithTag("Inimigo");
        GameObject inimigoMaisProximo = null;
        float distancia = Mathf.Infinity;
        Vector3 posicao = armaPets.transform.position;
        foreach (GameObject inimigoPerto in todosInimigos)
        {
            Vector3 diferenca = inimigoPerto.transform.position - posicao;
            float testeDistancia = diferenca.sqrMagnitude;
            if (testeDistancia < distancia)
            {
                inimigoMaisProximo = inimigoPerto;
                distancia = testeDistancia;
            }
        }
        return inimigoMaisProximo;
    }

    // controle movimento do pet
    private void ControleArmaPets()
    {
        alvoPet = AcharInimigoMaisPerto();
        if (alvoPet != null)
        {
            Vector3 dirAlvoPet = alvoPet.transform.position - armaPets.transform.position;
            float distanciaAlvo = dirAlvoPet.magnitude;
            if (distanciaAlvo > distanciaMinPetAtirar)
            {
                petEsq.transform.rotation = Quaternion.Slerp(petEsq.transform.rotation, new Quaternion(0, 0, 0, 1), velocidadeRotacaoPet * Time.deltaTime);
                petDir.transform.rotation = Quaternion.Slerp(petDir.transform.rotation, new Quaternion(0, 0, 0, 1), velocidadeRotacaoPet * Time.deltaTime);

                pontaPetEsq.transform.rotation = Quaternion.Slerp(pontaPetEsq.transform.rotation, new Quaternion(0, 0, 0, 1), velocidadeRotacaoPet * Time.deltaTime);
                pontaPetDir.transform.rotation = Quaternion.Slerp(pontaPetDir.transform.rotation, new Quaternion(0, 0, 0, 1), velocidadeRotacaoPet * Time.deltaTime);
            }
            if (distanciaAlvo <= distanciaMinPetAtirar)
            {

                // (opcao de rotacao instantanea) gameObject.transform.rotation = Quaternion.LookRotation(pontaPetDir.transform.forward, dirAlvoPetDir);

                Vector3 dirAlvoPetEsq = alvoPet.transform.position - pontaPetEsq.transform.position;
                dirAlvoPetEsq = dirAlvoPetEsq.normalized;
                petEsq.transform.up = Vector3.Slerp(petEsq.transform.up, dirAlvoPetEsq, velocidadeRotacaoPet * Time.deltaTime);
                pontaPetEsq.transform.up = Vector3.Slerp(pontaPetEsq.transform.up, dirAlvoPetEsq, velocidadeRotacaoPet * Time.deltaTime);

                Vector3 dirAlvoPetDir = alvoPet.transform.position - pontaPetDir.transform.position;
                dirAlvoPetDir = dirAlvoPetDir.normalized;
                petDir.transform.up = Vector3.Slerp(petDir.transform.up, dirAlvoPetDir, velocidadeRotacaoPet * Time.deltaTime);
                pontaPetDir.transform.up = Vector3.Slerp(pontaPetDir.transform.up, dirAlvoPetDir, velocidadeRotacaoPet * Time.deltaTime);
            }
        }
        if (alvoPet == null)
        {
            petEsq.transform.rotation = Quaternion.Slerp(petEsq.transform.rotation, new Quaternion(0, 0, 0, 1), velocidadeRotacaoPet * Time.deltaTime);
            petDir.transform.rotation = Quaternion.Slerp(petDir.transform.rotation, new Quaternion(0, 0, 0, 1), velocidadeRotacaoPet * Time.deltaTime);

            pontaPetEsq.transform.rotation = Quaternion.Slerp(pontaPetEsq.transform.rotation, new Quaternion(0, 0, 0, 1), velocidadeRotacaoPet * Time.deltaTime);
            pontaPetDir.transform.rotation = Quaternion.Slerp(pontaPetDir.transform.rotation, new Quaternion(0, 0, 0, 1), velocidadeRotacaoPet * Time.deltaTime);
        }
    }

    // Arma orbe giratorio
    private void ControleOrbeGiratorio()
    {
        if (!upgradeOrbe1.activeSelf)
        {
            armaOrbeGiratorio.transform.RotateAround(transform.position, transform.forward, velocidadeRotacaoOrbeGiratorio * Time.deltaTime);
        }
        if (upgradeOrbe1.activeSelf)
        {
            upgradeOrbe1.transform.RotateAround(transform.position, transform.forward, velocidadeRotacaoOrbeGiratorio * Time.deltaTime);
        }
    }

    // Muda cor para vermelho
    public void EfeitoTomaDano()
    {
        if (particulasDano)
            particulasDano.Play();

        foreach (Material mat in materiais)
        {
            mat.color += Color.red;
        }
    }
    

    // Calcula dano, mudar cor e particula de dano
    public void ReceberDano()
    {
        EfeitoTomaDano();
        tomaDano.Play();
        int dano = 1;
        pontosVida -= dano;
    }

    private void RetornaCorOriginal()
    {
        for (int i = 0; i < materiais.Length; i++)
        {
            materiais[i].color = Color.Lerp(materiais[i].color, coresOriginais[i], velocidadeCorMaterial * Time.deltaTime);
        }
    }
    public IEnumerator Invulnerabilidade()
    {
        yield return new WaitForSeconds(timerInvulneravel);
        isInvulneravel = false;
        StopCoroutine(Invulnerabilidade());    
    }
}
