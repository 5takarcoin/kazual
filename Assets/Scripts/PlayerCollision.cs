using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private float floorCheckRadius;
    [SerializeField] public float bottomOffset;
    [SerializeField] private float wallCheckRadius;
    [SerializeField] private float frontOffset;
    [SerializeField] private float roofCheckRadius;
    [SerializeField] private float upOffset;
    [SerializeField] private float wallUpOffset;

    [SerializeField] private float ledgeGrabForwardPosition;
    [SerializeField] private float ledgeGrabUpwardPosition;
    [SerializeField] private float ledgeGrabDistance;

    [SerializeField] public LayerMask floorLayers;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private LayerMask roofLayers;
    [SerializeField] private LayerMask ledgeGrabLayers;


    public bool CheckFloor(Vector3 direction)
    {
        Vector3 position = transform.position + direction * bottomOffset;
        Collider[] hitColliders = Physics.OverlapSphere(position, floorCheckRadius, floorLayers);
        if (hitColliders.Length > 0) return true;
        return false;
    }

    public bool CheckWall(Vector3 direction)
    {
        Vector3 position = transform.position + transform.up * wallUpOffset + direction * frontOffset;
        Collider[] hitColliders = Physics.OverlapSphere(position, wallCheckRadius, wallLayers);
        if (hitColliders.Length > 0) return true;
        return false;
    }

    public bool CheckRoof(Vector3 direction)
    {
        Vector3 position = transform.position + direction * upOffset;
        Collider[] hitColliders = Physics.OverlapSphere(position, roofCheckRadius, roofLayers);
        if (hitColliders.Length > 0) return true;
        return false;
    }

    public Vector3 CheckLedges()
    {
        Vector3 rayPosition = transform.position + transform.forward * ledgeGrabForwardPosition + transform.up * ledgeGrabUpwardPosition;
        if (Physics.Raycast(rayPosition, -transform.up, out RaycastHit hit, ledgeGrabDistance, ledgeGrabLayers)) return hit.point;
        return Vector3.zero;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position1 = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawSphere(position1, floorCheckRadius);

        Gizmos.color = Color.red;
        Vector3 position2 = transform.position + transform.up * wallUpOffset + (transform.forward * frontOffset);
        Gizmos.DrawSphere(position2, wallCheckRadius);

        Gizmos.color = Color.green;
        Vector3 position3 = transform.position + (transform.up * upOffset);
        Gizmos.DrawSphere(position3, roofCheckRadius);

        Gizmos.color = Color.black;
        Vector3 position4 = transform.position + (transform.forward * ledgeGrabForwardPosition) + (transform.up * ledgeGrabUpwardPosition);
        Gizmos.DrawLine(position4, position4 + (-transform.up * ledgeGrabDistance));
    }
}
