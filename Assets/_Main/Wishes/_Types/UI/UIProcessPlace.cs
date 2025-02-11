public class UIProcessPlace : ProcessPlace
{
    public override float ProcessTime => 0.1f;
    public override bool CanAddProgress(CitizenController citizen)
    {
        return false;
    }
}