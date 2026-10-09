using System;
using UnityEngine;

namespace CardapioRA
{
    [Serializable]
    public sealed class MenuCatalog
    {
        public MenuItem[] items = Array.Empty<MenuItem>();
    }

    [Serializable]
    public sealed class MenuItem
    {
        public string id;
        public string markerId;
        public string name;
        public string description;
        public string modelResource;
        public float price;
        public int servingSizeGrams;
        public Nutrition nutrition = new();
        public string[] allergens = Array.Empty<string>();

        public string PriceLabel => $"R$ {price:0.00}";
        public string AllergenLabel => allergens == null || allergens.Length == 0
            ? "Nao contem alergênicos declarados"
            : "Alergenicos: " + string.Join(", ", allergens);
    }

    [Serializable]
    public sealed class Nutrition
    {
        public int energyKcal;
        public float carbohydratesGrams;
        public float proteinGrams;
        public float totalFatGrams;
        public float fiberGrams;
        public int sodiumMilligrams;
    }
}

