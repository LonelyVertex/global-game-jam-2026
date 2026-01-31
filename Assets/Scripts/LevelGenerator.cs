using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [Serializable]
    public struct Obstacle
    {
        public float threshold;
        public GameObject prefab;
    }

    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private Obstacle[] obstacles;
    [SerializeField] private float levelWidth;
    [SerializeField] private float levelHeight;
    [SerializeField] private float obstacleDensity;
    [SerializeField] private float noiseScale = 12f;
    [SerializeField] private int octaves = 4;
    [SerializeField] private float lacunarity = 2f;
    [SerializeField] private float persistence = 0.5f;

    private Vector2 _noiseOffset;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        SpawnGround();
        SpawnObstacles();
    }

    private void SpawnGround()
    {
        var ground = Instantiate(groundPrefab, Vector3.zero, Quaternion.identity);

        ground.transform.localScale = new Vector3((levelWidth / 10) + 0.1f, 1f, (levelHeight / 10) + 0.1f);
    }

    private void SpawnObstacles()
    {
        var sorted = obstacles.OrderBy(o => o.threshold).ToList();
        _noiseOffset = new Vector2(Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));

        int cellsX = Mathf.CeilToInt(levelWidth / obstacleDensity);
        int cellsY = Mathf.CeilToInt(levelHeight / obstacleDensity);

        for (int ix = 0; ix < cellsX; ix++)
        for (int iy = 0; iy < cellsY; iy++)
        {
            float i = (ix + 0.5f) * obstacleDensity;
            float j = (iy + 0.5f) * obstacleDensity;

            // perimeter walls
            if (ix == 0 || iy == 0 || ix == cellsX - 1 || iy == cellsY - 1)
            {
                Vector3 pos = new(i - (levelWidth / 2f), 0, j - (levelHeight / 2f));
                Instantiate(wallPrefab, pos, Quaternion.identity);
            }
            else
            {

                float x = (i + _noiseOffset.x) / noiseScale;
                float y = (j + _noiseOffset.y) / noiseScale;

                float n = FractalNoise(x, y, octaves, lacunarity, persistence);

                // shape it a bit (optional but usually helps)
                n = Mathf.Pow(n, 1.6f);

                foreach (var obstacle in sorted)
                {
                    if (n <= obstacle.threshold)
                    {
                        Vector3 pos = new(i - (levelWidth / 2f), 0, j - (levelHeight / 2f));
                        Instantiate(obstacle.prefab, pos, Quaternion.identity);
                        break;
                    }
                }
            }
        }
    }

    private static float FractalNoise(float x, float y, int octaves, float lacunarity, float persistence)
    {
        float amp = 1f, freq = 1f, sum = 0f, norm = 0f;
        for (int o = 0; o < octaves; o++)
        {
            sum += amp * Mathf.PerlinNoise(x * freq, y * freq);
            norm += amp;
            amp *= persistence;
            freq *= lacunarity;
        }
        return sum / norm;
    }
}
