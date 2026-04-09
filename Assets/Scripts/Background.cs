using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Background : MonoBehaviour
{
    [System.Serializable]
    private class RowData
    {
        public Transform root;
        public SpriteRenderer ground;
        public SpriteRenderer leftBackDecoration;
        public SpriteRenderer leftFrontDecoration;
        public SpriteRenderer rightBackDecoration;
        public SpriteRenderer rightFrontDecoration;
    }

    [Header("Scrolling")]
    [SerializeField] private float scrollSpeed = 1.25f;
    [SerializeField] private Camera targetCamera;

    [Header("Sprites")]
    [SerializeField] private Sprite[] grassTiles;
    [SerializeField] private Sprite[] pathTiles;
    [SerializeField] private Sprite[] pathEdgeTiles;
    [SerializeField] private Sprite[] leftPathEdgeTiles;
    [SerializeField] private Sprite[] rightPathEdgeTiles;
    [SerializeField] private Sprite[] largeSideDecorations;
    [SerializeField] private Sprite[] smallSideDecorations;
    [SerializeField] private int tilePaddingPixels = 1;

    [Header("Layout")]
    [SerializeField] private int laneCount = 12;
    [SerializeField] private int pathLaneWidth = 4;
    [SerializeField] private int minimumRowCount = 22;
    [SerializeField] private float widthFillMultiplier = 1f;
    [SerializeField] private float sideDetailChance = 0.35f;
    [SerializeField] private Color backgroundKeyColor = new Color32(77, 103, 144, 255);
    [SerializeField] private float backgroundKeyThreshold = 0.08f;
    [SerializeField] private int groundSortingOrder = -10;
    [SerializeField] private int decorationSortingOrder = -9;
    [SerializeField] private int largeDecorationEveryNthRow = 5;
    [SerializeField] private int smallDecorationEveryNthRow = 2;
    [SerializeField] private float largeDecorationScaleMultiplier = 0.4f;
    [SerializeField] private float smallDecorationScaleMultiplier = 0.45f;

    private Camera runtimeCamera;
    private RowData[] rows;
    private float rowWidthWorld;
    private float rowHeightWorld;
    private float rowStepWorld;
    private float sideWidthWorld;
    private int tilePixelSize;
    private float rowScale = 1f;
    private float lastCameraWidth = -1f;
    private float lastCameraHeight = -1f;
    private Color grassFillColor;
    private Color pathFillColor;

    private void Awake()
    {
        runtimeCamera = targetCamera != null ? targetCamera : Camera.main;

        if ((leftPathEdgeTiles == null || leftPathEdgeTiles.Length == 0) && grassTiles != null && grassTiles.Length > 2)
        {
            leftPathEdgeTiles = new[] { grassTiles[2] };
        }

        if ((rightPathEdgeTiles == null || rightPathEdgeTiles.Length == 0) && grassTiles != null && grassTiles.Length > 0)
        {
            rightPathEdgeTiles = new[] { grassTiles[0] };
        }

        EnsureSharedPathEdgePool();

        if (runtimeCamera == null || IsMissingGroundSprites())
        {
            return;
        }

        RebuildRows();
    }

    private void Update()
    {
        if (runtimeCamera == null)
        {
            return;
        }

        if (CameraSizeChanged())
        {
            RebuildRows();
        }

        if (rows == null || rows.Length == 0)
        {
            return;
        }

        float moveAmount = scrollSpeed * Time.deltaTime;

        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].root == null)
            {
                continue;
            }

            rows[i].root.position += Vector3.down * moveAmount;
        }

        float cameraBottom = GetCameraBottom();
        float highestBottom = GetHighestRowBottom();

        for (int i = 0; i < rows.Length; i++)
        {
            RowData row = rows[i];

            if (row.root == null || GetRowTop(row) >= cameraBottom)
            {
                continue;
            }

            SetRowBottom(row, highestBottom + rowStepWorld);
            RefreshRow(row, i + Time.frameCount);
            highestBottom = row.root.position.y;
        }
    }

    private bool IsMissingGroundSprites()
    {
        return grassTiles == null
            || pathTiles == null
            || pathEdgeTiles == null
            || pathEdgeTiles.Length == 0
            || grassTiles.Length == 0
            || pathTiles.Length == 0;
    }

    private void EnsureSharedPathEdgePool()
    {
        if (pathEdgeTiles != null && pathEdgeTiles.Length > 0)
        {
            return;
        }

        var collected = new System.Collections.Generic.List<Sprite>();
        AddUniqueSprites(collected, leftPathEdgeTiles);
        AddUniqueSprites(collected, rightPathEdgeTiles);

        if (collected.Count == 0 && grassTiles != null)
        {
            AddUniqueSprites(collected, grassTiles);
        }

        pathEdgeTiles = collected.ToArray();
    }

    private void AddUniqueSprites(System.Collections.Generic.List<Sprite> target, Sprite[] source)
    {
        if (source == null)
        {
            return;
        }

        for (int i = 0; i < source.Length; i++)
        {
            Sprite sprite = source[i];

            if (sprite != null && !target.Contains(sprite))
            {
                target.Add(sprite);
            }
        }
    }

    private void RebuildRows()
    {
        ClearExistingRows();
        CalculateMetrics();

        int rowCount = Mathf.Max(
            minimumRowCount,
            Mathf.CeilToInt(GetCameraHeight() / rowStepWorld) + 3
        );

        rows = new RowData[rowCount];

        for (int i = 0; i < rowCount; i++)
        {
            rows[i] = CreateRow(i);
            RefreshRow(rows[i], i);
        }

        PositionRowsAtStart();
        lastCameraWidth = GetCameraWidth();
        lastCameraHeight = GetCameraHeight();
    }

    private void CalculateMetrics()
    {
        tilePixelSize = Mathf.RoundToInt(grassTiles[0].rect.width) - (tilePaddingPixels * 2);
        tilePixelSize = Mathf.Max(tilePixelSize, 1);
        grassFillColor = DetermineGrassFillColor();
        pathFillColor = DeterminePathFillColor();

        float pixelsPerUnit = grassTiles[0].pixelsPerUnit;
        float targetWidth = GetCameraWidth() * widthFillMultiplier;
        float sourceRowWidth = (laneCount * tilePixelSize) / pixelsPerUnit;

        rowScale = targetWidth / sourceRowWidth;
        rowWidthWorld = targetWidth;
        rowHeightWorld = (tilePixelSize / pixelsPerUnit) * rowScale;
        rowStepWorld = rowHeightWorld;
        sideWidthWorld = (rowWidthWorld - ((pathLaneWidth * tilePixelSize / pixelsPerUnit) * rowScale)) * 0.5f;
    }

    private RowData CreateRow(int rowIndex)
    {
        GameObject rowObject = new GameObject($"BackgroundRow_{rowIndex}");
        rowObject.transform.SetParent(transform, false);

        RowData row = new RowData
        {
            root = rowObject.transform,
            ground = CreateRenderer(rowObject.transform, "Ground", groundSortingOrder, rowScale),
            leftBackDecoration = CreateRenderer(rowObject.transform, "LeftBackDecoration", decorationSortingOrder, 1f),
            leftFrontDecoration = CreateRenderer(rowObject.transform, "LeftFrontDecoration", decorationSortingOrder, 1f),
            rightBackDecoration = CreateRenderer(rowObject.transform, "RightBackDecoration", decorationSortingOrder, 1f),
            rightFrontDecoration = CreateRenderer(rowObject.transform, "RightFrontDecoration", decorationSortingOrder, 1f)
        };

        return row;
    }

    private SpriteRenderer CreateRenderer(Transform parent, string objectName, int sortingOrder, float scale)
    {
        GameObject child = new GameObject(objectName);
        child.transform.SetParent(parent, false);
        child.transform.localScale = Vector3.one * scale;

        SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }

    private void RefreshRow(RowData row, int seed)
    {
        ReleaseGroundSprite(row.ground);
        row.ground.sprite = BuildGroundRowSprite(seed);
        row.ground.drawMode = SpriteDrawMode.Simple;
        row.ground.transform.localPosition = new Vector3(0f, 0f, 0f);

        SetBackDecoration(row.leftBackDecoration, seed * 7, true);
        SetFrontDecoration(row.leftFrontDecoration, seed * 11, true);
        SetBackDecoration(row.rightBackDecoration, seed * 13, false);
        SetFrontDecoration(row.rightFrontDecoration, seed * 17, false);
    }

    private Sprite BuildGroundRowSprite(int seed)
    {
        int width = laneCount * tilePixelSize;
        int height = tilePixelSize;
        Texture2D rowTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        rowTexture.filterMode = FilterMode.Point;
        rowTexture.wrapMode = TextureWrapMode.Clamp;

        int pathStart = Mathf.Clamp((laneCount - pathLaneWidth) / 2, 1, laneCount - pathLaneWidth - 1);
        int pathEnd = Mathf.Min(laneCount, pathStart + pathLaneWidth);

        for (int lane = 0; lane < laneCount; lane++)
        {
            bool usePath = lane >= pathStart && lane < pathEnd;
            bool isLeftEdge = lane == pathStart;
            bool isRightEdge = lane == pathEnd - 1;
            bool isPathInterior = usePath && !isLeftEdge && !isRightEdge;
            bool isNearPathGrass = lane == pathStart - 1 || lane == pathEnd;

            Color[] pixels = isPathInterior
                ? BuildPathFillPixels(seed, lane)
                : isLeftEdge
                    ? BuildPathEdgePixels(seed, false)
                    : isRightEdge
                        ? BuildPathEdgePixels(seed, true)
                        : isNearPathGrass
                            ? BuildGrassLanePixels(seed, lane, true)
                            : BuildGrassLanePixels(seed, lane, false);
            rowTexture.SetPixels(lane * tilePixelSize, 0, tilePixelSize, tilePixelSize, pixels);
        }

        rowTexture.Apply();

        return Sprite.Create(
            rowTexture,
            new Rect(0f, 0f, width, height),
            new Vector2(0f, 0f),
            grassTiles[0].pixelsPerUnit,
            0,
            SpriteMeshType.FullRect
        );
    }

    private Color[] BuildGrassLanePixels(int seed, int lane, bool forceDetail)
    {
        Color[] result = new Color[tilePixelSize * tilePixelSize];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = grassFillColor;
        }

        if (!forceDetail && !ShouldPlaceSideDetail(seed, lane))
        {
            return result;
        }

        Sprite overlaySprite = grassTiles[GetPseudoRandomIndex(seed + lane * 13, grassTiles.Length)];
        Color[] overlayPixels = MaybeFlipPixelsHorizontally(
            ExtractTrimmedPixels(overlaySprite),
            GetPseudoRandomIndex(seed * 41 + lane * 7, 2) == 0
        );

        for (int i = 0; i < result.Length; i++)
        {
            Color overlay = overlayPixels[i];
            if (!ShouldUseBasePixel(overlay))
            {
                result[i] = overlay;
            }
        }

        return result;
    }

    private Color[] BuildPathFillPixels(int seed, int lane)
    {
        Sprite sprite = pathTiles[GetPseudoRandomIndex(seed + lane * 13, pathTiles.Length)];
        return MaybeFlipPixelsHorizontally(
            ExtractTrimmedPixels(sprite),
            GetPseudoRandomIndex(seed * 29 + lane * 3, 2) == 0
        );
    }

    private Color[] BuildPathEdgePixels(int seed, bool flip)
    {
        Sprite source = pathEdgeTiles[GetPseudoRandomIndex(seed * 47, pathEdgeTiles.Length)];
        Color[] result = ExtractTrimmedPixels(source);

        bool shouldFlipVertically = ShouldFlipEdgeVariantForSide(source, flip);

        if (shouldFlipVertically)
        {
            result = FlipPixelsVertically(result);
        }

        for (int i = 0; i < result.Length; i++)
        {
            if (ShouldUseBasePixel(result[i]))
            {
                result[i] = pathFillColor;
            }
        }

        return result;
    }

    private bool ShouldFlipEdgeVariantForSide(Sprite sprite, bool rightSide)
    {
        if (sprite == null)
        {
            return rightSide;
        }

        int suffix = GetSpriteNumericSuffix(sprite.name);

        bool leftOnlyVariant = suffix == 5 || suffix == 20;
        bool rightOnlyVariant = suffix == 0 || suffix == 8 || suffix == 9;

        if (rightSide)
        {
            if (leftOnlyVariant)
            {
                return true;
            }

            if (rightOnlyVariant)
            {
                return false;
            }
        }
        else
        {
            if (rightOnlyVariant)
            {
                return true;
            }

            if (leftOnlyVariant)
            {
                return false;
            }
        }

        return rightSide;
    }

    private int GetSpriteNumericSuffix(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            return -1;
        }

        int underscoreIndex = spriteName.LastIndexOf('_');

        if (underscoreIndex < 0 || underscoreIndex >= spriteName.Length - 1)
        {
            return -1;
        }

        return int.TryParse(spriteName[(underscoreIndex + 1)..], out int value) ? value : -1;
    }

    private Color DetermineGrassFillColor()
    {
        Color[] pixels = ExtractTrimmedPixels(grassTiles[0]);
        Color bestColor = new Color32(176, 194, 19, 255);
        int bestCount = -1;

        for (int i = 0; i < pixels.Length; i++)
        {
            Color candidate = pixels[i];

            if (ShouldUseBasePixel(candidate))
            {
                continue;
            }

            int count = 0;

            for (int j = 0; j < pixels.Length; j++)
            {
                if (AreColorsClose(candidate, pixels[j], 0.03f))
                {
                    count++;
                }
            }

            if (count > bestCount)
            {
                bestCount = count;
                bestColor = candidate;
            }
        }

        bestColor.a = 1f;
        return bestColor;
    }

    private Color DeterminePathFillColor()
    {
        Color[] pixels = ExtractTrimmedPixels(pathTiles[0]);
        Color bestColor = new Color32(177, 109, 49, 255);
        int bestCount = -1;

        for (int i = 0; i < pixels.Length; i++)
        {
            Color candidate = pixels[i];

            if (ShouldUseBasePixel(candidate))
            {
                continue;
            }

            int count = 0;

            for (int j = 0; j < pixels.Length; j++)
            {
                if (AreColorsClose(candidate, pixels[j], 0.03f))
                {
                    count++;
                }
            }

            if (count > bestCount)
            {
                bestCount = count;
                bestColor = candidate;
            }
        }

        bestColor.a = 1f;
        return bestColor;
    }

    private bool ShouldPlaceSideDetail(int seed, int lane)
    {
        int value = GetPseudoRandomIndex(seed * 31 + lane * 17, 100);
        return value < Mathf.RoundToInt(sideDetailChance * 100f);
    }

    private bool ShouldUseBasePixel(Color overlay)
    {
        if (overlay.a <= 0.01f)
        {
            return true;
        }

        float redDiff = Mathf.Abs(overlay.r - backgroundKeyColor.r);
        float greenDiff = Mathf.Abs(overlay.g - backgroundKeyColor.g);
        float blueDiff = Mathf.Abs(overlay.b - backgroundKeyColor.b);

        return redDiff <= backgroundKeyThreshold
            && greenDiff <= backgroundKeyThreshold
            && blueDiff <= backgroundKeyThreshold;
    }

    private bool AreColorsClose(Color a, Color b, float threshold)
    {
        return Mathf.Abs(a.r - b.r) <= threshold
            && Mathf.Abs(a.g - b.g) <= threshold
            && Mathf.Abs(a.b - b.b) <= threshold;
    }

    private Color[] ExtractTrimmedPixels(Sprite source)
    {
        Rect rect = source.textureRect;
        int x = Mathf.RoundToInt(rect.x) + tilePaddingPixels;
        int y = Mathf.RoundToInt(rect.y) + tilePaddingPixels;
        int width = tilePixelSize;
        int height = tilePixelSize;

        return source.texture.GetPixels(x, y, width, height);
    }

    private void SetBackDecoration(SpriteRenderer renderer, int seed, bool leftSide)
    {
        if (renderer == null)
        {
            return;
        }

        renderer.sprite = null;

        bool canPlaceSmall = smallSideDecorations != null
            && smallSideDecorations.Length > 0
            && smallDecorationEveryNthRow > 0
            && (seed % smallDecorationEveryNthRow == 0 || seed % (smallDecorationEveryNthRow + 1) == 0);

        if (!canPlaceSmall)
        {
            return;
        }

        Sprite[] sourceSet = smallSideDecorations;
        float worldScale = rowScale * smallDecorationScaleMultiplier;
        Sprite decorationSprite = sourceSet[GetPseudoRandomIndex(seed * 5, sourceSet.Length)];
        renderer.sprite = decorationSprite;
        renderer.flipX = !leftSide;
        renderer.transform.localScale = Vector3.one * worldScale;

        float decorationWidth = decorationSprite.bounds.size.x * worldScale;
        float bandStart = leftSide ? 0.25f : rowWidthWorld - sideWidthWorld + 0.1f;
        float bandWidth = Mathf.Max(0.2f, sideWidthWorld - decorationWidth - 0.35f);
        float x = bandStart + (GetPseudoRandomIndex(seed * 23, 100) / 100f) * bandWidth;
        float y = GetPseudoRandomIndex(seed * 31, 100) / 100f * (rowHeightWorld * 0.6f);
        renderer.transform.localPosition = new Vector3(x, y, 0f);
        renderer.sortingOrder = decorationSortingOrder + Mathf.RoundToInt((rowHeightWorld - y) * 10f);
    }

    private void SetFrontDecoration(SpriteRenderer renderer, int seed, bool leftSide)
    {
        if (renderer == null)
        {
            return;
        }

        renderer.sprite = null;

        bool canPlaceLarge = largeSideDecorations != null
            && largeSideDecorations.Length > 0
            && largeDecorationEveryNthRow > 0
            && (seed % largeDecorationEveryNthRow == 0 || seed % (largeDecorationEveryNthRow + 1) == 0);

        if (!canPlaceLarge)
        {
            return;
        }

        float worldScale = rowScale * largeDecorationScaleMultiplier;
        Sprite decorationSprite = largeSideDecorations[GetPseudoRandomIndex(seed * 5, largeSideDecorations.Length)];
        renderer.sprite = decorationSprite;
        renderer.flipX = !leftSide;
        renderer.transform.localScale = Vector3.one * worldScale;

        float decorationWidth = decorationSprite.bounds.size.x * worldScale;
        float bandStart = leftSide ? 0.05f : rowWidthWorld - sideWidthWorld + 0.05f;
        float bandWidth = Mathf.Max(0.15f, sideWidthWorld - decorationWidth - 0.1f);
        float x = bandStart + (GetPseudoRandomIndex(seed * 37, 100) / 100f) * bandWidth;
        float y = GetPseudoRandomIndex(seed * 41, 100) / 100f * (rowHeightWorld * 0.22f);
        renderer.transform.localPosition = new Vector3(x, y, 0f);
        bool isTallTreeLike = decorationSprite.rect.height >= 60f;
        int treeBoost = isTallTreeLike ? 100 : 5;
        renderer.sortingOrder = decorationSortingOrder + treeBoost + Mathf.RoundToInt((rowHeightWorld - y) * 10f);
    }

    private Color[] MaybeFlipPixelsHorizontally(Color[] pixels, bool flip)
    {
        if (!flip)
        {
            return pixels;
        }

        Color[] result = new Color[pixels.Length];

        for (int y = 0; y < tilePixelSize; y++)
        {
            for (int x = 0; x < tilePixelSize; x++)
            {
                result[y * tilePixelSize + x] = pixels[y * tilePixelSize + (tilePixelSize - 1 - x)];
            }
        }

        return result;
    }

    private Color[] FlipPixelsVertically(Color[] pixels)
    {
        Color[] result = new Color[pixels.Length];

        for (int y = 0; y < tilePixelSize; y++)
        {
            for (int x = 0; x < tilePixelSize; x++)
            {
                result[y * tilePixelSize + x] = pixels[(tilePixelSize - 1 - y) * tilePixelSize + x];
            }
        }

        return result;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        AutoPopulateDecorations();
        EnsureSharedPathEdgePool();
    }

    private void AutoPopulateDecorations()
    {
        const string decorationsPath =
            "Assets/TopDownFantasy_Forest_v1/TopDownFantasy-Forest/Decorations/Decorations.png";

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(decorationsPath);

        if (assets == null || assets.Length == 0)
        {
            return;
        }

        var large = new System.Collections.Generic.List<Sprite>();
        var small = new System.Collections.Generic.List<Sprite>();

        for (int i = 0; i < assets.Length; i++)
        {
            if (assets[i] is not Sprite sprite)
            {
                continue;
            }

            float maxDimension = Mathf.Max(sprite.rect.width, sprite.rect.height);

            if (maxDimension >= 40f)
            {
                large.Add(sprite);
            }
            else if (maxDimension >= 10f)
            {
                small.Add(sprite);
            }
        }

        if (large.Count > 0)
        {
            largeSideDecorations = large.ToArray();
        }

        if (small.Count > 0)
        {
            smallSideDecorations = small.ToArray();
        }
    }
