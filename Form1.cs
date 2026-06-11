using System;
using System.Drawing;
using System.Windows.Forms;
using GestionBibliotheque.Forms;

namespace GestionBibliotheque
{
    public partial class Form1 : Form
    {
        private TabControl tabControlPrincipal;

        public Form1()
        {
            SetupForm();
        }

        private void SetupForm()
        {
            // Configuration de la fenêtre principale
            this.Text = "Gestion de Bibliothèque - Système Complet";
            this.Size = new Size(1200, 800);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Création du TabControl
            tabControlPrincipal = new TabControl
            {
                Location = new Point(15, 15),
                Size = new Size(1150, 730),
                Font = new Font("Segoe UI", 12)
            };
            this.Controls.Add(tabControlPrincipal);

            // Onglet Livres
            var tabLivres = new TabPage
            {
                Text = "📚 Livres",
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(3)
            };

            var livresForm = new LivresForm();
            livresForm.Dock = DockStyle.Fill;
            tabLivres.Controls.Add(livresForm);
            tabControlPrincipal.TabPages.Add(tabLivres);

            // Onglet Utilisateurs
            var tabUtilisateurs = new TabPage
            {
                Text = "👥 Utilisateurs",
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(3)
            };

            var utilisateursForm = new UtilisateursForm();
            utilisateursForm.Dock = DockStyle.Fill;
            tabUtilisateurs.Controls.Add(utilisateursForm);
            tabControlPrincipal.TabPages.Add(tabUtilisateurs);

            // Onglet Emprunts
            var tabEmprunts = new TabPage
            {
                Text = "📋 Emprunts",
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(3)
            };

            var empruntsForm = new EmpruntsForm();
            empruntsForm.Dock = DockStyle.Fill;
            tabEmprunts.Controls.Add(empruntsForm);
            tabControlPrincipal.TabPages.Add(tabEmprunts);

            // Onglet Statistiques
            var tabStatistiques = new TabPage
            {
                Text = "📊 Statistiques",
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(3)
            };

            var statistiquesForm = new StatistiquesForm();
            statistiquesForm.Dock = DockStyle.Fill;
            tabStatistiques.Controls.Add(statistiquesForm);
            tabControlPrincipal.TabPages.Add(tabStatistiques);
        }
    }
}