using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilidades
{
    public static Vector3 maxPersonagem = new Vector3(27.0f, 24.0f, 0.0f);
    public static Vector3 minPersonagem = new Vector3(-27.0f, -4.5f, 0.0f);

    public static Vector3 TravaPosicao(Vector3 pos)
    {
        if (pos.x > maxPersonagem.x)
        {
            pos = new Vector3(maxPersonagem.x, pos.y, pos.z);
        }
        if (pos.x < minPersonagem.x)
        {
            pos = new Vector3(minPersonagem.x, pos.y, pos.z);
        }

        if (pos.y > maxPersonagem.y)
        {
            pos = new Vector3(pos.x, maxPersonagem.y, pos.z);
        }
        if (pos.y < minPersonagem.y)
        {
            pos = new Vector3(pos.x, minPersonagem.y, pos.z);
        }

        return pos;
    }

    public static float CalculaCooldown(float contadorCooldown)
    {
        if (contadorCooldown > 0)
        {
            contadorCooldown -= Time.deltaTime;
        }
        if (contadorCooldown < 0)
        {
            contadorCooldown = 0;
        }
        return contadorCooldown;
        
    }
    // Pisca cor do objeto no Hit de algum tiro
    public static IEnumerator PiscaCorRoutine(Material material, bool isPiscaCorAtivo)
    {
        while (isPiscaCorAtivo == true)
        {
            material.color += Color.red;
            yield return new WaitForSeconds(0.2f);
            material.color -= Color.red;
            yield return new WaitForSeconds(0.2f);
            isPiscaCorAtivo = false;
        }
    }
    public static bool AtivaPiscaCor(bool isPiscaCorAtivo)
    {
        isPiscaCorAtivo = true;
        return isPiscaCorAtivo;
    }
}