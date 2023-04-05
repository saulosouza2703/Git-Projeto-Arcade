using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disparoArmaPet : MonoBehaviour
{
    // Disparo arma
    public GameObject pontaArmaEsq, pontaArmaDir, balaPet;
    // Tiro
    [Range(0, 1)] public float cooldown = 1.0f;
    private float contadorCooldown;
    // Alterar tiro altomatico
    public bool tiroAutomatico = true;
    // Dano arma
    public float danoArmaPet = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Cooldown e controle tiro
        CalculaCooldown();
        if ((tiroAutomatico == true) && contadorCooldown == 0)
        {
            Tiro();
            contadorCooldown = cooldown;
        }
    }
    private void CalculaCooldown()
    {
        if (contadorCooldown > 0)
        {
            contadorCooldown -= Time.deltaTime;
        }
        if (contadorCooldown < 0)
        {
            contadorCooldown = 0;
        }

    }
    // Tiro
    public void Tiro()
    {
        Instantiate(balaPet, pontaArmaEsq.transform.position, pontaArmaEsq.transform.rotation);
        Instantiate(balaPet, pontaArmaDir.transform.position, pontaArmaDir.transform.rotation);
    }
}
