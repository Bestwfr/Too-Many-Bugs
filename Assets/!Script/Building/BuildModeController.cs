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
            SpawnGhost(unit.unitGameObject);
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
                Place(cell, snapped, activeUnit.unitGameObject);
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

        private void Place(Vector2Int cell, Vector3 position, GameObject prefab)
        {
            var go = Instantiate(prefab, position, Quaternion.identity, placedParent);
            go.name = $"Unit_{activeUnit.ItemName}_{cell.x}_{cell.y}";
            occupied.Add(cell);

            var units = go.GetComponentsInChildren<IUnit>(true);
            for (int i = 0; i < units.Length; i++)
            {
                units[i].InitializeFromSO(activeUnit.turretData);
                units[i].Activate();
            }

            ExitBuildMode();
        }

        private void ExitBuildMode()
        {
            isPlacing = false;
            activeUnit = null;
            if (ghost) Destroy(ghost);
            ghost = null;
        }

        private void SpawnGhost(GameObject prefab)
        {
            ghost = Instantiate(prefab, transform);
            ghost.name = "BuildGhost";
            foreach (var sr in ghost.GetComponentsInChildren<SpriteRenderer>(true))
            {
                var c = sr.color; c.a = ghostAlpha; sr.color = c;
                sr.sortingOrder = short.MaxValue;
            }
            foreach (var col in ghost.GetComponentsInChildren<Collider2D>(true)) col.enabled = false;
            foreach (var col3D in ghost.GetComponentsInChildren<Collider>(true)) col3D.enabled = false;
        }

        private void SetGhostTint(bool valid)
        {
            foreach (var sr in ghost.GetComponentsInChildren<SpriteRenderer>(true))
            {
                var c = sr.color;
                c.a = ghostAlpha;
                c.r = 1f;
                c.g = valid ? 1f : 0.5f;
                c.b = valid ? 1f : 0.5f;
                sr.color = c;
            }
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
