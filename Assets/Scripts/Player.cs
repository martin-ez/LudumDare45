using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Material fowMat;

    string keys = "";
    bool blockedMovement = false;

    enum PlayerState
    {
        Moving,
        DDR
    }

    private PlayerState state = PlayerState.Moving;

    void Update()
    {
        keys = "";

        if (Input.GetKey(KeyCode.W)) keys += 'U';
        if (Input.GetKey(KeyCode.S)) keys += 'D';
        if (Input.GetKey(KeyCode.A)) keys += 'L';
        if (Input.GetKey(KeyCode.D)) keys += 'R';
        Move();
        fowMat.SetVector("_player_pos", transform.position);
    }

    private void Move()
    {
        if (!blockedMovement && keys.Length == 1)
        {
            char direction = keys[0];
            Vector3 endPos = transform.position;
            switch (direction)
            {
                case 'U':
                    endPos += Vector3.forward;
                    break;
                case 'D':
                    endPos += Vector3.back;
                    break;
                case 'L':
                    endPos += Vector3.left;
                    break;
                case 'R':
                    endPos += Vector3.right;
                    break;
            }
            StartCoroutine(Translate(endPos, .2f));
        }
    }

    IEnumerator Translate(Vector3 endPos, float duration)
    {
        blockedMovement = true;
        float t = 0;
        float time = 0;
        Vector3 startPos = transform.position;
        while (t < 1)
        {
            time += Time.deltaTime;
            t = time / duration;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        transform.position = endPos;
        blockedMovement = false;
    }
}
