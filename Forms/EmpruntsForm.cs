using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Data;

namespace GestionBibliotheque.Forms
{
    public partial class EmpruntsForm : UserControl
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        private DataGridView dgvEmprunts;
        private Label lblLivre, lblUtilisateur, lblDateRetourPrevue;
        private ComboBox cmbLivres, cmbUtilisateurs;
        private DateTimePicker dtpDateRetourPrevue;
        private Button btnEmprunter, btnRetourner, btnReinitialiser;
        private Panel panelInput;

        public EmpruntsForm()
        {
            InitializeComponent();
            ChargerEmprunts();
            ChargerDonneesEmprunts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // DataGridView
            dgvEmprunts = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(970, 250),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(230, 230, 250) },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(0, 51, 102),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold)
                },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(dgvEmprunts);

            // Panel pour les champs de saisie
            panelInput = new Panel
            {
                Location = new Point(15, 270),
                Size = new Size(970, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(panelInput);

            // ComboBox et Labels
            lblLivre = new Label { Text = "Livre :", Location = new Point(10, 20), Size = new Size(80, 25), Font = new Font("Segoe UI", 12) };
            cmbLivres = new ComboBox
            {
                Location = new Point(100, 20),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 12),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblUtilisateur = new Label { Text = "Utilisateur :", Location = new Point(10, 60), Size = new Size(100, 25), Font = new Font("Segoe UI", 12) };
            cmbUtilisateurs = new ComboBox
            {
                Location = new Point(120, 60),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 12),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblDateRetourPrevue = new Label { Text = "Retour prévu :", Location = new Point(10, 100), Size = new Size(120, 25), Font = new Font("Segoe UI", 12) };
            dtpDateRetourPrevue = new DateTimePicker
            {
                Location = new Point(140, 100),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 12),
                Value = DateTime.Now.AddDays(14),
                Format = DateTimePickerFormat.Short
            };

            panelInput.Controls.AddRange(new Control[] {
                lblLivre, cmbLivres, lblUtilisateur, cmbUtilisateurs, lblDateRetourPrevue, dtpDateRetourPrevue
            });

            // Boutons
            btnEmprunter = new Button
            {
                Text = "📚 Emprunter",
                Location = new Point(15, 430),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(50, 205, 50),
                ForeColor = Color.White
            };
            btnRetourner = new Button
            {
                Text = "↩️ Retourner",
                Location = new Point(145, 430),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(30, 144, 255),
                ForeColor = Color.White
            };
            btnReinitialiser = new Button
            {
                Text = "🔄 Réinitialiser",
                Location = new Point(275, 430),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(128, 128, 128),
                ForeColor = Color.White
            };

            this.Controls.AddRange(new Control[] {
                btnEmprunter, btnRetourner, btnReinitialiser
            });

            // Événements
            btnEmprunter.Click += BtnEmprunter_Click;
            btnRetourner.Click += BtnRetourner_Click;
            btnReinitialiser.Click += BtnReinitialiser_Click;

            this.Size = new Size(1000, 500);
            this.ResumeLayout(false);
        }

        private void ChargerEmprunts()
        {
            string query = @"SELECT e.Id, l.Titre as 'Livre', u.Nom as 'Utilisateur', 
                           e.DateEmprunt as 'Date Emprunt', e.DateRetourPrevue as 'Retour Prévu',
                           e.DateRetour as 'Date Retour',
                           CASE 
                               WHEN e.DateRetour IS NOT NULL THEN 'Retourné'
                               WHEN e.DateRetourPrevue < CURDATE() THEN 'En retard'
                               ELSE 'En cours'
                           END as 'Statut'
                    FROM Emprunts e
                    JOIN Livres l ON e.LivreId = l.Id
                    JOIN Utilisateurs u ON e.UtilisateurId = u.Id
                    ORDER BY e.DateEmprunt DESC";
            dgvEmprunts.DataSource = dbHelper.ExecuteQuery(query);
        }

        private void ChargerDonneesEmprunts()
        {
            // Charger les livres disponibles
            string queryLivres = "SELECT Id, CONCAT(Titre) as Affichage FROM Livres WHERE Disponible = TRUE ORDER BY Titre";
            DataTable dtLivres = dbHelper.ExecuteQuery(queryLivres);
            cmbLivres.DataSource = dtLivres;
            cmbLivres.DisplayMember = "Affichage";
            cmbLivres.ValueMember = "Id";

            // Charger les utilisateurs
            string queryUtilisateurs = "SELECT Id, CONCAT(Nom) as Affichage FROM Utilisateurs ORDER BY Nom";
            DataTable dtUtilisateurs = dbHelper.ExecuteQuery(queryUtilisateurs);
            cmbUtilisateurs.DataSource = dtUtilisateurs;
            cmbUtilisateurs.DisplayMember = "Affichage";
            cmbUtilisateurs.ValueMember = "Id";
        }

        private void BtnEmprunter_Click(object sender, EventArgs e)
        {
            if (cmbLivres.SelectedValue == null || cmbUtilisateurs.SelectedValue == null)
            {
                MessageBox.Show("Veuillez sélectionner un livre et un utilisateur.", "Champs requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Vérifier si l'utilisateur a déjà emprunté ce livre
            string checkEmpruntQuery = "SELECT COUNT(*) FROM Emprunts WHERE LivreId = @LivreId AND UtilisateurId = @UtilisateurId AND DateRetour IS NULL";
            MySqlParameter[] checkParams = {
                new MySqlParameter("@LivreId", cmbLivres.SelectedValue),
                new MySqlParameter("@UtilisateurId", cmbUtilisateurs.SelectedValue)
            };

            int empruntExistant = Convert.ToInt32(dbHelper.ExecuteScalar(checkEmpruntQuery, checkParams));
            if (empruntExistant > 0)
            {
                MessageBox.Show("Cet utilisateur a déjà emprunté ce livre !", "Emprunt existant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ajouter l'emprunt
            string query = "INSERT INTO Emprunts (LivreId, UtilisateurId, DateEmprunt, DateRetourPrevue) VALUES (@LivreId, @UtilisateurId, @DateEmprunt, @DateRetourPrevue)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@LivreId", cmbLivres.SelectedValue),
                new MySqlParameter("@UtilisateurId", cmbUtilisateurs.SelectedValue),
                new MySqlParameter("@DateEmprunt", DateTime.Now),
                new MySqlParameter("@DateRetourPrevue", dtpDateRetourPrevue.Value)
            };

            if (dbHelper.ExecuteNonQuery(query, parameters) > 0)
            {
                // Mettre à jour la disponibilité du livre
                string updateQuery = "UPDATE Livres SET Disponible = FALSE WHERE Id = @LivreId";
                MySqlParameter[] updateParams = {
                    new MySqlParameter("@LivreId", cmbLivres.SelectedValue)
                };
                dbHelper.ExecuteNonQuery(updateQuery, updateParams);

                MessageBox.Show("Livre emprunté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerEmprunts();
                ChargerDonneesEmprunts();
                ReinitialiserFormulaire();
            }
        }

        private void BtnRetourner_Click(object sender, EventArgs e)
        {
            if (dgvEmprunts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Sélectionnez un emprunt à retourner.", "Aucune sélection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvEmprunts.SelectedRows[0];
            string statut = selectedRow.Cells["Statut"].Value?.ToString();

            if (statut == "Retourné")
            {
                MessageBox.Show("Ce livre a déjà été retourné.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("Êtes-vous sûr de vouloir marquer ce livre comme retourné ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Mettre à jour la date de retour
                string query = "UPDATE Emprunts SET DateRetour = @DateRetour WHERE Id = @Id";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@DateRetour", DateTime.Now),
                    new MySqlParameter("@Id", Convert.ToInt32(selectedRow.Cells["Id"].Value))
                };

                if (dbHelper.ExecuteNonQuery(query, parameters) > 0)
                {
                    // Récupérer l'ID du livre et le remettre disponible
                    string getLivreIdQuery = "SELECT LivreId FROM Emprunts WHERE Id = @Id";
                    MySqlParameter[] getLivreParams = {
                        new MySqlParameter("@Id", Convert.ToInt32(selectedRow.Cells["Id"].Value))
                    };

                    int livreId = Convert.ToInt32(dbHelper.ExecuteScalar(getLivreIdQuery, getLivreParams));

                    string updateQuery = "UPDATE Livres SET Disponible = TRUE WHERE Id = @LivreId";
                    MySqlParameter[] updateParams = {
                        new MySqlParameter("@LivreId", livreId)
                    };
                    dbHelper.ExecuteNonQuery(updateQuery, updateParams);

                    MessageBox.Show("Livre retourné avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ChargerEmprunts();
                    ChargerDonneesEmprunts();
                }
            }
        }

        private void BtnReinitialiser_Click(object sender, EventArgs e)
        {
            ReinitialiserFormulaire();
        }

        private void ReinitialiserFormulaire()
        {
            dtpDateRetourPrevue.Value = DateTime.Now.AddDays(14);
        }
    }
}