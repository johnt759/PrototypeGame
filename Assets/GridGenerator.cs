using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 9;
    public float tileSpacing = 1f;

    [Header("Tile Template")]
    public GameObject tileTemplate;

    void Start()
    {
        ExpandGrid();
    }

    void ExpandGrid()
    {
        if (tileTemplate == null)
        {
            Debug.LogError(
                "GridGenerator: Tile Template is missing!"
            );
            return;
        }

        Tile[] existingTiles =
            FindObjectsOfType<Tile>();

        int createdCount = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                bool tileExists = false;

                foreach (Tile tile in existingTiles)
                {
                    int tileX =
                        Mathf.RoundToInt(
                            tile.transform.position.x
                        );

                    int tileZ =
                        Mathf.RoundToInt(
                            tile.transform.position.z
                        );

                    if (tileX == x &&
                        tileZ == z)
                    {
                        tileExists = true;
                        break;
                    }
                }

                if (tileExists)
                    continue;

                Vector3 newPosition =
                    new Vector3(
                        x * tileSpacing,
                        tileTemplate.transform.position.y,
                        z * tileSpacing
                    );

                GameObject newTile =
                    Instantiate(
                        tileTemplate,
                        newPosition,
                        tileTemplate.transform.rotation
                    );

                newTile.name =
                    "Tile_" + x + "_" + z;

                Tile tileComponent =
                    newTile.GetComponent<Tile>();

                if (tileComponent != null)
                {
                    tileComponent.tileType =
                        Tile.TileType.Normal;
                }

                createdCount++;
            }
        }

        Debug.Log(
            "GRID EXPANDED TO " +
            gridSize + "x" + gridSize +
            ". Created " +
            createdCount +
            " new tiles."
        );
    }
}