using DG.Tweening;
using UnityEngine;

public class HelperVFXPlayer : MonoBehaviour
{
    [SerializeField] protected ParticleSystem[] upgradePS;

    private Animator assitantAnimator;
    private UpgradableHelper upgradable;

    private void Awake()
    {
        upgradable = GetComponentInParent<UpgradableHelper>();
        assitantAnimator = upgradable.GetComponentInChildren<Animator>();
        
        upgradable.SubscribeUpgrade(PlayUpgradeVFX);
    }

    private void Update()
    {
        foreach (var ps in upgradePS)
        {
            ps.transform.position = assitantAnimator.transform.position;
        }
    }

    private void PlayUpgradeVFX()
    {
        var rot = assitantAnimator.transform.rotation;
        assitantAnimator.transform.DORotateQuaternion(rot * Quaternion.AngleAxis(360f, Vector3.up), 0.3f);
        foreach (var ps in upgradePS)
        {
            ps.Play();
        }
    }
}