﻿using UnityEngine;

public class ZDrawOrder : MonoBehaviour
{
    void Update()
    {
        var pos = transform.localPosition;
        pos.z = pos.y;
        transform.localPosition = pos;
    }
}
