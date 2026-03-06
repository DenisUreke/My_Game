using System.Collections.Generic;
using UnityEngine;

public class SequenceTrigger : MonoBehaviour
{
    [SerializeField] private SequenceAsset sequence;
    [SerializeField] private List<ActorBinding> bindings = new();

    [SerializeField] private bool playOnce = true;
    private bool _played;

    private SequenceRunner _runner;

    private void Awake()
    {
        _runner = FindFirstObjectByType<SequenceRunner>();
        if (_runner == null)
            Debug.LogError("No SequenceRunner found. Create a Systems GameObject and add SequenceRunner.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playOnce && _played) return;
        if (!other.CompareTag("Player")) return;

        _played = true;

        var dict = new Dictionary<ActorKey, Actor>();
        foreach (var b in bindings)
        {
            if (b.actor == null) continue;
            dict[b.key] = b.actor;
        }

        var ctx = new SequenceContext(dict);
        StartCoroutine(_runner.Run(sequence, ctx));
    }
}