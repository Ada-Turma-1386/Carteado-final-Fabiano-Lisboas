using System;
using TrucoSimplificado.Models;

namespace TrucoSimplificado.Utils
{
    // Fornece métodos para interagir com o usuário através do console.
    public static class ConsoleInterface
    {
        // Exibe uma mensagem no console.
        public static void ExibirMensagem(string mensagem)
        {
            Console.WriteLine(mensagem);
        }

        // Limpa a tela do console.
        public static void LimparTela()
        {
            Console.Clear();
        }

        // Aguarda o usuário pressionar uma tecla.
        public static void AguardarTecla()
        {
            Console.ReadKey();
        }

        // Lê uma linha de texto da entrada do console.
        public static string ObterEntrada()
        {
            return Console.ReadLine();
        }

        // Exibe as cartas na mão de um jogador.
        public static void ExibirMao(Jogador jogador)
        {
            if (jogador.ContarCartas() == 0)
            {
                Console.WriteLine("   (Nenhuma carta na mão)");
                return;
            }
            
            for (int i = 0; i < jogador.ContarCartas(); i++)
            {
                Console.WriteLine($"   {i + 1}. {jogador.Mao[i].ObterTexto()}");
            }
        }

        // Exibe as opções de jogada disponíveis para o jogador.
        public static void ExibirOpcoes(Jogador jogador, bool podeApostar, int proximaAposta)
        {
            Console.WriteLine("\n⚡ OPÇÕES DISPONÍVEIS:");
            
            // Exibe apenas as opções de apostas, não as cartas (já exibidas anteriormente)
            if (podeApostar && proximaAposta > 0)
            {
                switch (proximaAposta)
                {
                    case 3:
                        Console.WriteLine("   T - 🔥 PEDIR TRUCO (1 → 3 pontos)");
                        break;
                    case 6:
                        Console.WriteLine("   S - 🔥🔥 PEDIR SEIS (3 → 6 pontos)");
                        break;
                    case 9:
                        Console.WriteLine("   N - 🔥🔥🔥 PEDIR NOVE (6 → 9 pontos)");
                        break;
                    case 12:
                        Console.WriteLine("   D - 🔥🔥🔥🔥 PEDIR DOZE (9 → 12 pontos)");
                        break;
                }
            }
            
            // Exibe instrução para jogar carta
            Console.WriteLine("   Digite o número da carta (1, 2 ou 3) para jogar");
        }
    }
}