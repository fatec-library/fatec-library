namespace Fatec_Library.Helpers
{
    public static class AvatarHelper
    {
        public static string BackgroundColor(string nomeUsuario)
        {
            int valorAleatorio= nomeUsuario.GetHashCode();
            Random random = new Random(valorAleatorio);

            string[] cores = new[]
            {
            "#FF5733", "#33C1FF", "#28B463", "#9B59B6", "#F1C40F", "#1ABC9C"
        };

            return cores[random.Next(cores.Length)];
        }
    }

}
