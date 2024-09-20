using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryView
{
    [SerializeField] private Button[] _slots;
    [SerializeField] private Image[] _slotImages;
    [SerializeField] private Image _glow;
    [SerializeField] private Image _actionList;
    [SerializeField] private Button _joinItemBtn, _giveItemBtn, _cancelActionListBtn;
    [SerializeField] private TMP_Text _actionListItemName;

    public InventoryModelView ModelView { get; private set; }

    public void Initialize(InventoryModelView modelView)
    {
        ModelView = modelView;
        ModelView.ItemsChanged += UpdateGrid;
        for(int i = 0; i < _slots.Length; i++)
        {
            int index = i;
            _slots[i].onClick.AddListener(new(() => ModelView.SlotClicked(index)));
        }
        ModelView.ChoosenItemIndex.Subscribe(UpdateChoosenSlot);
        _joinItemBtn.onClick.AddListener(new(ModelView.JoinItem));
        _giveItemBtn.onClick.AddListener(new(ModelView.GiveToFriend));
        _cancelActionListBtn.onClick.AddListener(new(ModelView.CancelActionList));
    }

    public void UpdateGrid(InventoryItem[] items)
    {
        for(int i = 0; i < Inventory.MaxItems; i++)
        {
            if (ModelView.Items[i] == null)
            {
                _slotImages[i].sprite = null;
                _slotImages[i].color = new Color(0, 0, 0, 0);
            }
            else
            {
                _slotImages[i].sprite = ModelView.Items[i].Icon;
                _slotImages[i].color = Color.white;
            }
        }
    }

    public void UpdateChoosenSlot(int index)
    {
        if(index == -1)
        {
            _actionList.gameObject.SetActive(false);
            _glow.gameObject.SetActive(false);
        }
        else
        {
            _actionList.gameObject.SetActive(true);
            _actionList.transform.localScale = Vector3.one;
            _actionList.transform.position = new Vector3(
                _slots[index].transform.position.x,
                _actionList.transform.position.y,
                0);

            _actionList.transform.localScale = Vector3.zero;
            _actionList.transform.DOScale(Vector3.one, .3f);
            _glow.gameObject.SetActive(true);
            _glow.transform.position = _slots[index].transform.position;
            _actionListItemName.text = ModelView.Items[index].Name;
        }
    }
}
