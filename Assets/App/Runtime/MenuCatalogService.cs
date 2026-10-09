using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardapioRA
{
    public sealed class MenuCatalogService
    {
        private readonly MenuCatalog catalog;
        private readonly Dictionary<string, MenuItem> byMarker = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyList<MenuItem> Items => catalog.items;

        public MenuCatalogService(TextAsset source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "O catalogo Resources/menu_catalog.json nao foi encontrado.");

            catalog = JsonUtility.FromJson<MenuCatalog>(source.text);
            if (catalog?.items == null || catalog.items.Length == 0)
                throw new InvalidOperationException("O catalogo nao possui pratos.");

            foreach (var item in catalog.items)
            {
                if (string.IsNullOrWhiteSpace(item.id) || string.IsNullOrWhiteSpace(item.markerId))
                    throw new InvalidOperationException("Todo prato deve possuir id e markerId.");
                if (!byMarker.TryAdd(item.markerId, item))
                    throw new InvalidOperationException($"Marcador duplicado: {item.markerId}");
            }
        }

        public bool TryFindByMarker(string markerId, out MenuItem item) =>
            byMarker.TryGetValue(markerId ?? string.Empty, out item);
    }
}

