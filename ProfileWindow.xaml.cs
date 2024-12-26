using System.Windows;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;
using System;

namespace ConverterApp
{
    public partial class ProfileWindow : Window
    {
        private UserProfile currentUser;
        private readonly string profilePath = @"C:\Users\DMRTX\source\repos\WpfApp1\bin\Debug";

        public ProfileWindow(UserProfile user)
        {
            InitializeComponent();
            currentUser = user;
            LoadProfile();
            SetupEventHandlers();

            
            if (!Directory.Exists(profilePath))
            {
                Directory.CreateDirectory(profilePath);
            }
        }

        private void SetupEventHandlers()
        {
            btnSaveProfile.Click += (s, e) => SaveUserProfile();
            btnLoadProfile.Click += (s, e) => LoadUserProfile();
        }

        private void LoadProfile()
        {
            txtUserName.Text = currentUser.Name;
            txtUserEmail.Text = currentUser.Email;
        }

        private void SaveUserProfile()
        {
            try
            {
                currentUser.Name = txtUserName.Text;
                currentUser.Email = txtUserEmail.Text;

                var json = JsonConvert.SerializeObject(currentUser);
                string profileFilePath = Path.Combine(profilePath, $"profile_{currentUser.Name}.json");
                string lastProfilePath = Path.Combine(profilePath, "last_profile.txt");

                File.WriteAllText(profileFilePath, json);
                File.WriteAllText(lastProfilePath, currentUser.Name);
                MessageBox.Show("Профиль успешно сохранен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении профиля: {ex.Message}");
            }
        }

        private void LoadUserProfile()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = profilePath,
                Filter = "JSON files (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var json = File.ReadAllText(openFileDialog.FileName);
                    currentUser = JsonConvert.DeserializeObject<UserProfile>(json);
                    LoadProfile();
                    MessageBox.Show("Профиль успешно загружен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке профиля: {ex.Message}");
                }
            }
        }
    }
}