using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diary.Student
{
    /// <summary>
    /// Логика взаимодействия для EditStudentPage.xaml
    /// </summary>
    public partial class EditStudentPage : Page
    {
        public static string selectedGroup { get; set; }
        public static int selectedStudent { get; set; }
        public static string selectedOperation { get; set; }
        string oldLogin;

        public EditStudentPage(string Name, string Surname, string Patronymic, string Login,
                               string Password, object comboBoxSelectedIndex, int studID, string operation)
        {
            oldLogin = Login;

            selectedStudent = studID;
            InitializeComponent();
            selectedOperation = operation;
            txtBoxName.Text = Name;
            txtBoxSurname.Text = Surname;
            txtBoxPatr.Text = Patronymic;
            txtBoxLogin.Text = Login;
            txtBoxPassword.Text = Password;
            comboBoxGroup.SelectedItem = comboBoxSelectedIndex;

            gr691_mnmEntities db = new gr691_mnmEntities();
            var groups = from _group in db.GROUP
                         select new { gr = _group.GROUP1 };

            foreach (var item in groups)
            {
                comboBoxGroup.Items.Add(item.gr);
            }
        }

        private void ConfirmChanges(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();
            Validation validation = new Validation();

            if ((db.USER.Where(w => w.LOGIN == txtBoxLogin.Text && w.PASSWORD == txtBoxPassword.Text).FirstOrDefault() != null) && txtBoxLogin.Text != oldLogin)
            {
                lblError.Content = "Такой логин и пароль уже существует";
                lblError.Visibility = Visibility.Visible;
            }
            else if (txtBoxName.Text == "" || txtBoxSurname.Text == "" || txtBoxPatr.Text == "")
            {
                lblError.Content = "Поле не может быть пустым";
                lblError.Visibility = Visibility.Visible;
            }
            else if (validation.SpaceInWord(txtBoxPassword.Text) || validation.SpaceInWord(txtBoxLogin.Text))
            {
                lblError.Content = "Логин и пароль не могут содержать пробел";
                lblError.Visibility = Visibility.Visible;
            }
            else if (comboBoxGroup.SelectedItem == null)
            {
                lblError.Content = "Выберите группу";
                lblError.Visibility = Visibility.Visible;
            }
            else if (validation.SpecSymbol(txtBoxName.Text) || validation.SpecSymbol(txtBoxSurname.Text) || validation.SpecSymbol(txtBoxPatr.Text))
            {
                lblError.Content = "Символы !@#$%^&*()_+|}{:?>< недопустимы";
                lblError.Visibility = Visibility.Visible;
            }
            else
            {
                var gr = (from _group in db.GROUP
                          where _group.GROUP1 == selectedGroup
                          select new
                          {
                              grId = _group.GROUP_ID,
                              groupNumber = _group.GROUP1
                          }).FirstOrDefault();
                if (selectedOperation == "edit") //EDIT
                {
                    var updateStudent = db.USER.Where(w => w.USER_ID == selectedStudent).FirstOrDefault();

                    updateStudent.SURNAME = txtBoxSurname.Text;
                    updateStudent.NAME = txtBoxName.Text;
                    updateStudent.PATRONYMIC = txtBoxPatr.Text;
                    updateStudent.LOGIN = txtBoxLogin.Text;
                    updateStudent.PASSWORD = txtBoxPassword.Text;
                    updateStudent.FK_GROUP_ID = gr.grId;

                    db.SaveChanges();

                    AdminStudentListPage.selectedGroup = comboBoxGroup.SelectedItem.ToString();
                    MainWindow.MainFrame.Frame.Navigate(new AdminStudentListPage());
                }
                else //ADD
                {
                    USER student = new USER();
                    student.SURNAME = txtBoxSurname.Text;
                    student.NAME = txtBoxName.Text;
                    student.PATRONYMIC = txtBoxPatr.Text;
                    student.LOGIN = txtBoxLogin.Text;
                    student.PASSWORD = txtBoxPassword.Text;
                    student.FK_GROUP_ID = gr.grId;
                    student.FK_ROLE_ID = 3;

                    db.USER.Add(student);
                    db.SaveChanges();

                    AdminStudentListPage.selectedGroup = comboBoxGroup.SelectedItem.ToString();
                    MainWindow.MainFrame.Frame.Navigate(new AdminStudentListPage());
                }

            }
        }

        private void comboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedGroup = comboBoxGroup.SelectedItem.ToString();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrame.Frame.GoBack();
        }
    }
}
