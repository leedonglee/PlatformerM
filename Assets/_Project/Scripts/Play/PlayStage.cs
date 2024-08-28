using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStage : BaseStage
{
    [SerializeField]
    private List<StageLadder> _ladderList;

    // Player
    private Transform _playerTransform;
    private ILadder _playerLadder;

    void Start()
    {
        _playerTransform = _controller.Player.Transform;
    }

    public override bool CanClimb(Transform footTransform, bool climbingUp)
    {
        if (_playerLadder != null)
        {
            float maxY = _playerLadder.MaxY;
            float minY = _playerLadder.MinY;

            // Check Y
            return CanClimb(footTransform, climbingUp, maxY, minY);
        }
        else
        {
            for (int i = 0; i < _ladderList.Count; i++)
            {
                StageLadder ladder = _ladderList[i];

                if (!ladder.CanClimb)
                {
                    continue;
                }

                float differenceX = Mathf.Abs(ladder.transform.position.x - _playerTransform.position.x);

                // Check X
                if (differenceX < 0.25f)
                {
                    float maxY = ladder.MaxY;
                    float minY = ladder.MinY;

                    // TODO : 사다리의 MaxY -> 사다리가 있는 Ground의 Top값(max.y)

                    // Check Y
                    bool canClimb = CanClimb(footTransform, climbingUp, maxY, minY);

                    if (canClimb)
                    {
                        _playerLadder = ladder;
                        _playerTransform.position = new Vector2(ladder.transform.position.x, _playerTransform.position.y);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override ILadder GetPlayerLadder()
    {
        ILadder ladder = _playerLadder;
        _playerLadder = null;
        return ladder;
    }

    private bool CanClimb(Transform footTransform, bool climbingUp, float maxY, float minY)
    {
        if (climbingUp)
        {
            if (footTransform.position.y >= minY)
            {
                if (footTransform.position.y > maxY - 0.05f)
                {
                    return false;
                }

                return true;
            }
        }
        else
        {
            if (footTransform.position.y > minY && footTransform.position.y <= maxY + 0.05f)
            {
                return true;
            }
        }

        return false;
    }

}
