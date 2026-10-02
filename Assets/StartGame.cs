using UnityEngine;

public class StartGame : MonoBehaviour
{
    private bool started;
    private GUIStyle panelStyle;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;

    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
            canvas.enabled = false;
    }

    public void StartGameButton()
    {
        BeginGame();
    }

    private void BeginGame()
    {
        if (started)
            return;

        started = true;
        Time.timeScale = 1f;

        RedWaveManager waves = FindObjectOfType<RedWaveManager>();
        if (waves != null)
            waves.BeginGame();

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.BeginGame();

        Camera gameCamera = Camera.main;
        if (gameCamera != null)
        {
            gameCamera.transform.position = new Vector3(5f, 6f, 2f);
            gameCamera.transform.rotation = Quaternion.Euler(72.5f, 0f, 0f);
            gameCamera.orthographic = true;
            gameCamera.orthographicSize = 4.3f;
        }
    }

    private void OnGUI()
    {
        if (started)
            return;

        float scale = Mathf.Min(Screen.width / 960f, Screen.height / 540f);
        float panelWidth = Mathf.Min(850f, Screen.width / scale - 48f);
        float panelHeight = Mathf.Min(470f, Screen.height / scale - 32f);
        float x = (Screen.width / scale - panelWidth) * 0.5f;
        float y = (Screen.height / scale - panelHeight) * 0.5f;

        Matrix4x4 oldMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));

        if (panelStyle == null)
        {
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = MakeTexture(new Color(0.025f, 0.04f, 0.075f, 0.98f));
            panelStyle.border = new RectOffset(18, 18, 18, 18);

            textStyle = new GUIStyle(GUI.skin.label);
            textStyle.normal.textColor = Color.white;
            textStyle.fontSize = 20;
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.wordWrap = true;
            textStyle.richText = true;

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 24;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.normal.textColor = new Color(0.1f, 0.08f, 0.02f);
        }

        GUI.Box(new Rect(x, y, panelWidth, panelHeight), GUIContent.none, panelStyle);
        string instructions =
            "Move with the arrow keys.\n\n" +
            "🟦 BLUE = Normal\n" +
            "🟩 GREEN = Safe\n" +
            "🟥 RED = Danger\n\n" +
            "Watch for the yellow warning — it indicates where the red attack will come from.\n\n" +
            "Find a green tile to take a temporary safe spot.\n\n" +
            "SURVIVE 30–60 SECONDS\n" +
            "THEN REACH THE GOLD EXIT.";
        GUI.Label(new Rect(x + 36f, y + 24f, panelWidth - 72f, panelHeight - 110f), instructions, textStyle);

        float buttonWidth = 250f;
        Rect buttonRect = new Rect(x + (panelWidth - buttonWidth) * 0.5f, y + panelHeight - 76f, buttonWidth, 54f);
        if (GUI.Button(buttonRect, "Start Game", buttonStyle))
            BeginGame();

        GUI.matrix = oldMatrix;
    }

    private static Texture2D MakeTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}
