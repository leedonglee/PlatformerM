using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStage : BaseStage
{
    [SerializeField]
    private List<StageLadder> _ladderList;

    /*
    void Start()
    {
    }
    */

    public override ILadder GetLadder(Vector2 playerFoot, bool climbingUp)
    {
        for (int i = 0; i < _ladderList.Count; i++)
        {
            StageLadder ladder = _ladderList[i];

            if (!ladder.CanClimb)
            {
                continue;
            }

            float differenceX = Mathf.Abs(ladder.transform.position.x - playerFoot.x);

            // Check X
            if (differenceX < 0.25f)
            {
                // Check Y
                float maxY = ladder.MaxY;
                float minY = ladder.MinY;

                if (climbingUp)
                {
                    if (playerFoot.y >= minY)
                    {
                        if (playerFoot.y > maxY - 0.05f)
                        {
                            return null;
                        }

                        return ladder;
                    }
                }
                else
                {
                    if (playerFoot.y > minY && playerFoot.y <= maxY + 0.05f)
                    {
                        return ladder;
                    }
                }
            }
        }

        return null;
    }

}
