using UnityEngine;

public class SimplePerlinTerrain : MonoBehaviour
{
    [Header("Terrain Settings")]
    [SerializeField] public int width = 30;
    [SerializeField] public int depth = 30;
    [SerializeField] public float scale = 0.1f;
    [SerializeField] public float heightMultiplier = 8f;

    [Header("Block Prefabs")]
    public GameObject dirtPrefab;   // 흙 블록 프리팹
    public GameObject grassPrefab;  // 잔디 블록 프리팹
    public GameObject waterPrefab;  // 물 블록 프리팹

    [Header("Water Settings")]
    [SerializeField] public int waterLevel = 3; // 이 높이 이하의 빈 공간에 물을 채웁니다.

    SimplePerlinNoise simpleNoise;
    private int[,] heightMap; // 각 좌표의 지형 높이를 기억할 배열

    void Start()
    {
        // 동일한 게임 오브젝트에 부착된 SimplePerlinNoise 컴포넌트를 가져옵니다.
        simpleNoise = GetComponent<SimplePerlinNoise>();

        // 배열 크기 초기화
        heightMap = new int[width, depth];

        Generate();
    }

    public void Generate()
    {
        // 1단계: X축과 Z축으로 루프를 돌며 펄린 노이즈 기반 지형(흙, 잔디) 생성
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // 입력 좌표에 scale을 곱해 노이즈의 조밀함(주파수)을 조절
                float xCoord = x * scale;
                float zCoord = z * scale;

                // SimplePerlinNoise 스크립트에서 노이즈 값 추출
                float noise = simpleNoise.Noise(xCoord, zCoord);

                // 최종 큐브가 쌓일 높이를 결정
                int height = Mathf.RoundToInt(noise * heightMultiplier);

                // 물을 채울 때 참고할 수 있도록 해당 좌표의 지형 높이를 기록
                heightMap[x, z] = height;

                // 결정된 높이만큼 큐브 기둥을 생성 (조건 판별 포함)
                CreateCube(x, z, height);
            }
        }

        // 2단계: 지형 배치가 끝난 후, 특정 높이 이하의 빈 곳을 검색해서 물 채우기
        FillWater();
    }

    void CreateCube(int x, int z, int height)
    {
        // 0층부터 결정된 height 층까지 Y축으로 큐브를 쌓아 올림
        for (int y = 0; y <= height; y++)
        {
            Vector3 position = new Vector3(x, y, z);
            GameObject prefabToSpawn;

            // [조건 적용] 최상단일 경우(y == height) Grass, 그 외 아래는 Dirt 배치
            if (y == height)
            {
                prefabToSpawn = grassPrefab;
            }
            else
            {
                prefabToSpawn = dirtPrefab;
            }

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, position, Quaternion.identity, transform);
            }
        }
    }
    void FillWater()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // 해당 좌표의 지형 최고 높이를 가져옵니다.
                int currentTerrainHeight = heightMap[x, z];

                // 만약 지형 높이가 설정된 물 높이(waterLevel)보다 낮다면 빈 공간에 물을 채웁니다.
                if (currentTerrainHeight < waterLevel)
                {
                    // 지형 바로 위층(currentTerrainHeight + 1)부터 waterLevel까지 물 블록 배치
                    for (int y = currentTerrainHeight + 1; y <= waterLevel; y++)
                    {
                        Vector3 position = new Vector3(x, y, z);

                        if (waterPrefab != null)
                        {
                            Instantiate(waterPrefab, position, Quaternion.identity, transform);
                        }
                    }
                }
            }
        }
    }
}