using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

namespace FlamingOrange
{
    [System.Serializable]
    public class ItemClickedEvent : UnityEvent<ItemData> { }

    public class InventorySlot : MonoBehaviour, IPointerClickHandler
    {
        public ItemData itemInSlot;
        public ItemClickedEvent OnClicked;

        [SerializeField] private TextMeshProUGUI stackText;

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
            stackText.gameObject.SetActive(false);
        }

        void Update()
        {
            if (itemInSlot != null)
            {
                _showImage.sprite = itemInSlot.Icon;

                if (itemInSlot.IsStackable && CurrentStack > 1)
                {
                    stackText.gameObject.SetActive(true);
                    stackText.text = CurrentStack.ToString();
                }
                else
                {
                    stackText.gameObject.SetActive(false);
                }
            }
            else
            {
                _showImage.sprite = null;
                stackText.gameObject.SetActive(false);
            }
        }
    }
}
