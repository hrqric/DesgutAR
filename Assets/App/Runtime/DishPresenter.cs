using UnityEngine;

namespace CardapioRA
{
    public sealed class DishPresenter
    {
        private GameObject activeModel;

        public void Show(MenuItem item, Transform anchor)
        {
            if (activeModel != null)
                Object.Destroy(activeModel);

            var prefab = string.IsNullOrWhiteSpace(item.modelResource)
                ? null
                : Resources.Load<GameObject>(item.modelResource);

            activeModel = prefab != null
                ? Object.Instantiate(prefab, anchor)
                : CreatePlaceholder(item, anchor);

            activeModel.name = $"Dish_{item.id}";
            activeModel.transform.localPosition = new Vector3(0f, 0.035f, 0f);
            activeModel.transform.localRotation = Quaternion.identity;
            activeModel.transform.localScale = Vector3.one;
        }

        private static GameObject CreatePlaceholder(MenuItem item, Transform anchor)
        {
            var root = new GameObject($"Placeholder_{item.id}");
            root.transform.SetParent(anchor, false);

            var plate = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plate.transform.SetParent(root.transform, false);
            plate.transform.localScale = new Vector3(0.55f, 0.025f, 0.55f);
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
                food.transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.16f, 0.08f, Mathf.Sin(angle) * 0.16f);
                food.transform.localScale = Vector3.one * (0.11f + (i % 2) * 0.035f);
                food.GetComponent<Renderer>().material.color = colors[i % colors.Length];
            }

            return root;
        }
    }
}

