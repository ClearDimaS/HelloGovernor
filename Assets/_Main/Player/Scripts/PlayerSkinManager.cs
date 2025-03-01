using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerSkinManager : MonoBehaviour
{
    [Inject] private SoundManager soundManager;
    [Inject] private PlayerDataRepository playerDataRepository;
    [Inject] private PlayerSkinConfig skinConfig;
    [Inject] private DiContainer container;
    
    [SerializeField] private Transform gfxRoot;
    [SerializeField] private RuntimeAnimatorController animatorOverride;
    
    private int skinIndex;
    private GameObject skinGO;
    private Animator animator;
    private SkinData[] skins;

    public GameObject[] Prefabs { get; private set; }
    public Animator Animator => animator;
    public int SkinCount => skins.Length;
    
    private void Start()
    {
        CreateAndSortSkinDatas();
        SpawnSkin();
    }

    private void Update()
    {
        if (playerDataRepository.SkinIndex != skinIndex)
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

        Prefabs = skins.Select(x => x.prefab).ToArray();
    }

    private void SpawnSkin()
    {
        var modedIndex = playerDataRepository.SkinIndex % skins.Length;
        skinGO = container.InstantiatePrefab(skins[modedIndex].prefab, gfxRoot);
        animator = skinGO.GetComponentInChildren<Animator>();
        animator.runtimeAnimatorController = animatorOverride;
        skinIndex = playerDataRepository.SkinIndex;
        container.InstantiateComponent<SoundPlayer>(animator.gameObject).SetClip(soundManager.PlayerFootstepsSound);
    }

    public int GetPrice(int skinIndex)
    {
        return skinConfig.skinPrice;
    }
}