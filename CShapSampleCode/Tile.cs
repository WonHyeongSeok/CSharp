using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int x = 0;   // 타일 맵 인덱스 x
    public int y = 0;   // 타일 맵 인덱스 y

    // 게임 진행에 필요한 숫자
    // 2, 4, 8, 16 ...
    private int _num;

    public int num
    {
        get { return _num; }
        set { _num = value; text.text = value.ToString(); } // 숫자가 바뀔때마다 UI.Text.text도 자동으로 변경
    }

    private Text text;

    private Animator animator;

    public void Awake()
    {
        text = GetComponentInChildren<Text>();

        animator = GetComponent<Animator>();

        num = 2;
    }

    /// <summary>
    /// 유니티 에디터에서만, 게임오브젝트의 이름을 변경
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public void RefreshName()
    {
        gameObject.name = string.Format("Tile ({0}, {1})", x, y);
    }

    /// <summary>
    /// 합치는 애니메이션 재생
    /// </summary>
    public void PlayCombineAnimation()
    {
        animator.SetTrigger("Combine");
    }

    /// <summary>
    /// 타일을 목표 위치로 이동시키는 프로세스를 실행시키는 메소드
    /// </summary>
    public void MoveTo(Vector3 goalPos)
    {
        StartCoroutine(MoveProcess(transform.localPosition, goalPos, 0.1f));
    }

    /// <summary>
    /// 타일을 목표 위치로 이동시키는 프로세스
    /// </summary>

    private IEnumerator MoveProcess(Vector3 startPos, Vector3 goalPos, float time)
    {
        Vector3 currentPos = Vector3.zero;

        transform.localPosition = startPos;

        for (float t = 0.0f; t <= time; t += Time.deltaTime)
        {
            currentPos = Vector3.Lerp(startPos, goalPos, t / time);

            transform.localPosition = currentPos;

            yield return null;
        }

        transform.localPosition = goalPos;
    }
}