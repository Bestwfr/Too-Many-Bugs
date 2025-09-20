using UnityEngine;

public class PlayerStateDebugger : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("GUI Settings")]
    [SerializeField] private int fontSize = 24;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Vector2 screenPosition = new Vector2(10, 10);
    [SerializeField] private Vector2 boxSize = new Vector2(220, 40); // background box size

    private GUIStyle _style;

    private void OnGUI()
    {
        if (player != null && player.StateMachine != null && player.StateMachine.CurrentState != null)
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label);
                _style.alignment = TextAnchor.UpperLeft;
                _style.wordWrap = false;
            }

            _style.fontSize = fontSize;
            _style.normal.textColor = textColor;

            string stateName = $"State: {player.StateMachine.CurrentState.StateName}";

            float x = screenPosition.x;
            float y = screenPosition.y;

            // background box
            GUI.Box(new Rect(x, y, boxSize.x, boxSize.y), GUIContent.none);
            // text
            GUI.Label(new Rect(x, y, boxSize.x, boxSize.y), stateName, _style);
        }
    }
}