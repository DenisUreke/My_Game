using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceRunner : MonoBehaviour
{
    public IEnumerator Run(SequenceAsset sequence, SequenceContext ctx)
    {
        foreach (var step in sequence.steps)
        {
            int blockingRemaining = 0;

            foreach (var action in step.actions)
            {
                if (action == null) continue;

                if (action.IsBlocking)
                    blockingRemaining++;

                StartCoroutine(RunOne(action, ctx, () => blockingRemaining--));
            }

            while (blockingRemaining > 0)
                yield return null;
        }
    }

    private IEnumerator RunOne(SequenceAction action, SequenceContext ctx, System.Action onBlockingDone)
    {
        yield return action.Run(ctx);
        if (action.IsBlocking)
            onBlockingDone?.Invoke();
    }
}