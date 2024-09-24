using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "RasberryActivityInfo", menuName = "Activity/Rasberry")]
public class RasberryActivityInfo : ActivityInfo
{
    [SerializeField] private RasberryStarter _rasberryStarter;

    private Inventory _inventory;

    [Inject]
    private void Construct(Inventory inventory)
    {
        _inventory = inventory;
    }

    public override ActivityStarter GetActivityStarter()
    {
        if(!_inventory.HasItemByName(_inventory.Names.Basket))
        {
            return _rasberryStarter;
        }
        return base.GetActivityStarter();
    }
}
