using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class SequenceAction
{
    public virtual bool IsBlocking => true;
    public abstract IEnumerator Run(SequenceContext ctx);
}

[Serializable]
public class MoveToAction : SequenceAction
{
    public ActorKey actor;
    public Vector2 target;
    public float duration = 1f;

    public override IEnumerator Run(SequenceContext ctx)
    {
        Actor a = ctx.GetActor(actor);
        Rigidbody2D rb = a.Rb;

        Vector2 start = rb.position;

        // Edge case: instant move
        if (duration <= 0f)
        {
            rb.MovePosition(target);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);
            Vector2 next = Vector2.Lerp(start, target, alpha);
            rb.MovePosition(next);
            yield return null;
        }

        rb.MovePosition(target);
    }
}

[Serializable]
public class WaitAction : SequenceAction
{
    public float seconds = 1f;

    public override IEnumerator Run(SequenceContext ctx)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }
}

[Serializable]
public class SequenceStep
{
    // Actions in the same step run at the same time
    [SerializeReference] public List<SequenceAction> actions = new();
}

[CreateAssetMenu(menuName = "Cutscene/Sequence (Movement Only)")]
public class SequenceAsset : ScriptableObject
{
    public List<SequenceStep> steps = new();
}