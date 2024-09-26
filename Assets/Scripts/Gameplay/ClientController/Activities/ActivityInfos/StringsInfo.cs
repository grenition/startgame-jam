using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "StringsInfo", menuName = "Activity/StringsInfo")]
public class StringsInfo : ActivityInfo
{
    [SerializeField] private string _haventPrunerReason;

    private Inventory _inventory;

    [Inject]
    private void Construct(Inventory inventory)
    {
        _inventory = inventory;
    }

    public override bool CanInteract(out string reason)
    {
        if(!_inventory.HasItemByName(_inventory.Names.Pruner))
        {
            reason = _haventPrunerReason;
            return false;
        }
        return base.CanInteract(out reason);
    }
}
