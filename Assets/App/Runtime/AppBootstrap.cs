using System;
using UnityEngine;

namespace CardapioRA
{
    public sealed class AppBootstrap : MonoBehaviour
    {
        private static readonly string[] SusQuestions =
        {
            "Eu usaria este aplicativo com frequencia.",
            "Achei o aplicativo desnecessariamente complexo.",
            "Achei o aplicativo facil de usar.",
            "Precisaria de ajuda tecnica para usar o aplicativo.",
            "As funcoes do aplicativo estao bem integradas.",
            "Ha muita inconsistencia no aplicativo.",
            "A maioria das pessoas aprenderia a usa-lo rapidamente.",
            "Achei o aplicativo confuso de usar.",
            "Senti confianca ao usar o aplicativo.",
            "Precisei aprender muitas coisas antes de usa-lo."
        };

        [SerializeField] private ArImageTracker arTracker;
        [SerializeField] private Transform simulationAnchor;

        private MenuCatalogService catalog;
        private ExperimentRecorder recorder;
        private readonly DishPresenter presenter = new();
        private MenuItem selected;
        private Vector2 scroll;
        private bool showSus;
        private int susQuestion;
        private int[] susAnswers = new int[10];
        private string status = "Aponte a camera para uma etiqueta ou use a simulacao.";

        private void Awake()
        {
            catalog = new MenuCatalogService(Resources.Load<TextAsset>("menu_catalog"));
            recorder = new ExperimentRecorder();
            recorder.Record("session_started");

            if (simulationAnchor == null)
            {
                var anchor = new GameObject("SimulationAnchor");
                anchor.transform.position = Vector3.zero;
                simulationAnchor = anchor.transform;
            }

            if (arTracker != null)
                arTracker.MarkerTracked += OnMarkerTracked;
        }

        private void OnDestroy()
        {
            if (arTracker != null)
                arTracker.MarkerTracked -= OnMarkerTracked;
        }

        private void OnMarkerTracked(string markerId, Transform anchor)
        {
            if (!catalog.TryFindByMarker(markerId, out var item))
            {
                status = $"Marcador sem prato cadastrado: {markerId}";
                recorder.Record("unknown_marker", value: markerId);
                return;
            }

            SelectDish(item, anchor, "ar_marker");
        }

        private void SelectDish(MenuItem item, Transform anchor, string source)
        {
            if (selected?.id == item.id && source == "ar_marker")
                return;

            selected = item;
            presenter.Show(item, anchor);
            status = $"Prato identificado: {item.name}";
            recorder.Record("dish_presented", item.id, source);
        }

        private void OnGUI()
        {
            var scale = Mathf.Max(1f, Screen.width / 900f);
            var oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            var width = Screen.width / scale;
            var height = Screen.height / scale;

            GUI.Box(new Rect(16, 16, 360, height - 32), "Cardapio RA");
            GUILayout.BeginArea(new Rect(32, 50, 328, height - 82));
            GUILayout.Label(status);
            GUILayout.Space(10);

            if (showSus)
                DrawSus();
            else
                DrawMenu();

            GUILayout.EndArea();
            GUI.matrix = oldMatrix;
        }

        private void DrawMenu()
        {
            GUILayout.Label("SIMULACAO DE MARCADOR");
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(170));
            foreach (var item in catalog.Items)
            {
                if (GUILayout.Button(item.name, GUILayout.Height(42)))
                    SelectDish(item, simulationAnchor, "editor_simulation");
            }
            GUILayout.EndScrollView();

            if (selected != null)
            {
                GUILayout.Space(12);
                GUILayout.Label($"{selected.name}  |  {selected.PriceLabel}");
                GUILayout.Label(selected.description);
                GUILayout.Label($"Porcao: {selected.servingSizeGrams} g");
                GUILayout.Label($"Energia: {selected.nutrition.energyKcal} kcal");
                GUILayout.Label($"Carboidratos: {selected.nutrition.carbohydratesGrams:0.#} g");
                GUILayout.Label($"Proteinas: {selected.nutrition.proteinGrams:0.#} g");
                GUILayout.Label($"Gorduras totais: {selected.nutrition.totalFatGrams:0.#} g");
                GUILayout.Label($"Fibras: {selected.nutrition.fiberGrams:0.#} g");
                GUILayout.Label($"Sodio: {selected.nutrition.sodiumMilligrams} mg");
                GUILayout.Label(selected.AllergenLabel);
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Responder questionario SUS", GUILayout.Height(46)))
            {
                showSus = true;
                recorder.Record("sus_started", selected?.id);
            }
            GUILayout.Label($"Sessao anonima: {recorder.SessionId[..8]}");
        }

        private void DrawSus()
        {
            GUILayout.Label($"SUS - questao {susQuestion + 1} de 10");
            GUILayout.Space(18);
            GUILayout.Label(SusQuestions[susQuestion]);
            GUILayout.Space(12);
            GUILayout.Label("1 = discordo totalmente   5 = concordo totalmente");

            for (var answer = 1; answer <= 5; answer++)
            {
                if (GUILayout.Button(answer.ToString(), GUILayout.Height(40)))
                {
                    susAnswers[susQuestion] = answer;
                    susQuestion++;
                    if (susQuestion == 10)
                    {
                        var score = SusCalculator.Calculate(susAnswers);
                        recorder.Record("sus_completed", selected?.id, score.ToString("0.0"));
                        status = $"Questionario concluido. Pontuacao SUS: {score:0.0}";
                        showSus = false;
                        susQuestion = 0;
                        susAnswers = new int[10];
                    }
                }
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancelar"))
            {
                showSus = false;
                susQuestion = 0;
                susAnswers = new int[10];
                recorder.Record("sus_cancelled", selected?.id);
            }
        }
    }
}

