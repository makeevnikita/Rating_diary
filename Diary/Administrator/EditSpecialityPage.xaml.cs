using Diary.Student;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diary.Administrator
{
    /// <summary>
    /// Логика взаимодействия для EditSpecialityPage.xaml
    /// </summary>
    public partial class EditSpecialityPage : Page
    {
        public static int selectedSpeciality { get; set; }
        public EditSpecialityPage(string speciality, string code, int id)
        {
            InitializeComponent();
            txtBoxSpeciality.Text = speciality;
            txtBoxCode.Text = code;
            selectedSpeciality = id;
        }

        private void ConfirmChanges(object sender, RoutedEventArgs e)
        {
            Validation validation = new Validation();

            if (txtBoxCode.Text == "" || txtBoxSpeciality.Text == "")
            {
                lblError.Content = "Поле не может быть пустым";
                lblError.Visibility = Visibility.Visible;
            }
            else if (validation.Integer(txtBoxCode.Text))
            {
                lblError.Content = "Только целочисленнй тип";
            }
            else
            {
                gr691_mnmEntities db = new gr691_mnmEntities();

                var speciality = db.SPECIALITY.Where(w => w.SPECIALITY_ID == selectedSpeciality).FirstOrDefault();
                speciality.SPECIALITY1 = txtBoxSpeciality.Text;
                speciality.CODE = Convert.ToInt32(txtBoxCode.Text);

                db.SaveChanges();

                lblError.Visibility = Visibility.Hidden;
                MainWindow.MainFrame.Frame.Navigate(new AdminStudentListPage());
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrame.Frame.GoBack();
        }
    }
}
