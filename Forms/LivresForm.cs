using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Data;

namespace GestionBibliotheque.Forms
{
    public partial class LivresForm : UserControl
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        private DataGridView dgvLivres;
        private Label lblTitre, lblAuteur, lblISBN, lblAnnee, lblRecherche, lblDisponible;
        private TextBox txtTitre, txtAuteur, txtISBN, txtAnnee, txtRecherche;
        private CheckBox chkDisponible;
        private Button btnAjouter, btnModifier, btnSupprimer, btnRechercher, btnReinitialiser;
        private Panel panelInput;

        public LivresForm()
        {
            InitializeComponent();
            ChargerLivres();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // DataGridView
            dgvLivres = new DataGridView
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
            this.Controls.Add(dgvLivres);

            // Panel pour les champs de saisie
            panelInput = new Panel
            {
                Location = new Point(15, 270),
                Size = new Size(970, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(panelInput);

            // Labels et TextBox
            lblTitre = new Label { Text = "Titre :", Location = new Point(10, 20), Size = new Size(80, 25), Font = new Font("Segoe UI", 12) };
            txtTitre = new TextBox { Location = new Point(100, 20), Size = new Size(250, 30), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };

            lblAuteur = new Label { Text = "Auteur :", Location = new Point(10, 60), Size = new Size(80, 25), Font = new Font("Segoe UI", 12) };
            txtAuteur = new TextBox { Location = new Point(100, 60), Size = new Size(250, 30), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };

            lblISBN = new Label { Text = "ISBN :", Location = new Point(10, 100), Size = new Size(80, 25), Font = new Font("Segoe UI", 12) };
            txtISBN = new TextBox { Location = new Point(100, 100), Size = new Size(250, 30), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };

            lblAnnee = new Label { Text = "Année :", Location = new Point(10, 140), Size = new Size(80, 25), Font = new Font("Segoe UI", 12) };
            txtAnnee = new TextBox { Location = new Point(100, 140), Size = new Size(100, 30), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };

            lblDisponible = new Label { Text = "Disponible :", Location = new Point(220, 140), Size = new Size(100, 25), Font = new Font("Segoe UI", 12) };
            chkDisponible = new CheckBox { Location = new Point(330, 140), Size = new Size(20, 20), Checked = true };

            lblRecherche = new Label { Text = "Recherche :", Location = new Point(360, 20), Size = new Size(100, 25), Font = new Font("Segoe UI", 12) };
            txtRecherche = new TextBox { Location = new Point(470, 20), Size = new Size(250, 30), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };

            panelInput.Controls.AddRange(new Control[] {
                lblTitre, txtTitre, lblAuteur, txtAuteur, lblISBN, txtISBN,
                lblAnnee, txtAnnee, lblDisponible, chkDisponible, lblRecherche, txtRecherche
            });

            // Boutons
            btnAjouter = new Button
            {
                Text = "➕ Ajouter",
                Location = new Point(15, 480),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(50, 205, 50),
                ForeColor = Color.White
            };
            btnModifier = new Button
            {
                Text = "✏️ Modifier",
                Location = new Point(145, 480),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(30, 144, 255),
                ForeColor = Color.White
            };
            btnSupprimer = new Button
            {
                Text = "🗑️ Supprimer",
                Location = new Point(275, 480),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(220, 20, 60),
                ForeColor = Color.White
            };
            btnRechercher = new Button
            {
                Text = "🔍 Rechercher",
                Location = new Point(405, 480),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(255, 165, 0),
                ForeColor = Color.White
            };
            btnReinitialiser = new Button
            {
                Text = "🔄 Réinitialiser",
                Location = new Point(535, 480),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(128, 128, 128),
                ForeColor = Color.White
            };

            this.Controls.AddRange(new Control[] {
                btnAjouter, btnModifier, btnSupprimer, btnRechercher, btnReinitialiser
            });

            // Événements
            btnAjouter.Click += BtnAjouter_Click;
            btnModifier.Click += BtnModifier_Click;
            btnSupprimer.Click += BtnSupprimer_Click;
            btnRechercher.Click += BtnRechercher_Click;
            btnReinitialiser.Click += BtnReinitialiser_Click;
            dgvLivres.CellClick += DgvLivres_CellClick;

            this.Size = new Size(1000, 550);
            this.ResumeLayout(false);
        }

        private void ChargerLivres()
        {
            string query = "SELECT * FROM Livres ORDER BY Titre";
            dgvLivres.DataSource = dbHelper.ExecuteQuery(query);
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitre.Text) || string.IsNullOrWhiteSpace(txtAuteur.Text))
            {
                MessageBox.Show("Veuillez remplir le titre et l'auteur.", "Champs requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO Livres (Titre, Auteur, ISBN, AnneePublication, Disponible) VALUES (@Titre, @Auteur, @ISBN, @Annee, @Disponible)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Titre", txtTitre.Text),
                new MySqlParameter("@Auteur", txtAuteur.Text),
                new MySqlParameter("@ISBN", txtISBN.Text),
                new MySqlParameter("@Annee", string.IsNullOrEmpty(txtAnnee.Text) ? (object)DBNull.Value : int.Parse(txtAnnee.Text)),
                new MySqlParameter("@Disponible", chkDisponible.Checked)
            };

            if (dbHelper.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Livre ajouté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerLivres();
                ReinitialiserFormulaire();
            }
        }

