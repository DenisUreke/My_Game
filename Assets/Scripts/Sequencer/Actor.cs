using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // This attribute ensures that any GameObject with this script will also have a Rigidbody2D component.
public class Actor : MonoBehaviour
{
    public Rigidbody2D Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

}
