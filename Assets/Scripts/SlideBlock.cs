using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideBlock : MonoBehaviour
{
    private Vector3 startPosition;

    private void Awake() {
        startPosition = transform.position;
    }

    public bool CheckCanMove(Vector3 direction) {
        return  (!Physics.Raycast(transform.position, transform.TransformDirection(direction), 2f, LayerMask.GetMask("Block", "Wall") ));
    }

    public bool IsCorrect() {
        return startPosition.Equals(transform.position);
    }
}
