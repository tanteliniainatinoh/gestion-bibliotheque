namespace GestionBibliotheque.Models
{
    public class Livre
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Auteur { get; set; }
        public string ISBN { get; set; }
        public int? AnneePublication { get; set; }
        public bool Disponible { get; set; }
    }
}