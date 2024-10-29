using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float ballSpeed = 10.0f; // 공의 이동 속도

    private Vector2 ballDirection; // 공의 이동 방향
    private bool isBallReleased = false; // 공이 패들에서 떨어졌는지를 판단

    private Rigidbody2D rb;
    CircleCollider2D cc; // 공의 충돌 콜라이더를 담아둘 변수
    GameObject temp; // 삭제할 블럭 임시로 담아둘 변수
    bool isDel = false; // 동시에 두 블럭 삭제 판단할 변수
    int count = 0; // 공이 동시에 몇개의 블럭과 충돌했는지 판단할 변수

    private Vector2 ballPos;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        ballDirection = Vector2.up.normalized; // 초기 공 이동 방향 설정
    }

    void Update()
    {
        if (!isBallReleased)
        {
            Vector3 paddlePosition = GameObject.Find("Paddle").transform.position;
            // 패들 오브젝트를 찾아 위치를 반환. 대소문자 구분 필수!

            Vector2 ballPosition = paddlePosition; // 공의 위치를 패들의 위치로 변경
            ballPosition.y += 0.185f; // 패들과 공 사이의 간격
            transform.position = ballPosition; // 패들 위에 공 고정

            if (Input.GetButtonDown("Fire1"))
            {
                isBallReleased = true;
                // 무작위 방향으로 공 발사
                ballDirection = new Vector2(Random.Range(-1f, 1f), 1).normalized;
            }
        }
        else
        {
            transform.Translate(ballDirection * ballSpeed * Time.deltaTime);
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Tag도 대소문자 구분 필수!
        if (collision.gameObject.CompareTag("Wall"))
        {
            // 벽과 충돌할 때 방향 반전
            ballDirection = Vector2.Reflect(ballDirection, collision.contacts[0].normal).normalized;
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            // 패들과 충돌할 때 방향 조절
            float hitPoint = collision.contacts[0].point.x;
            float paddleCenter = collision.transform.position.x;
            float angle = (hitPoint - paddleCenter) * 2.0f;
            ballDirection = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)).normalized;
        }
    }

    //    
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     // 공이 대각선으로 이동 시 블럭이 동시에 두 개 충돌되는 것을 처리하기 위해
    //     // OnTriggerEnter2D에서 충돌 처리를 합니다.
    //     if (collision.gameObject.CompareTag("Block"))
    //     {
    //         collision.GetComponent<BlockComponent>().TakeDamage();
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 공이 대각선으로 이동 시 블럭이 동시에 두 개 충돌되는 것을 처리하기 위해
        // OnTriggerEnter2D에서 충돌 처리를 합니다.

        if (collision.gameObject.CompareTag("Block"))
        {
            RemoveBrick(collision.gameObject);
        }
    }

    void RemoveBrick(GameObject brick)
    {
        if (count >= 2) // 이미 두 블럭이상 충돌 시
        {
            count = 0; // 충돌 카운트 초기화
            return; // 리턴
        }

        ballPos = transform.position; // 충돌 시의 공 위치를 ballPos 변수에 담아둠
        Vector2 pos = Vector2.zero;

        // 충돌 서클 콜라이더를 충돌 시의 볼 위치와 공 콜라이더의 지름/2 사이즈로 생성하여
        // 충돌된 블럭들을 Collider2D 배열에 담아줌.
        Collider2D[] col = Physics2D.OverlapCircleAll(ballPos, cc.radius / 2, LayerMask.GetMask("Block"));

        count = col.Length; // 충돌된 블럭 개수

        // 두 블럭 이상 충돌 시 블럭 충돌 처리를 위한 블럭을 담아둘 오브젝트 배열
        GameObject[] colObj = new GameObject[col.Length];

        // 두 블럭 충돌 시 공의 위치와 각 블럭의 위치 사이의 거리값을 담아둘 배열
        float[] p = new float[2];
        
        // 서클 콜라이더에 부딪힌 블럭이 세개일 때 대각선의 블럭은 충돌처리에서 제외
        if (col.Length == 3)
        {
            int sour = 0; // 대각선의 블럭의 인덱스

            // 대각선 블럭 체크
            // 위로 이동
            if (ballDirection.y >= 0)
            {
                // 오른쪽 이동
                if (ballDirection.x >= 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        // 충돌된 오브젝트 들 중 대각선에 있는지 체크
                        if (col[i].transform.position.x >= ballPos.x && col[i].transform.position.y >= ballPos.y)
                        {
                            // 대각선에 있는 오브젝트를 찾았다면 그 인덱스를 sour 변수에 담아둠. 이하 동일
                            sour = i;
                        }
                    }
                }
                else // 왼쪽 이동
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (col[i].transform.position.x < ballPos.x && col[i].transform.position.y >= ballPos.y)
                        {
                            sour = i;
                        }
                    }
                }
            }
            // 아래로 이동
            else
            {
                // 오른쪽 이동
                if (ballDirection.x >= 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (col[i].transform.position.x >= ballPos.x && col[i].transform.position.y < ballPos.y)
                        {
                            sour = i;
                        }
                    }
                }
                else // 왼쪽 이동
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (col[i].transform.position.x < ballPos.x && col[i].transform.position.y < ballPos.y)
                        {
                            sour = i;
                        }
                    }
                }
            }

            int cnt = 0;

            for (int i = 0; i < 3; i++)
            {
                if (i != sour) // 제거할 블럭 인덱스와 같지 않을때만 아래 코드 실행
                {
                    // 각 블럭과 공의 위치를 토대로 거리값을 배열에 담아줌
                    p[cnt] = Vector2.Distance(ballPos, col[i].transform.position);
                    // 대각선의 블럭을 제외한 나머지 블럭을 충돌 처리위해 만들어 둔 colObj 배열에 담아줌
                    
                    colObj[cnt] = col[i].gameObject;

                    cnt++;
                }
            }

            // 공과 더 가까운 블럭을 temp 변수에 담아줌
            if (p[0] <= p[1]) temp = colObj[0];
            else temp = colObj[1];
        }
        else if (col.Length == 2)
        {
            // 충돌된 두 블럭을 블럭 배열에 담아줌
            colObj[0] = col[0].gameObject;
            colObj[1] = col[1].gameObject;

            // 공과의 거리값을 배열에 담아줌
            p[0] = Vector2.Distance(ballPos, colObj[0].transform.position);
            p[1] = Vector2.Distance(ballPos, colObj[1].transform.position);

            // 공과 더 가까운 블럭을 temp 변수에 담아줌
            if (p[0] <= p[1]) temp = colObj[0];
            else temp = colObj[1];
        }
        else temp = brick; // 충돌한 블럭을 담아둠

