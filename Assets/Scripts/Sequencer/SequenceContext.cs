using System.Collections.Generic;

public class SequenceContext
{
    private readonly Dictionary<ActorKey, Actor> _actors;

    public SequenceContext(Dictionary<ActorKey, Actor> actors)
    {
        _actors = actors;
    }

    public Actor GetActor(ActorKey key)
    {
        if (!_actors.TryGetValue(key, out var actor) || actor == null)
            throw new System.Exception($"No actor bound for key: {key}");
        return actor;
    }
}