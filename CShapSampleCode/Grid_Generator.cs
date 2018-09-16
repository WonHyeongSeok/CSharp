using UnityEngine;

public partial class Grid : MonoBehaviour
{
    /// <summary>
    /// 타일을 깔 배경이 되는 그리드 전체를 count*count만큼 만드는 메소드
    /// </summary>
    private void GenerateGrid()
    {
        for (int x = 0; x < count; ++x)
        {
            for (int y = 0; y < count; ++y)
            {
                GenerateGridTile(gridTilePrefab, gridTilePanel, x, y, cellSize, string.Format("Grid Tile ({0}, {1})", x, y));
            }
        }
    }

    /// <summary>
    /// 타일을 깔 배경이 되는 그리드를 만드는 메소드
    /// </summary>

    private GameObject GenerateGridTile(GameObject prefab, Transform parent, int x, int y, Vector2 size, string name)
    {
        var tileGO = Instantiate(prefab, parent);
        tileGO.name = name;

        var tileRT = tileGO.GetComponent<RectTransform>();
        tileRT.localPosition = PointToVector3(x, y);
        tileRT.sizeDelta = cellSize;

        return tileGO;
    }

    /// <summary>
    /// 숫자가 매겨질 타일 오브젝트를 생성하는 메소드
    /// </summary>

    private Tile GenerateTile(GameObject prefab, Transform parent, int x, int y, Vector2 size)
    {
        var tileGO = GenerateGridTile(prefab, parent, x, y, size, name);

        var tile = tileGO.GetComponent<Tile>();
        tile.x = x;
        tile.y = y;
        tiles.Add(tile);
        tile.RefreshName();

        return tile;
    }

    /// <summary>
    /// 비어있는 곳에 하나의 타일을 무작위로 생성
    /// </summary>
    public void RandomGenerateTile()
    {
        var limit = 100000; //최대 한계치 설정
        for (int i = 0; i < limit; ++i)
        {
            // x,y 인덱스를 무작위로 설정
            var x = Random.Range(0, count);
            var y = Random.Range(0, count);

            // 이미 그 자리에 타일이 있으면 다른 자리에 생성하려고 시도
            var tile = GetTile(x, y);
            if (tile != null)
                continue;

            GenerateTile(tilePrefab, tilePanel, x, y, cellSize);  // 타일 생성
            break;
        }
    }
}