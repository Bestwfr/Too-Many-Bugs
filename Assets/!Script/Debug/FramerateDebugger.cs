using UnityEngine;
using System.Text;

public class FramerateDebugger : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] int fontSize = 18;
    [SerializeField] Color textColor = Color.white;
    [SerializeField] Vector2 margin = new Vector2(12, 10);
    [SerializeField] Vector2 boxSize = new Vector2(220, 40); // fixed area

    [Header("Sampling")]
    [SerializeField, Range(0.05f, 1f)] float updateInterval = 0.25f;

    [Header("Control")]
    [SerializeField, Range(-1, 1000)] int targetFrameRate = 60;
    [SerializeField, Range(0, 2)] int vSyncCount = 0;

    private GUIStyle _style;
    private readonly StringBuilder _sb = new StringBuilder(128);

    private float _timer;
    private int _frames;
    private float _timeAccum;

    private float _fpsCurrent;
    private float _fpsAvg;

    private int _lastTarget = int.MinValue;
    private int _lastVsync = int.MinValue;
    private int _appliedTarget = int.MinValue;
    private int _appliedVsync = int.MinValue;

    void Awake()
    {
        ApplySettings();
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        _frames++;
        _timeAccum += dt;
        _fpsCurrent = dt > 0f ? 1f / dt : 0f;

        _timer += dt;
        if (_timer >= updateInterval)
        {
            _fpsAvg = _frames / _timeAccum;
            _timer = 0f;
            _frames = 0;
            _timeAccum = 0f;
        }

        if (targetFrameRate != _lastTarget || vSyncCount != _lastVsync)
            ApplySettings();
    }

    void ApplySettings()
    {
        vSyncCount = Mathf.Clamp(vSyncCount, 0, 2);
        QualitySettings.vSyncCount = vSyncCount;
        _appliedVsync = vSyncCount;

        if (vSyncCount > 0)
        {
            Application.targetFrameRate = -1;
            _appliedTarget = -1;
        }
        else
        {
            Application.targetFrameRate = targetFrameRate;
            _appliedTarget = targetFrameRate;
        }

        _lastTarget = targetFrameRate;
        _lastVsync = vSyncCount;
    }

    void OnGUI()
    {
        if (_style == null)
        {
            _style = new GUIStyle(GUI.skin.label);
            _style.alignment = TextAnchor.UpperRight;
            _style.wordWrap = false; // prevents wrapping
        }
        _style.fontSize = fontSize;
        _style.normal.textColor = textColor;

        _sb.Length = 0;
        _sb.Append("FPS ").Append(_fpsCurrent.ToString("0.0"))
          .Append("  avg ").Append(_fpsAvg.ToString("0.0"))
          .AppendLine();

        if (_appliedVsync > 0)
            _sb.Append("Mode VSync x").Append(_appliedVsync);
        else
            _sb.Append("Mode Target ").Append(_appliedTarget);

        string text = _sb.ToString();

        float x = Screen.width - boxSize.x - margin.x;
        float y = margin.y;

        // background
        GUI.Box(new Rect(x, y, boxSize.x, boxSize.y), GUIContent.none);
        // text
        GUI.Label(new Rect(x, y, boxSize.x, boxSize.y), text, _style);
    }

    public void SetTargetFrameRate(int value)
    {
        targetFrameRate = value;
        ApplySettings();
    }

    public void SetVSyncCount(int value)
    {
        vSyncCount = value;
        ApplySettings();
    }
}
