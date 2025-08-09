using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    public Joystick joystick;

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private Transform firingTransform;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    private Vector3 playerMove;

    [Header("Radius")]
    [SerializeField] private float radius;

    [SerializeField] private GameObject Harmmer;
    [SerializeField] private Transform target;
    private bool isAttack = false;
    float timer;
    [SerializeField] private float attackDuration = 1f; // thời gian duy trì trạng thái attack
    private float attackTimer = 0f;
    public int point;
    private Enemy enemyCurrent;
    private bool isDetech = false;
    private bool isDead = false;

    public GameObject dead1;
    void Start()
    {
        point = 0;
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (isDead) return;
        PlayerMove();
        AttackTrigle();
    }
    private void PlayerMove()
    {
        if (isAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                isAttack = false;
                anim.SetBool("Attack", false);
                Harmmer.SetActive(true);
                target = null;
                attackTimer = 0f;
            }
            else
            {
                anim.SetFloat("Speed", 0);
            }
        }
        else
        {
            playerMove.x = joystick.Horizontal;
            playerMove.z = joystick.Vertical;
            playerMove.y = 0;

            Vector3 movement = playerMove * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
            anim.SetFloat("Speed", playerMove.sqrMagnitude);

            if (playerMove.sqrMagnitude > 0.01f)
            {
                Quaternion toRotation = Quaternion.LookRotation(playerMove, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    public void AttackTrigle()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        Enemy firstEnemyDetected = null;

        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                firstEnemyDetected = hit.GetComponent<Enemy>();
                break; // chỉ lấy enemy đầu tiên
            }
        }

        if (firstEnemyDetected != null)
        {
            if (enemyCurrent != null && enemyCurrent != firstEnemyDetected)
            {
                enemyCurrent.targetEnemy.SetActive(false); // Tắt enemy cũ nếu khác
            }

            enemyCurrent = firstEnemyDetected;
            enemyCurrent.targetEnemy.SetActive(true); // Bật enemy mới

            if (playerMove.sqrMagnitude == 0.0f)
            {
                target = enemyCurrent.transform;
                anim.SetBool("Attack", true);

                Vector3 directionEnemy = target.position - transform.position;
                directionEnemy.y = 0;
                Quaternion toRotation = Quaternion.LookRotation(directionEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
        }
        else
        {
            if (enemyCurrent != null)
            {
                enemyCurrent.targetEnemy.SetActive(false);
                enemyCurrent = null;
            }
        }
    }

    public void SetOffAttack() => anim.SetBool("Attack", false);
    public void Shooting()
    {
        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet2"))
        {
            dead1.SetActive(true);
            UIManager.instance.isDead = true;
            anim.SetBool("Death", true);
            Harmmer.SetActive(false);
            isDead = true;
        }
    }
    public void DestroyPlayer()
    {
        gameObject.SetActive(false);
        Time.timeScale = 0;
    }
}