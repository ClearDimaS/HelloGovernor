using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FashionBuilding : BuildingBase
{
    [field: SerializeField] public Transform ActivationPlace { get; private set; }
}