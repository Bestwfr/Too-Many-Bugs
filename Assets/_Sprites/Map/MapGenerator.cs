using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    [Min(8)] public int width = 128;
    [Min(8)] public int height = 128;
    public bool centerOnOrigin = true;

    [Header("Tiles & Target")]
    public Tilemap tilemap;
    public TileBase groundTile;
    public TileBase waterTile;

    [Header("Noise (Base fBm)")]
    [Min(0.0001f)] public float noiseScale = 0.08f;
    [Range(1, 6)] public int octaves = 3;
    [Range(0.2f, 2.5f)] public float lacunarity = 1.9f;
    [Range(0.3f, 0.9f)] public float persistence = 0.55f;

    [Header("Domain Warp (shore irregularity)")]
    [Tooltip("How much to warp x/y sampling for jaggier coastlines.")]
    [Range(0f, 40f)] public float warpStrength = 12f;
    [Min(0.0001f)] public float warpScale = 0.03f;

    [Header("Island Mask (always island)")]
    [Tooltip("0..1 factor of map half-size used as base island radius. Lower = smaller island (more water).")]
    [Range(0.4f, 1.2f)] public float islandRadiusFactor = 0.9f;
    [Tooltip("How strongly the edge falls to water.")]
    [Range(0f, 1f)] public float falloffStrength = 0.6f;
    [Tooltip("Controls how soft the coastline transitions are.")]
    [Range(0.1f, 0.9f)] public float falloffSoftness = 0.45f;

    [Header("Threshold & Post-process")]
    [Tooltip("Final cutoff. Above = land, below = water.")]
    [Range(0f, 1f)] public float landThreshold = 0.5f;
    [Tooltip("Iterations of cellular smoothing for cleaner coasts.")]
    [Range(0, 8)] public int smoothIterations = 2;
    [Tooltip("If true, removes scattered islands and keeps the biggest one.")]
    public bool keepLargestLandmass = true;

    [Header("Randomization")]
    public int seed = 0;
    public Vector2 noiseOffset = Vector2.zero;

    [Header("Runtime")]
    public bool generateOnAwake = true;

    // working buffers
    bool[,] landMask;
    System.Random rng;

    void OnValidate()
    {
        if (noiseScale < 0.0001f) noiseScale = 0.0001f;
        if (warpScale < 0.0001f) warpScale = 0.0001f;
        if (octaves < 1) octaves = 1;
    }

    void Awake()
    {
        if (generateOnAwake) Generate();
    }

    [ContextMenu("Generate Now")]
    public void Generate()
    {
        if (tilemap == null || groundTile == null || waterTile == null)
        {
            Debug.LogWarning("[MapGenerator] Assign Tilemap + both Tiles.");
            return;
        }

        rng = new System.Random(seed);

        landMask = new bool[width, height];
        BuildIslandMask();
        for (int i = 0; i < smoothIterations; i++)
            SmoothOnce(landMask);

        if (keepLargestLandmass)
            KeepLargestBlob(landMask);

        PaintTiles(landMask);
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (tilemap != null) tilemap.ClearAllTiles();
    }

    // -------------------------
    // Core field generation
    // -------------------------
    void BuildIslandMask()
    {
        // Seed jitter keeps maps deterministic per seed but allows offset nudging.
        Vector2 seedJitter = new Vector2(
            rng.Next(-1_000_000, 1_000_000),
            rng.Next(-1_000_000, 1_000_000)
        );
        Vector2 baseOffset = noiseOffset + seedJitter;

        // precompute island falloff parameters
        float cx = (width - 1) * 0.5f;
        float cy = (height - 1) * 0.5f;
        float maxRadius = Mathf.Min(cx, cy) * islandRadiusFactor;

        for (int y = 0; y < height; y++)
        {
            float ty = centerOnOrigin ? (y - height / 2) : y;
            for (int x = 0; x < width; x++)
            {
                float tx = centerOnOrigin ? (x - width / 2) : x;

                // --- Domain warp: sample offset field to distort the base noise domain
                float wx = Mathf.PerlinNoise(
                    (x + baseOffset.x) * warpScale,
                    (y + baseOffset.y) * warpScale
                );
                float wy = Mathf.PerlinNoise(
                    (x - baseOffset.x) * warpScale * 0.9f,
                    (y + baseOffset.y) * warpScale * 1.1f
                );
                float ox = (wx - 0.5f) * warpStrength;
                float oy = (wy - 0.5f) * warpStrength;

                // --- fBm noise (with warp)
                float n = FBm(
                    (tx + baseOffset.x + ox) * noiseScale,
                    (ty + baseOffset.y + oy) * noiseScale,
                    octaves, lacunarity, persistence
                ); // 0..1

                // --- Radial falloff (ensures island)
                float dx = (x - cx);
                float dy = (y - cy);
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float t = Mathf.InverseLerp(maxRadius * (1f - falloffSoftness), maxRadius, d);
                float fall = Mathf.SmoothStep(0f, 1f, t); // 0 center -> 1 edge

                // combine: push shoreline down near edges
                float value = n - fall * falloffStrength;

                landMask[x, y] = value >= landThreshold;
            }
        }
    }

    float FBm(float u, float v, int oct, float lac, float pers)
    {
        float f = 0f;
        float amp = 1f;
        float freq = 1f;
        float ampSum = 0f;

        for (int i = 0; i < oct; i++)
        {
            f += Mathf.PerlinNoise(u * freq, v * freq) * amp;
            ampSum += amp;
            amp *= pers;
            freq *= lac;
        }
        return f / Mathf.Max(ampSum, 0.0001f);
    }

    // -------------------------
    // Simple coastline smoothing
    // -------------------------
    void SmoothOnce(bool[,] mask)
    {
        var copy = (bool[,])mask.Clone();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int landN = CountLandNeighbors(copy, x, y);
                if (copy[x, y])
                {
                    // kill isolated spikes
                    if (landN <= 2) mask[x, y] = false;
                }
                else
                {
                    // fill small gaps along coast
                    if (landN >= 5) mask[x, y] = true;
                }
            }
        }
    }

    int CountLandNeighbors(bool[,] src, int x, int y)
    {
        int c = 0;
        for (int j = -1; j <= 1; j++)
        {
            int ny = y + j;
            if (ny < 0 || ny >= height) continue;
            for (int i = -1; i <= 1; i++)
            {
                int nx = x + i;
                if (nx < 0 || nx >= width) continue;
                if (i == 0 && j == 0) continue;
                if (src[nx, ny]) c++;
            }
        }
        return c;
    }

    // -------------------------
    // Keep only the largest island
    // -------------------------
    void KeepLargestBlob(bool[,] mask)
    {
        bool[,] visited = new bool[width, height];
        List<Vector2Int> best = null;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!mask[x, y] || visited[x, y]) continue;

                // flood fill
                var blob = new List<Vector2Int>();
                var q = new Queue<Vector2Int>();
                q.Enqueue(new Vector2Int(x, y));
                visited[x, y] = true;

                while (q.Count > 0)
                {
                    var p = q.Dequeue();
                    blob.Add(p);

                    foreach (var n in Neigh4(p.x, p.y))
                    {
                        if (n.x < 0 || n.x >= width || n.y < 0 || n.y >= height) continue;
                        if (visited[n.x, n.y]) continue;
                        if (!mask[n.x, n.y]) continue;
                        visited[n.x, n.y] = true;
                        q.Enqueue(n);
                    }
                }

                if (best == null || blob.Count > best.Count) best = blob;
            }
        }

        // clear everything except the biggest blob
        if (best == null) return;
        var keep = new HashSet<int>();
        foreach (var p in best) keep.Add(p.y * width + p.x);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                mask[x, y] = keep.Contains(y * width + x);
    }

    IEnumerable<Vector2Int> Neigh4(int x, int y)
    {
        yield return new Vector2Int(x + 1, y);
        yield return new Vector2Int(x - 1, y);
        yield return new Vector2Int(x, y + 1);
        yield return new Vector2Int(x, y - 1);
    }

    // -------------------------
    // Painting
    // -------------------------
    void PaintTiles(bool[,] mask)
    {
        tilemap.ClearAllTiles();

        int xStart = centerOnOrigin ? -width / 2 : 0;
        int yStart = centerOnOrigin ? -height / 2 : 0;

        for (int y = 0; y < height; y++)
        {
            int ty = yStart + y;
            for (int x = 0; x < width; x++)
            {
                int tx = xStart + x;
                tilemap.SetTile(new Vector3Int(tx, ty, 0), mask[x, y] ? groundTile : waterTile);
            }
        }

        tilemap.RefreshAllTiles();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var gen = (MapGenerator)target;

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Generate Now"))
                gen.Generate();

            if (GUILayout.Button("Clear"))
                gen.Clear();
        }
    }
}
#endif
