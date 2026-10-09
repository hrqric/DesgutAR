using System;
using System.IO;
using UnityEngine;

namespace CardapioRA
{
    [Serializable]
    internal sealed class ExperimentEvent
    {
        public string sessionId;
        public string eventName;
        public string dishId;
        public string value;
        public string timestampUtc;
        public float secondsSinceStart;
    }

    public sealed class ExperimentRecorder
    {
        private readonly string sessionId = Guid.NewGuid().ToString("N");
        private readonly float startedAt = Time.realtimeSinceStartup;
        private readonly string path;

        public string SessionId => sessionId;
        public string OutputPath => path;

        public ExperimentRecorder()
        {
            path = Path.Combine(Application.persistentDataPath, "experiment-events.jsonl");
        }

        public void Record(string eventName, string dishId = "", string value = "")
        {
            var entry = new ExperimentEvent
            {
                sessionId = sessionId,
                eventName = eventName,
                dishId = dishId ?? string.Empty,
                value = value ?? string.Empty,
                timestampUtc = DateTime.UtcNow.ToString("O"),
                secondsSinceStart = Time.realtimeSinceStartup - startedAt
            };

            try
            {
                File.AppendAllText(path, JsonUtility.ToJson(entry) + Environment.NewLine);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Nao foi possivel registrar o evento: {exception.Message}");
            }
        }
    }
}

