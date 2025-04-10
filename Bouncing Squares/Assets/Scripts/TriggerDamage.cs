using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    public BouncingSquare ignoredSquare = null;
    public int damage = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == null || ignoredSquare == null)
            return;

        if (collision.gameObject == ignoredSquare.gameObject)
            return;

        if (collision.gameObject.CompareTag("Square"))
        {
            BouncingSquare square = collision.gameObject.GetComponent<BouncingSquare>();

            if (square)
                square.Damage(damage, transform.position);
        }
    }
}
