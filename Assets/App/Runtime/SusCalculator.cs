using System;

namespace CardapioRA
{
    public static class SusCalculator
    {
        public static float Calculate(int[] answers)
        {
            if (answers == null || answers.Length != 10)
                throw new ArgumentException("A escala SUS exige exatamente 10 respostas.", nameof(answers));

            var sum = 0;
            for (var i = 0; i < answers.Length; i++)
            {
                if (answers[i] is < 1 or > 5)
                    throw new ArgumentOutOfRangeException(nameof(answers), "Cada resposta deve estar entre 1 e 5.");
                sum += i % 2 == 0 ? answers[i] - 1 : 5 - answers[i];
            }
            return sum * 2.5f;
        }
    }
}

