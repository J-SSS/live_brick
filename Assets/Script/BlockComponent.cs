using System.Collections;
using UnityEngine;

public class BlockComponent : MonoBehaviour
{
    [SerializeField] int hp = 1;                   // 벽돌 내구도

    public void TakeDamage()                       // 공과 충돌시 호출 될 메소드
    {
        Debug.Log("데미지");
        hp -= 1;                                   // 내구도 1 감소
        if (hp == 0) StartCoroutine(Death());      // 내구도가 0이면 오브젝트 삭제
    }

    IEnumerator Death()
    {
        Debug.Log("삭제");
        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject); // 내구도가 0이 되면 Death() 코루틴을 호출하여 약간의 지연 후에 오브젝트를 삭제
    }
}
