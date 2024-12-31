using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WaitForAllTweensComplete : CustomYieldInstruction
{
    private List<Tween> tweens;

    public override bool keepWaiting
    {
        get
        {
            foreach (var tween in tweens)
            {
                if (tween.IsActive() && !tween.IsComplete())
                    return true;
            }
            return false;
        }
    }

    public WaitForAllTweensComplete(List<Tween> tweens)
    {
        this.tweens = tweens;
    }
}

