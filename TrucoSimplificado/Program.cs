using TrucoSimplificado.Logica;

namespace TrucoSimplificado
{
    // Ponto de entrada principal para a aplicação do jogo de Truco.
    public class Program
    {
        // O método principal que inicia a execução do jogo.
        public static void Main(string[] args)
        {
            // Cria uma nova instância do jogo de Truco Simplificado.
            var jogo = new TrucoSimplificado();
            // Inicia o jogo.
            jogo.Jogar();
        }
    }
}