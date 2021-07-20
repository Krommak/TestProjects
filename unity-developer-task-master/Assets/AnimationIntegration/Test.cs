using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private GameObject finishingText;
    [SerializeField] private Transform targetBone;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerModel;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform enemy;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject weapon, sword;
    [SerializeField] private float speed = 2;
    [SerializeField] private float rotationSpeed = 2;
    private Vector3 rot;
    Vector3 cameraOffset;
    private CharacterController charControler;

    private bool finishingIsAllowed = false;

    private bool moveToEnemy = false;

    private void Awake()
    {
        cameraOffset = cameraTransform.position;
        charControler = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        Plane playerPlane = new Plane(Vector3.up, player.transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            float targetX;
            Vector3 targetPoint = ray.GetPoint(hitdist);

            targetX = Quaternion.LookRotation(targetPoint - player.transform.position).eulerAngles.y;

            targetX -= player.transform.rotation.eulerAngles.y;

            rot = new Vector3(-targetX, 0, 0);
        }
        MoveCharacter();
        float dist = Vector3.Distance(enemy.position, transform.position);
        if (dist < 5 && enemyAnimator.enabled == true)
        {
            finishingIsAllowed = true;
            SetActiveText(true);
        }
        if (dist > 5)
        {
            finishingIsAllowed = false;
            SetActiveText(false);
        }
        if (dist > 1 && moveToEnemy)
        {
            MoveToEnemy(enemy);
        }
        if (dist <= 1 && moveToEnemy)
        {
            moveToEnemy = false;
            playerAnimator.SetBool("Move", false);
            StartCoroutine(Finishing());
        }
    }

    public void LateUpdate()
    {
        targetBone.localEulerAngles = rot;
        cameraTransform.position = playerModel.transform.position + cameraOffset;
    }

    public void MoveCharacter()
    {
        Vector3 moveDirection;
        if (finishingIsAllowed && Input.GetKeyDown(KeyCode.Space))
        {
            RotateToPoint(enemy);
            moveToEnemy = true;
            playerAnimator.SetBool("Move", true);
        }
        if (Input.GetKey(KeyCode.W) && !moveToEnemy)
        {
            playerAnimator.SetBool("Move", true);
            moveDirection = new Vector3(0, 0, speed);
            player.transform.Translate(moveDirection);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            playerAnimator.SetBool("Move", false);
        }
        if (Input.GetKey(KeyCode.D) && !moveToEnemy)
        {
            playerAnimator.SetBool("Move", true);
            player.transform.Rotate(new Vector3(0, rotationSpeed, 0));
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            playerAnimator.SetBool("Move", false);
        }
        if (Input.GetKey(KeyCode.A) && !moveToEnemy)
        {
            playerAnimator.SetBool("Move", true);
            player.transform.Rotate(new Vector3(0, -rotationSpeed, 0));
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            playerAnimator.SetBool("Move", false);
        }
        if (Input.GetKey(KeyCode.S) && !moveToEnemy)
        {
            playerAnimator.SetBool("MovingBack", true);
            moveDirection = new Vector3(0, 0, -speed*0.75f);
            player.transform.Translate(moveDirection);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            playerAnimator.SetBool("MovingBack", false);
        }
    }

    public void RotateToPoint(Transform targetPoint)
    {
        float targetRotationY = Quaternion.LookRotation(targetPoint.position - player.transform.position).eulerAngles.y;
        targetRotationY -= player.transform.rotation.eulerAngles.y;
        player.transform.Rotate(new Vector3(0, targetRotationY, 0));
    }
    private void MoveToEnemy(Transform targetPoint)
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, targetPoint.position, speed / 2);
    }

    private IEnumerator Finishing()
    {
        playerAnimator.SetTrigger("Finishing");
        TakeSword(true);
        yield return new WaitForSecondsRealtime(0.5f);
        Death();
        SetActiveText(false);
        yield return new WaitForSecondsRealtime(1 + 2 / 3);
        TakeSword(false);
    }

    private void TakeSword(bool takeSword)
    {
        weapon.SetActive(!takeSword);
        sword.SetActive(takeSword);
    }

    public void SetActiveText(bool isActive)
    {
        finishingText.SetActive(isActive);
    }

    public void Death()
    {
        enemyAnimator.enabled = false;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(5.0f);
        enemyAnimator.enabled = true;

        Vector3 newPos = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
        enemy.position = newPos;
    }
}