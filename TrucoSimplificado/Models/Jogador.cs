using System.Collections.Generic;
using System;

namespace TrucoSimplificado.Models
{
    // Representa um jogador no jogo de Truco.
    public class Jogador
    {
        // O nome do jogador.
        public string Nome;

        // A lista de cartas na mão do jogador.
        public List<Carta> Mao;

        // A pontuação atual do jogador no jogo.
        public int Pontuacao;

        // Construtor da classe Jogador.
        public Jogador(string nome)
        {
            Nome = nome;
            Mao = new List<Carta>();
            Pontuacao = 0;
        }

        // Adiciona uma carta à mão do jogador.
        public void ReceberCarta(Carta carta)
        {
            Mao.Add(carta);
        }

        // Joga uma carta da mão do jogador.
        public Carta JogarCarta(int indice)
        {
            if (indice >= 0 && indice < Mao.Count)
            {
                Carta carta = Mao[indice];
                Mao.RemoveAt(indice);
                return carta;
            }
            return null;
        }

        // Conta o número de cartas na mão do jogador.
        public int ContarCartas()
        {
            return Mao.Count;
        }
    }
}