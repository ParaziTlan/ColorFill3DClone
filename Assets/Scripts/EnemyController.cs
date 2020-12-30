using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameObject fragmentPrefab;   // Enemy sayısı yeterince az! . Lightweight pattern entegre edip kullanmama gerek olmadığını düşündüm.

    [Header(" Fill Patrol Where Vector!!!")]
    [Header(" Goes to a point, then comes back")]
    [Header(" Goes to a point, then comes back")]
    [Header("")]
    public Vector2 PatrolWhere;
    [Header(" Fill Speed!!!")]
    public float Speed = 2f;
    [Header("")]
    public List<Vector2> FragmentCoords = new List<Vector2>();

    private Rigidbody rb;

    public void LoadFragments(bool loadOnlyLatest = false)
    {
        if (loadOnlyLatest) //sadece editörde son oluşturulan 
        {
            Vector2 lastCoord = FragmentCoords[FragmentCoords.Count - 1];
            Instantiate(fragmentPrefab, new Vector3(lastCoord.x, 0.5f, lastCoord.y), Quaternion.identity, transform);
            return;
        }
        foreach (Vector2 coord in FragmentCoords)
        {
            Instantiate(fragmentPrefab, new Vector3(coord.x, 0.5f, coord.y), Quaternion.identity, transform);
        }
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    private void OnEnable()
    {
        LevelManager.LevelStarted += LevelStarted;
    }
    private void OnDisable()
    {
        LevelManager.LevelStarted -= LevelStarted;
    }
    private void LevelStarted()
    {
        if (PatrolWhere.sqrMagnitude > 0)
            MoveNextPatrolPoint();
    }

    private void MoveNextPatrolPoint()
    {
        Vector2 where = new Vector2(transform.position.x + PatrolWhere.x, transform.position.z + PatrolWhere.y);
        rb.velocity = Speed * new Vector3(Mathf.Clamp(PatrolWhere.x, -1f, 1f), 0f, Mathf.Clamp(PatrolWhere.y, -1f, 1f));
        PatrolWhere = -PatrolWhere;
        StartCoroutine(CheckAtPoint(where));
    }
    IEnumerator CheckAtPoint(Vector2 where)
    {
        while (Mathf.Abs(transform.position.x - where.x) > 0.05f | Mathf.Abs(transform.position.z - where.y) > 0.05f)
            yield return new WaitForEndOfFrame();

        MoveNextPatrolPoint();
    }
}
