using UnityEngine;

namespace CardapioRA
{
    public sealed class DishPresenter
    {
        private GameObject activeModel;

        public void Hide()
        {
            if (activeModel == null)
                return;

            Object.Destroy(activeModel);
            activeModel = null;
        }

        public void Show(MenuItem item, Transform anchor)
        {
            Hide();

            var prefab = string.IsNullOrWhiteSpace(item.modelResource)
                ? null
                : Resources.Load<GameObject>(item.modelResource);

            activeModel = prefab != null
                ? Object.Instantiate(prefab, anchor)
                : CreatePlaceholder(item, anchor);

            activeModel.name = $"Dish_{item.id}";
            activeModel.transform.localPosition = new Vector3(0f, 0.005f, 0f);
            activeModel.transform.localRotation = Quaternion.identity;
            activeModel.transform.localScale = Vector3.one;

            // Marcadores: o prato nasce so com a etiqueta (escala do anchor = 1 em RA real).
            if (prefab != null)
                FitToTargetSize(activeModel.transform);
        }

        // Largura alvo do prato, em unidades do anchor. A etiqueta mede 0,12 m, entao o prato
        // fica quase do tamanho dela, qualquer que seja o tamanho real em que ela aparece na tela.
        private const float TargetSize = 0.11f;

        // Modelos escaneados chegam em escalas arbitrarias: ajusta o tamanho e apoia o prato no centro
        // da etiqueta. Tudo e calculado no espaco local do modelo (e nao do mundo), porque a etiqueta
        // pode estar inclinada ou de pe, e a orientacao do mundo nao serve de referencia.
        private static void FitToTargetSize(Transform model)
        {
            if (!TryGetLocalBounds(model, out var bounds))
                return;

            var largest = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (largest <= 0.0001f)
                return;

            var scale = TargetSize / largest;
            model.localScale = Vector3.one * scale;

            var bottomCenter = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            model.localPosition = new Vector3(0f, 0.005f, 0f) - bottomCenter * scale;

            Debug.Log($"[DishPresenter] {model.name}: tamanho original {largest:0.###}, escala {scale:0.###}, anchor lossyScale {model.parent.lossyScale}");
        }

        private static bool TryGetLocalBounds(Transform root, out Bounds bounds)
        {
            bounds = default;
            var found = false;

            foreach (var filter in root.GetComponentsInChildren<MeshFilter>())
            {
                if (filter.sharedMesh == null)
                    continue;

                var meshBounds = filter.sharedMesh.bounds;
                for (var i = 0; i < 8; i++)
                {
                    var sign = new Vector3((i & 1) == 0 ? -1f : 1f, (i & 2) == 0 ? -1f : 1f, (i & 4) == 0 ? -1f : 1f);
                    var corner = meshBounds.center + Vector3.Scale(meshBounds.extents, sign);
                    var point = root.InverseTransformPoint(filter.transform.TransformPoint(corner));

                    if (!found)
                    {
                        bounds = new Bounds(point, Vector3.zero);
                        found = true;
                    }
                    else
                    {
                        bounds.Encapsulate(point);
                    }
                }
            }

            return found;
        }

        private static GameObject CreatePlaceholder(MenuItem item, Transform anchor)
        {
            var root = new GameObject($"Placeholder_{item.id}");
            root.transform.SetParent(anchor, false);

            var plate = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plate.transform.SetParent(root.transform, false);
            plate.transform.localScale = new Vector3(0.11f, 0.008f, 0.11f);
            plate.GetComponent<Renderer>().material.color = new Color(0.92f, 0.92f, 0.95f);

            var colors = new[]
            {
                new Color(0.80f, 0.28f, 0.18f),
                new Color(0.20f, 0.65f, 0.30f),
                new Color(0.95f, 0.70f, 0.15f)
            };

            for (var i = 0; i < 7; i++)
            {
                var food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                food.transform.SetParent(root.transform, false);
                var angle = i * Mathf.PI * 2f / 7f;
                food.transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.032f, 0.018f, Mathf.Sin(angle) * 0.032f);
                food.transform.localScale = Vector3.one * (0.022f + (i % 2) * 0.007f);
                food.GetComponent<Renderer>().material.color = colors[i % colors.Length];
            }

            return root;
        }
    }
}

