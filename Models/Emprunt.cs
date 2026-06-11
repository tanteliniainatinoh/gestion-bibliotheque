using System;

namespace GestionBibliotheque.Models
{
    public class Emprunt
    {
        public int Id { get; set; }
        public int LivreId { get; set; }
        public int UtilisateurId { get; set; }
        public DateTime DateEmprunt { get; set; }
        public DateTime DateRetourPrevue { get; set; }
        public DateTime? DateRetour { get; set; }
        public string Statut { get; set; }
        public string LivreTitre { get; set; }
        public string UtilisateurNom { get; set; }
    }
}