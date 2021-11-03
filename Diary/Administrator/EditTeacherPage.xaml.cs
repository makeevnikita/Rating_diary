using Diary.Student;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diary.Administrator
{
    /// <summary>
    /// Логика взаимодействия для EditTeacherPage.xaml
    /// </summary>
    public partial class EditTeacherPage : Page
    {
        public static int selectedTeacher { get; set; }
        public static string selectedOperation { get; set; }
        public EditTeacherPage(string Name, string Surname, string Patronymic, string Login,
                               string Password, int teacherId, string operation)
        {
            selectedTeacher = teacherId;
            InitializeComponent();

            selectedOperation = operation;
            txtBoxName.Text = Name;
            txtBoxSurname.Text = Surname;
            txtBoxPatr.Text = Patronymic;
            txtBoxLogin.Text = Login;
            txtBoxPassword.Text = Password;
        }

        private void ConfirmChanges(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();
            Validation validation = new Validation();

            if (db.USER.Where(w => w.LOGIN == txtBoxLogin.Text && w.PASSWORD == txtBoxPassword.Text).FirstOrDefault() != null)
            {
                lblError.Content = "Такой логин и пароль уже существует";
                lblError.Visibility = Visibility.Visible;
            }
            else if (txtBoxName.Text == "" ||
                     txtBoxSurname.Text == "" ||
                     txtBoxPatr.Text == "")
            {
                lblError.Content = "Поле не может быть пустым";
                lblError.Visibility = Visibility.Visible;
            }
            else if (validation.SpaceInWord(txtBoxPassword.Text) || validation.SpaceInWord(txtBoxLogin.Text))
            {
                lblError.Content = "Логин и пароль не могут содержать пробел";
                lblError.Visibility = Visibility.Visible;
            }
            else
            {
                if (selectedOperation == "edit")
                {
                    var updateTeacher = db.USER.Where(w => w.USER_ID == selectedTeacher).FirstOrDefault();

                    updateTeacher.SURNAME = txtBoxSurname.Text;
                    updateTeacher.NAME = txtBoxName.Text;
                    updateTeacher.PATRONYMIC = txtBoxPatr.Text;
                    updateTeacher.LOGIN = txtBoxLogin.Text;
                    updateTeacher.PASSWORD = txtBoxPassword.Text;

                    db.SaveChanges();
                    MainWindow.MainFrame.Frame.Navigate(new AdminStudentListPage());
                }
                else
                {
                    USER newTeacher = new USER();
                    newTeacher.SURNAME = txtBoxSurname.Text;
                    newTeacher.NAME = txtBoxName.Text;
                    newTeacher.PATRONYMIC = txtBoxPatr.Text;
                    newTeacher.LOGIN = txtBoxLogin.Text;
                    newTeacher.PASSWORD = txtBoxPassword.Text;
                    newTeacher.FK_ROLE_ID = 2;

                    db.USER.Add(newTeacher);
                    db.SaveChanges();
                    MainWindow.MainFrame.Frame.Navigate(new AdminStudentListPage());
                }

            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrame.Frame.GoBack();
        }
    }
}
