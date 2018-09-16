using System.Collections.Generic;
using UnityEngine;

public partial class Grid : MonoBehaviour
{
    public GameObject gridTilePrefab = null;    // 배경으로 사용할 Grid Prefab
    public GameObject tilePrefab = null;    // 숫자를 표시할 Tile Prefab
    public Transform gridTilePanel = null;  // Grid 오브젝트들을 묶어놓을 부모 패널의 Transform
    public Transform tilePanel = null;  // Tile 오브젝트들을 묶어놓을 부모 패널의 Transform
    public int count = 4;   // 그리드 및 타일을 가로x세로 개수만큼 생성/사용할 개수 (가로와 세로 개수가 항상 같음)
    public int lastCombinedNumber = 0;  // 마지막으로 이동 후 합쳐진 숫자를 기록해둔 변수

    private RectTransform gridRT = null;    // 이 GameObject에 추가되어있는 RectTransform
    private static Vector2 cellSize = Vector2.zero; // 한 칸의 타일에 해당하는 크기
    private static Vector2 halfCellSize = Vector2.zero; // 한 칸의 타일의 절반에 해당하는 크기
    private List<Tile> tiles = new List<Tile>();    // 생성되어있는 타일들의 목록 (배경 그리드 제외)

    public void Awake()
    {
        gridRT = GetComponent<RectTransform>();
        cellSize = new Vector2(gridRT.rect.width / count, gridRT.rect.height / count);  // 한 칸의 타일에 해당하는 크기를 계산
        halfCellSize = cellSize * 0.5f;

        // 타일을 깔 배경이 되는 그리드 전체를 count*count만큼 만듬
        GenerateGrid();
    }

    /// <summary>
    /// 타입 맵 인덱스 x,y에 해당하는 타일을 반환
    /// </summary>
    /// <returns>찾은 타일을 반환, 해당 인덱스에 타일이 없으면 null 반환</returns>
    public Tile GetTile(int x, int y)
    {
        return tiles.Find((t) => t.x == x && t.y == y);
    }

    /// <summary>
    /// 타일 맵 인덱스 y에 해당하는 라인을 전부 리스트로 묶어서 돌려주는 메소드
    /// </summary>

    public List<Tile> GetColumns(int y)
    {
        return tiles.FindAll((t) => t.y == y);
    }

    /// <summary>
    /// 타일 맵 인덱스 x에 해당하는 라인을 전부 리스트로 묶어서 돌려주는 메소드
    /// </summary>

    public List<Tile> GetRows(int x)
    {
        return tiles.FindAll((t) => t.x == x);
    }

    /// <summary>
    /// 현재 생성되어있는 모든 타일을 삭제
    /// </summary>
    public void ResetAllTiles()
    {
        foreach (var t in tiles)
            Destroy(t.gameObject);

        tiles.Clear();
    }

    /// <summary>
    /// 타일을 지우는 메소드
    /// 컬렉션에서도 제거되고, 실제 GameObject도 삭제됨
    /// </summary>
    public void RemoveTile(Tile tile, float destroyTime = 0.1f)
    {
        tile.transform.SetAsFirstSibling();

        tiles.Remove(tile);

        Destroy(tile.gameObject, destroyTime);
    }

    /// <summary>
    /// 두 타일을 합치는 메소드
    /// </summary>

    public void CombineTile(Tile surviveTile, Tile deadTile)
    {
        surviveTile.num *= 2; // 합쳐졌으므로 숫자 * 2

        surviveTile.PlayCombineAnimation();  // 합쳐지는 애니메이션 재생

        // 살아있는 타일이 있는 위치로, 합쳐질 타일을 이동
        // 뒤로 이동해서 가리지 않도록
        MoveTile(deadTile, surviveTile.x, surviveTile.y);

        // 합쳐지고 남은 타일을 삭제
        RemoveTile(deadTile);
    }

    /// <summary>
    /// 타일 맵 인덱스 x,y를 게임 좌표계로 변경
    /// </summary>
    /// <returns>변경된 좌표값</returns>
    private static Vector3 PointToVector3(int x, int y)
    {
        return new Vector3(+(x * cellSize.x + halfCellSize.x), -(y * cellSize.y + halfCellSize.y));
    }
}