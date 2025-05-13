using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeAttack : MonoBehaviour
{
    Inventory inventory;
    Weapon meleeWeapon;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackDelay = 0.5f;
    private float lastAttackTime;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        // Check if V key is pressed for melee attack (for testing)
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (GetComponent<UIInteractions>().MenuOpen()) { return; }
            if (Time.time < lastAttackTime + attackDelay) { return; }

            meleeWeapon = inventory.GetWeapon();
            if (meleeWeapon == null)
            {
                Debug.Log("No melee weapon equipped!");
                return;
            }

            Debug.Log("Performing melee attack!");

            // Short-range attack logic
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange))
            {
                GameObject target = hit.collider.gameObject;
                if (target.CompareTag("Enemy"))
                {
                    Debug.Log("Melee hit enemy!");
                    target.GetComponent<EnemyBase>().TakeDamage((int)meleeWeapon.Damage());
                }
                else
                {
                    Debug.Log("Melee hit " + target.name);
                }
            }

            lastAttackTime = Time.time;
        }
    }
}
