using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    public enum EndingType { Deserter, Victory }
    public EndingType endingType;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (endingType == EndingType.Deserter)
            {
                GameManager.Instance.TriggerDeserterEnding();
            }
        }
    }
}