#endif

    private void PositionRowsAtStart()
    {
        float leftEdge = runtimeCamera.transform.position.x - (rowWidthWorld * 0.5f);
        float startingBottom = GetCameraBottom() - rowHeightWorld;

        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].root.position = new Vector3(leftEdge, startingBottom + (rowStepWorld * i), 0f);
        }
    }

    private void ClearExistingRows()
    {
        if (rows != null)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i] != null && rows[i].ground != null)
                {
                    ReleaseGroundSprite(rows[i].ground);
                }
            }
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        rows = null;
    }

    private void ReleaseGroundSprite(SpriteRenderer renderer)
    {
        if (renderer == null || renderer.sprite == null)
        {
            return;
        }

        Texture2D texture = renderer.sprite.texture;
        Destroy(renderer.sprite);

        if (texture != null)
        {
            Destroy(texture);
        }
    }

    private bool CameraSizeChanged()
    {
        return !Mathf.Approximately(lastCameraWidth, GetCameraWidth())
            || !Mathf.Approximately(lastCameraHeight, GetCameraHeight());
    }

    private int GetPseudoRandomIndex(int seed, int length)
    {
        if (length <= 1)
        {
            return 0;
        }

        int value = seed * 1103515245 + 12345;
        value &= int.MaxValue;
        return value % length;
    }

    private float GetHighestRowBottom()
    {
        float highestBottom = float.MinValue;

        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].root == null)
            {
                continue;
            }

            highestBottom = Mathf.Max(highestBottom, rows[i].root.position.y);
        }

        return highestBottom;
    }

    private float GetRowTop(RowData row)
    {
        return row.root.position.y + rowHeightWorld;
    }

    private void SetRowBottom(RowData row, float bottomY)
    {
        Vector3 position = row.root.position;
        position.y = bottomY;
        row.root.position = position;
    }

    private float GetCameraHeight()
    {
        return runtimeCamera != null && runtimeCamera.orthographic
            ? runtimeCamera.orthographicSize * 2f
            : 10f;
    }

    private float GetCameraWidth()
    {
        return runtimeCamera != null && runtimeCamera.orthographic
            ? GetCameraHeight() * runtimeCamera.aspect
            : 10f;
    }

    private float GetCameraBottom()
    {
        return runtimeCamera != null && runtimeCamera.orthographic
            ? runtimeCamera.transform.position.y - runtimeCamera.orthographicSize
            : -5f;
    }
}