// 공 이동 방향 설정
        BoxCollider2D bc = temp.GetComponent<BoxCollider2D>();

// 충돌된 블럭이 1개 이상일 때
        if (col.Length > 1)
        {
            if (colObj[0].transform.position.y == colObj[1].transform.position.y)
            {
                // 두 블럭이 나란히 같은 축에 있을 때
                if (ballDirection.y >= 0) pos = Vector2.down;
                else pos = Vector2.up;
            }
            else if (colObj[0].transform.position.x == colObj[1].transform.position.x)
            {
                // 두 블럭이 나란히 같은 축에 있을 때
                if (ballDirection.x >= 0) pos = Vector2.left;
                else pos = Vector2.right;
            }
            else
            {
                // 두 블럭이 대각선 방향에 놓여있을 때
                isDel = true; // 두 블럭을 삭제해야 하는 상태로 전환

                // 공이 위로 이동 중
                if (ballDirection.y >= 0)
                {
                    if (ballDirection.x >= 0) pos = (Vector2.left + Vector2.down).normalized; // 왼쪽 아래
                    else pos = (Vector2.right + Vector2.down).normalized; // 오른쪽 아래
                }
                else
                {
                    // 공이 아래로 이동 중
                    if (ballDirection.x >= 0) pos = (Vector2.left + Vector2.up).normalized; // 왼쪽 위
                    else pos = (Vector2.right + Vector2.up).normalized; // 오른쪽 위
                }
            }
        }
        else
        {
            // 충돌된 블럭이 하나 일 때
            if (ballDirection.y >= 0)
            {
                if (temp.transform.position.x - (bc.size.x / 2) > ballPos.x && ballDirection.x >= 0)
                {
                    pos = Vector2.left;
                }
                else if (temp.transform.position.x + (bc.size.x / 2) <= ballPos.x && ballDirection.x < 0)
                {
                    pos = Vector2.right;
                }
                else pos = Vector2.down;
            }
            else
            {
                if (temp.transform.position.x - (bc.size.x / 2) > ballPos.x && ballDirection.x >= 0)
                {
                    pos = Vector2.left;
                }
                else if (temp.transform.position.x + (bc.size.x / 2) <= ballPos.x && ballDirection.x < 0)
                {
                    pos = Vector2.right;
                }
                else pos = Vector2.up;
            }
        }

        // 짧은 시간 중복 충돌을 막아줌
        StartCoroutine(SetCol());

        // 공의 이동 방향 변경
        ballDirection = Vector2.Reflect(ballDirection, pos);

        // 블럭 삭제 처리
        if (!isDel) temp.GetComponent<BlockComponent>().TakeDamage();
        else
        {
            colObj[0].GetComponent<BlockComponent>().TakeDamage();
            colObj[1].GetComponent<BlockComponent>().TakeDamage();
            isDel = false;
        }

        temp = null;

        
    }
    IEnumerator SetCol()
    {
        cc.enabled = false;
        yield return new WaitForSeconds(0.005f);
        cc.enabled = true;
        count = 0;
    }
}
