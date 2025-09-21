using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlamingOrange
{
    [DisallowMultipleComponent]
    public class BuildModeController : MonoBehaviour, IItemUseContext
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField, Min(0.1f)] private float cellSize = 1f;
        [SerializeField] private Vector2 gridOrigin = Vector2.zero;
        [SerializeField] private Transform placedParent;
        [SerializeField] private float ghostAlpha = 0.5f;

        private SO_Unit activeUnit;
        private GameObject ghost;
        private SpriteRenderer ghostSR;
        private bool isPlacing;
        private readonly HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        void Awake()
        {
            if (!worldCamera) worldCamera = Camera.main;
            if (!placedParent) placedParent = this.transform;
        }

        public void EnterBuildMode(SO_Unit unit)
        {
            ExitBuildMode();
            activeUnit = unit;
            isPlacing = true;
            SpawnGhost(unit.UnitSprite);
        }

        void Update()
        {
            if (!isPlacing || activeUnit == null || ghost == null) return;

            var hasPointer = Input.touchCount > 0 || Input.mousePresent;
            if (!hasPointer) return;

            Vector2 screen = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
            if (IsPointerOverUI(screen)) return;

            var world = worldCamera.ScreenToWorldPoint(screen);
            world.z = 0f;

            var cell = WorldToCell(world);
            var snapped = CellToWorld(cell);

            ghost.transform.position = snapped;

            bool valid = !occupied.Contains(cell);
            SetGhostTint(valid);

            bool placePressed =
                (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ||
                Input.GetMouseButtonDown(0);

            if (placePressed && valid)
            {
                Place(cell, snapped, activeUnit.UnitSprite);
            }

            bool cancelPressed =
                (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Canceled) ||
                Input.GetMouseButtonDown(1) ||
                Input.GetKeyDown(KeyCode.Escape);

            if (cancelPressed)
            {
                ExitBuildMode();
            }
        }

        private void Place(Vector2Int cell, Vector3 position, Sprite sprite)
        {
            var go = new GameObject($"Unit_{activeUnit.ItemName}_{cell.x}_{cell.y}");
            go.transform.SetParent(placedParent, true);
            go.transform.position = position;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            occupied.Add(cell);

            ExitBuildMode();
        }


        private void ExitBuildMode()
        {
            isPlacing = false;
            activeUnit = null;
            if (ghost) Destroy(ghost);
            ghost = null;
            ghostSR = null;
        }

        private void SpawnGhost(Sprite sprite)
        {
            ghost = new GameObject("BuildGhost");
            ghost.transform.SetParent(transform, true);
            ghostSR = ghost.AddComponent<SpriteRenderer>();
            ghostSR.sprite = sprite;
            ghostSR.sortingOrder = short.MaxValue;
            var c = ghostSR.color; c.a = ghostAlpha; ghostSR.color = c;

            var cols = ghost.GetComponentsInChildren<Collider2D>(true);
            for (int i = 0; i < cols.Length; i++) cols[i].enabled = false;
        }

        private void SetGhostTint(bool valid)
        {
            if (!ghostSR) return;
            var c = ghostSR.color;
            c.a = ghostAlpha;
            c.r = valid ? 1f : 1f;
            c.g = valid ? 1f : 0.5f;
            c.b = valid ? 1f : 0.5f;
            ghostSR.color = c;
        }

        private Vector2Int WorldToCell(Vector3 world)
        {
            float lx = world.x - gridOrigin.x;
            float ly = world.y - gridOrigin.y;
            int cx = Mathf.RoundToInt(lx / cellSize);
            int cy = Mathf.RoundToInt(ly / cellSize);
            return new Vector2Int(cx, cy);
        }

        private Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(
                gridOrigin.x + cell.x * cellSize,
                gridOrigin.y + cell.y * cellSize,
                0f);
        }

        private static bool IsPointerOverUI(Vector2 screenPos)
        {
            if (EventSystem.current == null) return false;
            var eventData = new PointerEventData(EventSystem.current) { position = screenPos };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
    }
}
