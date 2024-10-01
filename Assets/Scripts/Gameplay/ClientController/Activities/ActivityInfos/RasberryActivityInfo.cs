using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "RasberryActivityInfo", menuName = "Activity/Rasberry")]
public class RasberryActivityInfo : ActivityInfo
{
    [SerializeField] private RasberryStarter _rasberryStarter;
    [SerializeField] private ActivityInfo _keysInfo;
    [SerializeField] private string _nothingHere;

    private Inventory _inventory;
    private CompletedTasks _tasks;

    [Inject]
    private void Construct(Inventory inventory, CompletedTasks tasks)
    {
        _inventory = inventory;
        _tasks = tasks;
        Debug.Log("Inventory injected");
    }

    public override bool CanInteract(out string reason)
    {
        if(!_tasks.Tasks.Contains(_keysInfo))
        {
            reason = _nothingHere;
            return false;
        }
        return base.CanInteract(out reason);
    }

    public override ActivityStarter GetActivityStarter()
    {
        Debug.Log($"Inventory: {_inventory}");
        if(_inventory.HasItemByName(_inventory.Names.Basket))
        {
            return _rasberryStarter;
        }
        return base.GetActivityStarter();
    }
}
