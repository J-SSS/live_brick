using System;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     Debug.Log("Start!");   
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
    /**
     * [SerializeField] : 오브젝트를 매칭시키기 위한 필드
     * 그냥 public으로 선언해도 비슷한 결과
     */
    // 
    [SerializeField] private GameObject bar;
    private Vector3 mousePos;
    
    private void OnMouseDown() // 유니티 기본 제공
    {
        Debug.Log("onDown");
        mousePos = getPosInfo();
    }
    
    private void OnMouseDrag() // 유니티 기본 제공
    {
        Debug.Log("onDrag");
        Vector3 worldPos = getPosInfo();
        Vector3 diffPos = worldPos - mousePos;
        diffPos.z = 0; // x좌표만 사용하므로 초기화
        diffPos.y = 0;
        
        bar.transform.position = new Vector3(Mathf.Clamp(bar.transform.position.x + diffPos.x, -2.5f, 2.5f),
            bar.transform.position.y, bar.transform.position.z);
    }

    private Vector3 getPosInfo()
    {
        Vector3 pos = Input.mousePosition; // 유니티 기본 제공
        pos.z = 10;
        pos = Camera.main.ScreenToWorldPoint(pos);
        return pos;
    }
}
