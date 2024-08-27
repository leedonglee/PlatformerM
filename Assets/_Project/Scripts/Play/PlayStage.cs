using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStage : BaseStage
{
    [SerializeField]
    private List<StageLadder> _ladderList;

    private Transform _playerTransform;

    void Start()
    {
        _playerTransform = _controller.Player.Transform;
    }

    public override ILadder GetLadder(Transform footTransform, bool moveUp)
    {
        for (int i = 0; i < _ladderList.Count; i++)
        {
            StageLadder ladder = _ladderList[i];

            if (!ladder.CanClimb)
            {
                continue;
            }

            float differenceX = Mathf.Abs(ladder.transform.position.x - _playerTransform.position.x);

            bool canClimb = false;

            // Check X
            if (differenceX < 0.25f)
            {
                float maxY = ladder.MaxY;
                float minY = ladder.MinY;

                // Check Y
                if (moveUp)
                {
                    if (footTransform.position.y > minY && footTransform.position.y < maxY)
                    {
                        canClimb = true;
                    }
                }
                else
                {
                    float differenceY = Mathf.Abs(ladder.transform.position.x - _playerTransform.position.x);

                    if (footTransform.position.y >= maxY && differenceY < 0.25f)
                    {
                        canClimb = true;
                    }
                }
            }

            if (canClimb)
            {
                _playerTransform.position = new Vector2(ladder.transform.position.x, _playerTransform.position.y);
                return ladder;
            }
        }

        return null;
    }

    /*
    public override bool CanClimb(Transform playerTransform)
    {
        for (int i = 0; i < _ladderList.Count; i++)
        {
            Ladder ladder = _ladderList[i];

            float difference = Mathf.Abs(ladder.transform.position.x - playerTransform.position.x);

            // X축 1미만
            if (difference < 1f)
            {
                TODO : 그냥 점프만 하는것도 포함시켜버려서 플레이어 X 고정은 플레이어에서 처리 (bool을 바꾸자 struct로)
                
                // Y축 계산
                if (playerTransform.position.y > ladder.MinY && playerTransform.position.y < ladder.MaxY)
                {
                    playerTransform.position = new Vector2(ladder.transform.position.x, playerTransform.position.y);
                    return true;
                }
            }
        }

        return false;
    }
    */

    /*
    public override bool CanJumpDown()
    {
        return false;
    }
    */
}
