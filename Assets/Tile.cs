using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Normal,
        Hazard,
        Target,
        Warning,
        Gold
    }

    [Header("Tile State")]
    public TileType tileType = TileType.Normal;

    private Renderer tileRenderer;

    void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        RefreshColor();
    }

    // ==========================================
    // CHANGE TILE TYPE
    // ==========================================

    public void SetTileType(TileType newType)
    {
        tileType = newType;
        RefreshColor();
    }

    // Keep compatibility with your existing code
    public void UpdateColor(TileType newType)
    {
        SetTileType(newType);
    }

    // ==========================================
    // REFRESH CURRENT COLOR
    // Does NOT change tileType
    // ==========================================

    public void UpdateColor()
    {
        RefreshColor();
    }

    private void RefreshColor()
    {
        if (tileRenderer == null)
        {
            tileRenderer = GetComponent<Renderer>();
        }

        if (tileRenderer == null)
            return;

        Material mat = tileRenderer.material;

        // Disable emission by default
        mat.DisableKeyword("_EMISSION");

        if (mat.HasProperty("_EmissionColor"))
        {
            mat.SetColor(
                "_EmissionColor",
                Color.black
            );
        }

        switch (tileType)
        {
            // ======================================
            // NORMAL
            // ======================================

            case TileType.Normal:
                mat.color = Color.blue;
                break;

            // ======================================
            // HAZARD
            // ======================================

            case TileType.Hazard:
                mat.color = Color.red;
                break;

            // ======================================
            // GREEN SAFE TILE
            // ======================================

            case TileType.Target:
                mat.color =
                    new Color(
                        0.1f,
                        1f,
                        0.2f
                    );
                break;

            // ======================================
            // WARNING
            // ======================================

            case TileType.Warning:
                mat.color =
                    new Color(
                        1f,
                        0.55f,
                        0f
                    );
                break;

            // ======================================
            // GOLD GOAL
            // ======================================

            case TileType.Gold:

                Color goldColor =
                    new Color(
                        1f,
                        0.9f,
                        0.05f
                    );

                mat.color = goldColor;

                mat.EnableKeyword(
                    "_EMISSION"
                );

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor(
                        "_EmissionColor",
                        goldColor * 5f
                    );
                }

                break;
        }
    }
}