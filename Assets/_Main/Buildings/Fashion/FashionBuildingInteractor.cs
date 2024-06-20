using UnityEngine;

public class FashionBuildingInteractor : SimplePlayerPhysicsBehaviour
{
    [SerializeField] private SkinChooser skinChooser;

    protected override void OnEnter(PlayerController component)
    {
        base.OnEnter(component);
        Debug.Log($"on enter: {component}");
        skinChooser.Show();
    }
}