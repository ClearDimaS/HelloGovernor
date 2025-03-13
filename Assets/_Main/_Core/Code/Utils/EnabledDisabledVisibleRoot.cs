public class EnabledDisabledVisibleRoot : CulledRoot
{
    public override bool IsVisible => isEnabled;
    private bool isEnabled;

    private void OnEnable()
    {
        isEnabled = true;
    }

    private void OnDisable()
    {
        isEnabled = false;
    }
}