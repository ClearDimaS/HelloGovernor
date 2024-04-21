using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerSkinManager : MonoBehaviour
{
    [Inject] private PlayerDataRepository playerDataRepository;
    [Inject] private PlayerSkinConfig skinConfig;
    
    [SerializeField] private Transform gfxRoot;
    [SerializeField] private RuntimeAnimatorController animatorOverride;
    
    private int skinIndex;
    private GameObject skinGO;
    private Animator animator;
    private SkinData[] skins;

    public Animator Animator => animator;
    
    private void Start()
    {
        CreateAndSortSkinDatas();
        SpawnSkin();
    }

    private void Update()
    {
        if (playerDataRepository.GetData().skinIndex != skinIndex)
        {
            if (skinGO != null)
            {
                Destroy(skinGO);
            }
            SpawnSkin();
        }
    }

    private void CreateAndSortSkinDatas()
    {
        skins = new SkinData[skinConfig.femaleSkins.Length + skinConfig.maleSkins.Length];
        for (int i = 0; i < skins.Length; i++)
        {
            if (i % 2 == 0)
            {
                skins[i] = new SkinData(skinConfig.maleSkins[i/2]);   
            }
            else
            {
                skins[i] = new SkinData(skinConfig.femaleSkins[i/2]);
            }
        }
    }

    private void SpawnSkin()
    {
        var data = playerDataRepository.GetData();
        var modedIndex = data.skinIndex % skins.Length;
        if (modedIndex % 2 == 0)
        {
            skinGO = Instantiate(skins[modedIndex].prefab, gfxRoot);
            animator = skinGO.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = animatorOverride;
        }
        skinIndex = data.skinIndex;
    }
}