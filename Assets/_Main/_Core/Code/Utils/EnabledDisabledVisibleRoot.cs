public class EnabledDisabledVisibleRoot : CulledRoot
{
    public override bool IsVisible => isEnabled;
    private bool isEnabled;

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        isEnabled = true;
    }

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
        isEnabled = false;
    }
}