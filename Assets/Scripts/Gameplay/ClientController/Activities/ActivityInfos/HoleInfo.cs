using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "HoleInfo", menuName = "Activity/HoleInfo")]
public class HoleInfo : ActivityInfo
{
    [SerializeField] private string _noLightReason;

    private Inventory _inventory;

    [Inject]
    private void Construct(Inventory inventory)
    {
        _inventory = inventory;
    }

    public override bool CanInteract(out string reason)
    {
        if(!_inventory.HasItemByName(_inventory.Names.Light))
        {
            reason = _noLightReason;
            return false;
        }
        return base.CanInteract(out reason);
    }
}
