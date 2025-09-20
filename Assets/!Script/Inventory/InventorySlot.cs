using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Serialization;

namespace FlamingOrange
{
    [System.Serializable]
    public class ItemClickedEvent : UnityEvent<ItemData> { }

    public class InventorySlot : MonoBehaviour, IPointerClickHandler
    {
        public ItemData itemInSlot;
        public ItemClickedEvent OnClicked;

        [SerializeField, FormerlySerializedAs("stackText")]
        private TextMeshProUGUI itemCostText;

        [Header("only use when item in slot is stackable")]
        public int CurrentStack;

        private Image _showImage;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemInSlot != null)
            {
                OnClicked?.Invoke(itemInSlot);
            }
        }

        void Start()
        {
            _showImage = transform.GetChild(0).GetComponent<Image>();

            if (itemCostText != null)
            {
                itemCostText.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (itemInSlot != null)
            {
                _showImage.sprite = itemInSlot.Icon;

                if (itemCostText != null)
                {
                    bool hasCost = itemInSlot.ItemCost > 0;
                    itemCostText.gameObject.SetActive(hasCost);

                    if (hasCost)
                    {
                        itemCostText.text = itemInSlot.ItemCost.ToString();
                    }
                }
            }
            else
            {
                _showImage.sprite = null;

                if (itemCostText != null)
                {
                    itemCostText.gameObject.SetActive(false);
                }
            }
        }
    }
}
