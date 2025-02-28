using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUnderlay : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer copyFrom;
    [SerializeField] protected SpriteRenderer[] outlines;

    private void Awake()
    {
        foreach (var r in outlines)
        {
            r.sprite = copyFrom.sprite;
        }
    }
}
