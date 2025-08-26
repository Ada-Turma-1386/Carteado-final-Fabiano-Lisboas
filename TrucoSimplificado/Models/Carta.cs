using System.Collections.Generic;

namespace TrucoSimplificado.Models
{
    // Representa uma carta do baralho de Truco.
    public class Carta
    {
        // O naipe da carta. 1=Copas, 2=Espadas, 3=Ouros, 4=Paus.
        public int Naipe { get; }

        // O valor da carta. 4, 5, 6, 7, 11(Q), 12(J), 13(K), 14(A), 15(2), 16(3).
        public int Valor { get; }

        // Dicionários para mapear 'traduzir' os valores e naipes.
        private static readonly Dictionary<int, string> NomesValores = new Dictionary<int, string>
        {
            {4, "4"}, {5, "5"}, {6, "6"}, {7, "7"}, {11, "Q"}, {12, "J"}, {13, "K"}, {14, "A"}, {15, "2"}, {16, "3"}
        };

        private static readonly Dictionary<int, string> NomesNaipes = new Dictionary<int, string>
        {
            {1, "Copas"}, {2, "Espadas"}, {3, "Ouros"}, {4, "Paus"}
        };

        // A força da manilha é definida pelo naipe.
        private static readonly Dictionary<int, int> ForcaManilha = new Dictionary<int, int>
        {
            {4, 40}, // Paus (Zap)
            {1, 39}, // Copas
            {2, 38}, // Espadas
            {3, 37}  // Ouros
        };

        // Construtor da classe Carta.
        public Carta(int naipe, int valor)
        {
            Naipe = naipe;
            Valor = valor;
        }

        // Calcula a força da carta.
        public int ObterForca(int manilha)
        {
            // Se a carta é uma manilha, a força é especial.
            if (Valor == manilha && ForcaManilha.ContainsKey(Naipe))
            {
                return ForcaManilha[Naipe];
            }
            // Senão, a força é o valor normal.
            return Valor;
        }

        // Retorna o nome da carta.
        public string ObterTexto()
        {
            string nomeValor = NomesValores.ContainsKey(Valor) ? NomesValores[Valor] : "?";
            string nomeNaipe = NomesNaipes.ContainsKey(Naipe) ? NomesNaipes[Naipe] : "?";
            return $"{nomeValor} de {nomeNaipe}";
        }

        // Obtém o nome de um valor de carta.
        public static string ObterNomeValor(int valor)
        {
            return NomesValores.ContainsKey(valor) ? NomesValores[valor] : "?";
        }
    }
}