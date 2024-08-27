using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStage : BaseStage
{
    [SerializeField]
    private List<Ladder> _ladderList;

    public override bool CanClimb(Transform playerTransform)
    {
        for (int i = 0; i < _ladderList.Count; i++)
        {
            Ladder ladder = _ladderList[i];

            float difference = Mathf.Abs(ladder.transform.position.x - playerTransform.position.x);

            // X축 1미만
            if (difference < 1f)
            {
                /* TODO : 그냥 점프만 하는것도 포함시켜버려서 플레이어 X 고정은 플레이어에서 처리 (bool을 바꾸자 struct로)
                
                // Y축 계산
                if (playerTransform.position.y > ladder.MinY && playerTransform.position.y < ladder.MaxY)
                {
                    playerTransform.position = new Vector2(ladder.transform.position.x, playerTransform.position.y);
                    return true;
                }
                */
            }
        }

        return false;
    }

    /*
    public override bool CanJumpDown()
    {
        return false;
    }
    */
}
