using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    private EnemyController enemyController;

    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player is hit
        if (other.CompareTag("Player"))
        {
            PlayerData playerData = other.GetComponent<PlayerData>();
            if (enemyController != null)
            {
                enemyController.DealDamage(playerData);
            }
        }
    }
}
