using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlamingOrange
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
        [SerializeField] private ItemData itemToAdd;
        [SerializeField] private ItemData itemToRemove;

        public bool IsPedestalSelectionEnabled { get; private set; }

        public void SetPedestalSelectionEnabled(bool enabled)
        {
            IsPedestalSelectionEnabled = enabled;
        }

        void Awake()
        {
            SetPedestalSelectionEnabled(false);
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E)) AddItem(itemToAdd);
            if (Input.GetKeyDown(KeyCode.Q)) RemoveItem(itemToRemove);
        }
    }
}



