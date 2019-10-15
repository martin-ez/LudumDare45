using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoWManager : MonoBehaviour
{
    public Material fowMat;
    public Material collectableMat;
    public float maxSizeCamera = 15;

    public const string BASE_COLOR = "_base_color";
    public const string EDGE_COLOR = "_edge_color";
    public const string PING_COLOR = "_base_color";

    public const string PLAYER_POS = "_player_pos";
    public const string RADIUS = "_fow_radius";

    public const string PING_ORIGIN = "_ping_origin";
    public const string PING_RADIUS = "_ping_radius";
    public const string PING_THICKNESS = "_ping_thickness";

    public const string NOISE_WIDTH = "_noise_width";

    public static FoWManager instance = null;

    private CameraFollow cameraFollow;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            cameraFollow = FindObjectOfType<CameraFollow>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetFloat(PING_RADIUS, 0);
        SetFloat(PING_THICKNESS, 0);
        SetFloat(RADIUS, 0);
        SetFloat(NOISE_WIDTH, 0);
        SetVector(PLAYER_POS, Vector3.zero);
    }

    public void SetColor(string property, Color color)
    {
        fowMat.SetColor(property, color);
    }

    public void SetFloat(string property, float value)
    {
        fowMat.SetFloat(property, value);
        collectableMat.SetFloat(property, value);
    }

    public void SetVector(string property, Vector3 vector)
    {
        fowMat.SetVector(property, vector);
        collectableMat.SetVector(property, vector);
    }

    private void OnDestroy()
    {
        fowMat.SetVector(PLAYER_POS, Vector3.zero);
        collectableMat.SetVector(PLAYER_POS, Vector3.zero);
        fowMat.SetFloat(RADIUS, 80f);
        collectableMat.SetFloat(RADIUS, 80f);
    }

    public IEnumerator AnimateFocus(bool lose, float value, float duration)
    {
        float from = lose ? 1 : value;
        float to = lose ? value : 1;

        instance.SetFloat(NOISE_WIDTH, from);

        float t = 0;
        float time = 0;
        while (t < 1)
        {
            time += Time.deltaTime;
            t = time / duration;
            instance.SetFloat(NOISE_WIDTH, Mathf.Lerp(from, to, t));
            yield return null;
        }

        instance.SetFloat(NOISE_WIDTH, to);
    }

    public IEnumerator AnimateRadius(float from, float to, float duration, bool ignoreCameraSize = false)
    {
        instance.SetFloat(RADIUS, from);

        float t = 0;
        float time = 0;
        while (t < 1)
        {
            time += Time.deltaTime;
            t = time / duration;
            instance.SetFloat(RADIUS, Mathf.Lerp(from, to, t));
            Camera.main.orthographicSize = Mathf.Min(Mathf.Lerp(from, to, t) + 2f, ignoreCameraSize ? float.PositiveInfinity : maxSizeCamera);
            yield return null;
        }

        instance.SetFloat(RADIUS, to);
    }
}
