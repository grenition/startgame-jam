using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "KeysActivityInfo", menuName = "Activity/Keys")]
public class KeysActivityInfo : ActivityInfo
{
    [SerializeField] private string _reason;

    private Inventory _inventory;

    [Inject]
    private void Construct(Inventory inventory)
    {
        _inventory = inventory;
    }

    public override bool CanInteract(out string reason)
    {
        reason = "";
        if(_inventory.HasItemByName(_inventory.Names.Key))
        {
            return true;
        }
        reason = _reason;
        return false;
    }
}
