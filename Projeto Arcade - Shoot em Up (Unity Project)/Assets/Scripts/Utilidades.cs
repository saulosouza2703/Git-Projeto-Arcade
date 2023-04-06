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
}