        private void BtnModifier_Click(object sender, EventArgs e)
        {
            if (dgvLivres.SelectedRows.Count == 0)
            {
                MessageBox.Show("Sélectionnez un livre à modifier.", "Aucune sélection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "UPDATE Livres SET Titre=@Titre, Auteur=@Auteur, ISBN=@ISBN, AnneePublication=@Annee, Disponible=@Disponible WHERE Id=@Id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Titre", txtTitre.Text),
                new MySqlParameter("@Auteur", txtAuteur.Text),
                new MySqlParameter("@ISBN", txtISBN.Text),
                new MySqlParameter("@Annee", string.IsNullOrEmpty(txtAnnee.Text) ? (object)DBNull.Value : int.Parse(txtAnnee.Text)),
                new MySqlParameter("@Disponible", chkDisponible.Checked),
                new MySqlParameter("@Id", Convert.ToInt32(dgvLivres.SelectedRows[0].Cells["Id"].Value))
            };

            if (dbHelper.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Livre modifié avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerLivres();
            }
        }

        private void BtnSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvLivres.SelectedRows.Count == 0)
            {
                MessageBox.Show("Sélectionnez un livre à supprimer.", "Aucune sélection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce livre ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM Livres WHERE Id=@Id";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@Id", Convert.ToInt32(dgvLivres.SelectedRows[0].Cells["Id"].Value))
                };

                if (dbHelper.ExecuteNonQuery(query, parameters) > 0)
                {
                    MessageBox.Show("Livre supprimé avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ChargerLivres();
                    ReinitialiserFormulaire();
                }
            }
        }

        private void BtnRechercher_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM Livres WHERE Titre LIKE @Recherche OR Auteur LIKE @Recherche OR ISBN LIKE @Recherche ORDER BY Titre";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Recherche", "%" + txtRecherche.Text + "%")
            };
            dgvLivres.DataSource = dbHelper.ExecuteQuery(query, parameters);
        }

        private void BtnReinitialiser_Click(object sender, EventArgs e)
        {
            txtRecherche.Text = "";
            ChargerLivres();
        }

        private void DgvLivres_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLivres.Rows[e.RowIndex];
                txtTitre.Text = row.Cells["Titre"].Value?.ToString() ?? "";
                txtAuteur.Text = row.Cells["Auteur"].Value?.ToString() ?? "";
                txtISBN.Text = row.Cells["ISBN"].Value?.ToString() ?? "";
                txtAnnee.Text = row.Cells["AnneePublication"].Value?.ToString() ?? "";
                chkDisponible.Checked = Convert.ToBoolean(row.Cells["Disponible"].Value);
            }
        }

        private void ReinitialiserFormulaire()
        {
            txtTitre.Text = "";
            txtAuteur.Text = "";
            txtISBN.Text = "";
            txtAnnee.Text = "";
            chkDisponible.Checked = true;
        }
    }
}