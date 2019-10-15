using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public Material playerMat;

    public float movementSpeed = 4f;

    public Transform model;

    bool blockedMovement = true;
    private Rigidbody rb;
    Vector3 forward, right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        model.gameObject.SetActive(false);
        playerMat.SetVector("_dissolve_origin", transform.position);
        playerMat.SetFloat("_effect", 3);
    }

    public void Appear(float time)
    {
        model.gameObject.SetActive(true);
        StartCoroutine(AnimateDissolve(true, time));
    }

    public void Disappear()
    {
        model.gameObject.SetActive(false);
    }

    public void Toggle()
    {
        blockedMovement = !blockedMovement;
        rb.constraints = blockedMovement ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (Input.anyKey) Move();
        FoWManager.instance.SetVector(FoWManager.PLAYER_POS, transform.position);
        model.transform.localPosition += Vector3.up * Mathf.Sin(Time.time) * 0.005f;
    }

    private void Move()
    {
        if (!blockedMovement)
        {
            Vector3 rightMov = Input.GetAxis("HorizontalKey") * right * Time.deltaTime * movementSpeed;
            Vector3 forwardMov = Input.GetAxis("VerticalKey") * forward * Time.deltaTime * movementSpeed;

            Vector3 forwardVector = Vector3.Normalize(rightMov + forwardMov);
            if (forwardVector != Vector3.zero) transform.forward = forwardVector;
            transform.position += rightMov;
            transform.position += forwardMov;
        }       
    }

    public void Restart()
    {
        StartCoroutine(RecoverAnimation());
    }

    IEnumerator AnimateDissolve(bool appear, float duration)
    {
        float from = appear ? 3 : 0;
        float to = appear ? 0 : 3;

        playerMat.SetVector("_dissolve_origin", transform.position + (Vector3.up * (appear ? 3f : 0)));
        playerMat.SetFloat("_effect", from);

        float t = 0;
        float time = 0;
        while (t < 1)
        {
            time += Time.deltaTime;
            t = time / duration;
            playerMat.SetFloat("_effect", Mathf.Lerp(from, to, t));
            yield return null;
        }

        playerMat.SetFloat("_effect", to);
    } 

    IEnumerator RecoverAnimation()
    {
        Toggle();
        transform.SetParent(null);
        StartCoroutine(FoWManager.instance.AnimateFocus(true, 30f, 1.5f));
        yield return StartCoroutine(AnimateDissolve(false, 0.75f));

        float t = 0;
        float time = 0;
        Vector3 from = transform.position;
        while (t < 1)
        {
            time += Time.deltaTime;
            t = time / 1.5f;
            transform.position = Vector3.Lerp(from, Vector3.zero, t);
            playerMat.SetVector("_dissolve_origin", transform.position + (Vector3.up * 2));
            yield return null;
        }

        transform.position = Vector3.zero;
        StartCoroutine(FoWManager.instance.AnimateFocus(false, 30f, 1.5f));
        yield return StartCoroutine(AnimateDissolve(true, 1.5f));
        Toggle();
    }
}
