using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float ballSpeed = 10.0f; // 공의 이동 속도
    
    private Vector2 ballDirection; // 공의 이동 방향
    private bool isBallReleased = false; // 공이 패들에서 떨어졌는지를 판단
    
    void Start()
    {
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
        } else if (collision.gameObject.CompareTag("Paddle"))
        {
            // 패들과 충돌할 때 방향 조절
            float hitPoint = collision.contacts[0].point.x;
            float paddleCenter = collision.transform.position.x;
            float angle = (hitPoint - paddleCenter) * 2.0f;
            ballDirection = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)).normalized;
        }
    }
    
    
}
