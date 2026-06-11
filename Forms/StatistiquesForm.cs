using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Data;

namespace GestionBibliotheque.Forms
{
    public partial class StatistiquesForm : UserControl
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        private Label lblStatsLivres, lblStatsUtilisateurs, lblStatsEmprunts, lblStatsRetards;
        private DataGridView dgvStatsLivresPopulaires, dgvStatsUtilisateursActifs;
        private Button btnRafraichirStats;

        public StatistiquesForm()
        {
            InitializeComponent();
            ChargerStatistiques();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Labels pour les statistiques principales
            lblStatsLivres = new Label
            {
                Text = "Total Livres: 0",
                Location = new Point(20, 20),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 51, 102)
            };

            lblStatsUtilisateurs = new Label
            {
                Text = "Total Utilisateurs: 0",
                Location = new Point(20, 50),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 51, 102)
            };

            lblStatsEmprunts = new Label
            {
                Text = "Emprunts Actifs: 0",
                Location = new Point(20, 80),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 51, 102)
            };

            lblStatsRetards = new Label
            {
                Text = "Retards: 0",
                Location = new Point(20, 110),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 60)
            };

            // DataGridView pour les livres populaires
            dgvStatsLivresPopulaires = new DataGridView
            {
                Location = new Point(20, 150),
                Size = new Size(550, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(0, 51, 102),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                }
            };

            // DataGridView pour les utilisateurs actifs
            dgvStatsUtilisateursActifs = new DataGridView
            {
                Location = new Point(580, 150),
                Size = new Size(550, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(0, 51, 102),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                }
            };

            // Bouton de rafraîchissement
            btnRafraichirStats = new Button
            {
                Text = "🔄 Rafraîchir Statistiques",
                Location = new Point(20, 370),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White
            };

            // Ajout des contrôles au UserControl
            this.Controls.AddRange(new Control[] {
                lblStatsLivres, lblStatsUtilisateurs, lblStatsEmprunts, lblStatsRetards,
                dgvStatsLivresPopulaires, dgvStatsUtilisateursActifs, btnRafraichirStats
            });

            // Événements
            btnRafraichirStats.Click += BtnRafraichirStats_Click;

            this.Size = new Size(1150, 450);
            this.ResumeLayout(false);
        }

        private void ChargerStatistiques()
        {
            try
            {
                // Statistiques de base
                string statsQuery = @"
                    SELECT 
                        (SELECT COUNT(*) FROM Livres) as TotalLivres,
                        (SELECT COUNT(*) FROM Utilisateurs) as TotalUtilisateurs,
                        (SELECT COUNT(*) FROM Emprunts WHERE DateRetour IS NULL) as EmpruntsActifs,
                        (SELECT COUNT(*) FROM Emprunts WHERE DateRetour IS NULL AND DateRetourPrevue < CURDATE()) as Retards";

                DataTable statsTable = dbHelper.ExecuteQuery(statsQuery);

                if (statsTable.Rows.Count > 0)
                {
                    DataRow row = statsTable.Rows[0];
                    lblStatsLivres.Text = $"Total Livres: {row["TotalLivres"]}";
                    lblStatsUtilisateurs.Text = $"Total Utilisateurs: {row["TotalUtilisateurs"]}";
                    lblStatsEmprunts.Text = $"Emprunts Actifs: {row["EmpruntsActifs"]}";
                    lblStatsRetards.Text = $"Retards: {row["Retards"]}";
                }

                // Livres les plus populaires
                string livresPopulairesQuery = @"
                    SELECT l.Titre, l.Auteur, COUNT(e.Id) as NombreEmprunts
                    FROM Livres l
                    LEFT JOIN Emprunts e ON l.Id = e.LivreId
                    GROUP BY l.Id, l.Titre, l.Auteur
                    ORDER BY NombreEmprunts DESC
                    LIMIT 10";

                dgvStatsLivresPopulaires.DataSource = dbHelper.ExecuteQuery(livresPopulairesQuery);

                // Utilisateurs les plus actifs
                string utilisateursActifsQuery = @"
                    SELECT u.Nom, u.Email, COUNT(e.Id) as NombreEmprunts
                    FROM Utilisateurs u
                    LEFT JOIN Emprunts e ON u.Id = e.UtilisateurId
                    GROUP BY u.Id, u.Nom, u.Email
                    ORDER BY NombreEmprunts DESC
                    LIMIT 10";

                dgvStatsUtilisateursActifs.DataSource = dbHelper.ExecuteQuery(utilisateursActifsQuery);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des statistiques : " + ex.Message,
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRafraichirStats_Click(object sender, EventArgs e)
        {
            ChargerStatistiques();
            //MessageBox.Show("Statistiques rafraîchies avec succès !",
            //    "Rafraîchissement", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}