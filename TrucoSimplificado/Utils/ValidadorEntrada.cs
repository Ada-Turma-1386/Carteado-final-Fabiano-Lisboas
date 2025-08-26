using System;

namespace TrucoSimplificado.Utils
{
    // Fornece métodos estáticos para validar a entrada do usuário.
    public static class ValidadorEntrada
    {
        // Valida se a entrada do usuário corresponde a uma escolha de carta válida.
        public static bool ValidarEscolhaCarta(string input, int maxCartas)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (int.TryParse(input.Trim(), out int escolha))
            {
                return escolha >= 1 && escolha <= maxCartas;
            }
            return false;
        }

        // Valida se a aposta que o jogador tentou fazer está disponível na rodada.
        public static bool ValidarApostaDisponivel(string input, int pontosRodada)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string inputUpper = input.Trim().ToUpper();

            switch (pontosRodada)
            {
                case 1:
                    return inputUpper == "T"; // Truco
                case 3:
                    return inputUpper == "S"; // Seis
                case 6:
                    return inputUpper == "N"; // Nove
                case 9:
                    return inputUpper == "D"; // Doze
                default:
                    return false;
            }
        }

        // Valida se a resposta do usuário a um pedido de truco é válida (sim ou não).
        public static bool ValidarRespostaTruco(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string resposta = input.Trim().ToLower();
            return resposta == "s" || resposta == "sim" || resposta == "n" || resposta == "nao" || resposta == "não";
        }

        // Verifica se a resposta do usuário é "sim".
        public static bool RespostaEhSim(string input)
        {
            string resposta = input.Trim().ToLower();
            return resposta == "s" || resposta == "sim";
        }

        // Obtém uma escolha numérica válida do usuário dentro de um intervalo.
        public static int ObterEscolhaValida(string prompt, int minimo, int maximo)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (ValidarEscolhaCarta(input, maximo))
                {
                    return int.Parse(input.Trim());
                }

                Console.WriteLine($"❌ Opção inválida! Digite um número entre {minimo} e {maximo}.");
            }
        }
    }
}
