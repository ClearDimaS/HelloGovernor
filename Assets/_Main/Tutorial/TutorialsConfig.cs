using System;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Configs/Tutorials Config", fileName = "TutorialsConfig")]
public class TutorialsConfig : ScriptableObject
{
    [SerializeField] private LocalizedString OperatableTitleLocalized;
    [SerializeField] private LocalizedString ItemsUseLocalized;
    [SerializeField] private LocalizedString ItemsTakeLocalized;
    [SerializeField] private LocalizedString BuyLocalized;
    [SerializeField] private LocalizedString UpgradeLocalized;
    [SerializeField] private LocalizedString BuyIncomeUpgradeLocalized;
    [SerializeField] private LocalizedString UpgradeIncomeUpgradeLocalized;
    [SerializeField] private LocalizedString HelperLocalized;
    [SerializeField] private LocalizedString CashierLocalized;
    [SerializeField] private LocalizedString AssistantLocalized;
    [SerializeField] private LocalizedString ChangeSkinLocalized;
    
    public string OperatableTitle => OperatableTitleLocalized.GetLocalizedString();
    public string ItemsUse => ItemsUseLocalized.GetLocalizedString();
    public string ItemsTake => ItemsTakeLocalized.GetLocalizedString();
    public string Buy => BuyLocalized.GetLocalizedString();
    public string Upgrade => UpgradeLocalized.GetLocalizedString();
    public string BuyIncomeUpgrade => BuyIncomeUpgradeLocalized.GetLocalizedString();
    public string UpgradeIncomeUpgrade => UpgradeIncomeUpgradeLocalized.GetLocalizedString();
    public string Helper => HelperLocalized.GetLocalizedString();
    public string Cashier => CashierLocalized.GetLocalizedString();
    public string Assistant => AssistantLocalized.GetLocalizedString();
    public string ChangeSkin => ChangeSkinLocalized.GetLocalizedString();

}
