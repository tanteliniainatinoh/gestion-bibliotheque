using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Data;

namespace GestionBibliotheque.Forms
{
    public partial class UtilisateursForm : UserControl
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        private DataGridView dgvUtilisateurs;
        private Label lblNom, lblEmail, lblTelephone;
        private TextBox txtNom, txtEmail, txtTelephone;
        private Button btnAjouter, btnModifier, btnSupprimer, btnReinitialiser;
        private Panel panelInput;

        public UtilisateursForm()
        {
            InitializeComponent();
            ChargerUtilisateurs();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // DataGridView
            dgvUtilisateurs = new DataGridView
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
            this.Controls.Add(dgvUtilisateurs);

            // Panel pour les champs de saisie
            panelInput = new Panel
            {
                Location = new Point(15, 270),
                Size = new Size(970, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(panelInput);

            // Labels et TextBox
            lblNom = new Label { Text = "Nom :", Location = new Point(10, 20), Size = new Size(80, 25), Font = new Font("Segoe UI", 12) };
            txtNom = new TextBox { Location = new Point(100, 20), Size = new Size(250, 30), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };

            lblEmail = new Label { Text = "Email :", Location = new Point(10, 60), Size = new Size(80, 25), Font = new Font("Segoe UI", 12) };
            txtEmail = new TextBox { Location = new Point(100, 60), Size = new Size(250, 30), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };

            lblTelephone = new Label { Text = "Téléphone :", Location = new Point(10, 100), Size = new Size(100, 25), Font = new Font("Segoe UI", 12) };
            txtTelephone = new TextBox { Location = new Point(120, 100), Size = new Size(200, 30), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };

            panelInput.Controls.AddRange(new Control[] {
                lblNom, txtNom, lblEmail, txtEmail, lblTelephone, txtTelephone
            });

            // Boutons
            btnAjouter = new Button
            {
                Text = "➕ Ajouter",
                Location = new Point(15, 430),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(50, 205, 50),
                ForeColor = Color.White
            };
            btnModifier = new Button
            {
                Text = "✏️ Modifier",
                Location = new Point(145, 430),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(30, 144, 255),
                ForeColor = Color.White
            };
            btnSupprimer = new Button
            {
                Text = "🗑️ Supprimer",
                Location = new Point(275, 430),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(220, 20, 60),
                ForeColor = Color.White
            };
            btnReinitialiser = new Button
            {
                Text = "🔄 Réinitialiser",
                Location = new Point(405, 430),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(128, 128, 128),
                ForeColor = Color.White
            };

            this.Controls.AddRange(new Control[] {
                btnAjouter, btnModifier, btnSupprimer, btnReinitialiser
            });

            // Événements
            btnAjouter.Click += BtnAjouter_Click;
            btnModifier.Click += BtnModifier_Click;
            btnSupprimer.Click += BtnSupprimer_Click;
            btnReinitialiser.Click += BtnReinitialiser_Click;
            dgvUtilisateurs.CellClick += DgvUtilisateurs_CellClick;

            this.Size = new Size(1000, 500);
            this.ResumeLayout(false);
        }

        private void ChargerUtilisateurs()
        {
            string query = "SELECT * FROM Utilisateurs ORDER BY Nom";
            dgvUtilisateurs.DataSource = dbHelper.ExecuteQuery(query);
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Veuillez remplir le nom et l'email.", "Champs requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO Utilisateurs (Nom, Email, Telephone) VALUES (@Nom, @Email, @Telephone)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Nom", txtNom.Text),
                new MySqlParameter("@Email", txtEmail.Text),
                new MySqlParameter("@Telephone", txtTelephone.Text)
            };

            if (dbHelper.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Utilisateur ajouté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerUtilisateurs();
                ReinitialiserFormulaire();
            }
        }

        private void BtnModifier_Click(object sender, EventArgs e)
        {
            if (dgvUtilisateurs.SelectedRows.Count == 0)
            {
                MessageBox.Show("Sélectionnez un utilisateur à modifier.", "Aucune sélection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "UPDATE Utilisateurs SET Nom=@Nom, Email=@Email, Telephone=@Telephone WHERE Id=@Id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Nom", txtNom.Text),
                new MySqlParameter("@Email", txtEmail.Text),
                new MySqlParameter("@Telephone", txtTelephone.Text),
                new MySqlParameter("@Id", Convert.ToInt32(dgvUtilisateurs.SelectedRows[0].Cells["Id"].Value))
            };

            if (dbHelper.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Utilisateur modifié avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerUtilisateurs();
            }
        }

        private void BtnSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvUtilisateurs.SelectedRows.Count == 0)
            {
                MessageBox.Show("Sélectionnez un utilisateur à supprimer.", "Aucune sélection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet utilisateur ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM Utilisateurs WHERE Id=@Id";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@Id", Convert.ToInt32(dgvUtilisateurs.SelectedRows[0].Cells["Id"].Value))
                };

                if (dbHelper.ExecuteNonQuery(query, parameters) > 0)
                {
                    MessageBox.Show("Utilisateur supprimé avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ChargerUtilisateurs();
                    ReinitialiserFormulaire();
                }
            }
        }

        private void BtnReinitialiser_Click(object sender, EventArgs e)
        {
            ReinitialiserFormulaire();
        }

        private void DgvUtilisateurs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUtilisateurs.Rows[e.RowIndex];
                txtNom.Text = row.Cells["Nom"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                txtTelephone.Text = row.Cells["Telephone"].Value?.ToString() ?? "";
            }
        }

        private void ReinitialiserFormulaire()
        {
            txtNom.Text = "";
            txtEmail.Text = "";
            txtTelephone.Text = "";
        }
    }
}