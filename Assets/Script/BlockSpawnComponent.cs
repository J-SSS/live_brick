using UnityEngine;

public class BlockSpawnComponent : MonoBehaviour
{
    [SerializeField] GameObject block;            // 생성할 벽돌
    [SerializeField] Vector2 pos;                 // 벽돌 생성 시작 위치
    [SerializeField] Vector2 offset;              // 벽돌 간의 간격

    [SerializeField] int row;                     // 행
    [SerializeField] int col;                     // 열

    // Start is called before the first frame update
    void Start()
    {
        CreateBlocks();
    }
    
    void CreateBlocks()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Instantiate(block, new Vector2(pos.x + (j * offset.x), pos.y + (i * offset.y)), Quaternion.identity);
            }
        }
    }
}
