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

            if (prefab != null)
                FitToTargetSize(activeModel.transform);
        }

        // Modelos escaneados chegam em escalas arbitrarias: ajusta para ~12 cm e apoia na etiqueta.
        private static void FitToTargetSize(Transform model)
        {
            var targetSizeMeters = 0.12f * model.parent.lossyScale.x;

            var renderers = model.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return;

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            var largest = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (largest <= 0.0001f)
                return;

            model.localScale *= targetSizeMeters / largest;

            bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            var offset = model.parent.position - new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            model.position += offset + Vector3.up * 0.005f;
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

