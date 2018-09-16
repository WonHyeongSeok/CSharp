using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 매니저 클래스
/// </summary>
public class GameManager : MonoBehaviour
{
    public int tileCountOnAwake = 2;
    public Text scoreText = null;
    public PlayButton playButton = null;
    private Grid grid = null;
    private Animator animator = null;
    public int _score = 0;

    private int score
    {
        get { return _score; }
        set { _score = value; scoreText.text = string.Format("Score: {0}", _score); }
    }

    public void Awake()
    {
        animator = GetComponent<Animator>();

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Play 버튼이 눌렸을 때, 게임을 플레이
    /// </summary>
    public void Play()
    {
        gameObject.SetActive(true);

        StartCoroutine(PlayProcess());
    }

    private IEnumerator PlayProcess()
    {
        yield return null;

        grid = GameObject.FindObjectOfType<Grid>();

        for (int i = 0; i < tileCountOnAwake; ++i)
            grid.RandomGenerateTile();
    }

    /// <summary>
    /// Stop 버튼이 눌렸을 때, 게임을 정지
    /// </summary>
    public void Stop()
    {
        StartCoroutine(StopProcess());
    }

    /// <summary>
    /// 게임 Stop 기능을 처리할 코루틴
    /// </summary>
    private IEnumerator StopProcess()
    {
        // StopGame 애니메이션 재생
        animator.SetTrigger("Stop");

        yield return new WaitForSeconds(0.2f);  // 모든 게임판들이 사라지고 난 뒤에

        //초기화
        score = 0;
        grid.ResetAllTiles();
        playButton.Show();

        yield return new WaitForSeconds(0.2f);   // Play 버튼이 생기고 난 뒤에

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 모든 타일을 왼쪽으로 이동
    /// </summary>
    public void MoveLeft()
    {
        if (grid.MoveLeft())
            OnMoved();
    }

    /// <summary>
    /// 오른쪽으로 이동
    /// </summary>
    public void MoveRight()
    {
        if (grid.MoveRight())
            OnMoved();
    }

    /// <summary>
    ///  위쪽으로 이동
    /// </summary>
    public void MoveUp()
    {
        if (grid.MoveUp())
            OnMoved();
    }

    /// <summary>
    /// 아래쪽으로 이동
    /// </summary>
    public void MoveDown()
    {
        if (grid.MoveDown())
            OnMoved();
    }

    /// <summary>
    /// 타일을 상하좌우로 이동하고 난 뒤에 실행되는 메소드
    /// </summary>
    private void OnMoved()
    {
        grid.RandomGenerateTile();

        score += grid.lastCombinedNumber;
    }
}