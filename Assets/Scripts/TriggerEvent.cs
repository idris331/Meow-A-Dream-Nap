using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TriggerEvent : MonoBehaviour
{
    [Space(10)]
    [SerializeField] private UnityEvent _event;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            _event?.Invoke();
    }
}
