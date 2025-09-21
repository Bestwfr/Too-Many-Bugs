using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlamingOrange
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
        [SerializeField] private GameObject inventory;

        [SerializeField] private CoinManager _coinManager;
        [SerializeField] private BuildModeController buildModeController;

        void OnEnable()
        {
            RegisterSlotCallbacks();
        }

        void OnDisable()
        {
            UnregisterSlotCallbacks();
        }

        private void RegisterSlotCallbacks()
        {
            if (slots == null) return;

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (slot == null) continue;

                slot.OnClicked.AddListener(Buy);
            }
        }

        private void UnregisterSlotCallbacks()
        {
            if (slots == null) return;

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (slot == null) continue;

                slot.OnClicked.RemoveListener(Buy);
            }
        }

        private void Buy(ItemData buying)
        {
            if (buying == null) return;

            if (buying is SO_Unit && buying.ItemCost <= _coinManager.coins)
            {
                Debug.Log("unit");
                _coinManager.RemoveCoins(buying.ItemCost);
                buying.Use(buildModeController);
                inventory.SetActive(false);
                return;
            }

            if (buying is SO_Boost && buying.ItemCost <= _coinManager.coins)
            {
                Debug.Log("boost");
                _coinManager.RemoveCoins(buying.ItemCost);
            }
            else
            {
                return;
            }
        }

        public void AddItem(ItemData itemToAdd)
        {
            if (itemToAdd == null || slots == null || slots.Count == 0) return;

            if (itemToAdd.IsStackable)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i] != null && slots[i].itemInSlot == itemToAdd && slots[i].CurrentStack < itemToAdd.MaxStack)
                    {
                        slots[i].CurrentStack++;
                        return;
                    }
                }
            }

            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] != null && slots[i].itemInSlot == null)
                {
                    slots[i].itemInSlot = itemToAdd;
                    slots[i].CurrentStack = 1;
                    return;
                }
            }
        }

        public void RemoveItem(ItemData itemToRemove)
        {
            if (itemToRemove == null || slots == null || slots.Count == 0) return;

            if (itemToRemove.IsStackable)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i] != null && slots[i].itemInSlot == itemToRemove)
                    {
                        slots[i].CurrentStack--;
                        bool emptied = false;

                        if (slots[i].CurrentStack <= 0)
                        {
                            slots[i].itemInSlot = null;
                            slots[i].CurrentStack = 0;
                            emptied = true;
                        }

                        if (emptied) CompactLeft();
                        return;
                    }
                }
            }
            else
            {
                for (int i = slots.Count - 1; i >= 0; i--)
                {
                    if (slots[i] != null && slots[i].itemInSlot == itemToRemove)
                    {
                        slots[i].itemInSlot = null;
                        slots[i].CurrentStack = 0;
                        CompactLeft();
                        return;
                    }
                }
            }
        }

        public void ShowAll()
        {
            if (slots == null) return;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] == null) continue;
                slots[i].gameObject.SetActive(true);
            }
        }

        public void FilterByTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName) || slots == null) return;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] == null) continue;
                var item = slots[i].itemInSlot;
                bool show = item != null && item.GetType().Name == typeName;
                slots[i].gameObject.SetActive(show);
            }
        }

        private void CompactLeft()
        {
            if (slots == null || slots.Count == 0) return;

            int write = 0;
            for (int read = 0; read < slots.Count; read++)
            {
                var s = slots[read];
                if (s == null) continue;

                if (s.itemInSlot != null)
                {
                    if (write != read)
                    {
                        slots[write].itemInSlot = s.itemInSlot;
                        slots[write].CurrentStack = s.CurrentStack;

                        s.itemInSlot = null;
                        s.CurrentStack = 0;
                    }
                    write++;
                }
            }

            for (int i = write; i < slots.Count; i++)
            {
                if (slots[i] == null) continue;
                slots[i].itemInSlot = null;
                slots[i].CurrentStack = 0;
            }
        }

        void Start()
        {
            inventory.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && !inventory.activeSelf)
            {
                inventory.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.E) && inventory.activeSelf)
            {
                inventory.SetActive(false);
            }
        }
    }
}



