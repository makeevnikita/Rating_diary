using Diary.Student;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Diary
{
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void SignIn_Button(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            if (txtLogin.Text == "")
            {
                lblError.Content = "Заполните поля";
                txtLogin.BorderBrush = Brushes.Red;
                lblError.Visibility = Visibility.Visible;
                return;
            }
            if (txtPassword.Password == "")
            {
                lblError.Content = "Заполните поля";
                txtPassword.BorderBrush = Brushes.Red;
                lblError.Visibility = Visibility.Visible;
                return;
            }

            var user = db.USER.Where(w => w.LOGIN == txtLogin.Text && w.PASSWORD == txtPassword.Password).FirstOrDefault();

            if (user != null)
            {
                MainWindow.UserInfo.role = (int)user.FK_ROLE_ID;

                if ((int)user.FK_ROLE_ID == 1)
                {
                    MainWindow.UserInfo.userId = user.USER_ID;
                }
                if ((int)user.FK_ROLE_ID == 2)
                {
                    MainWindow.UserInfo.userId = user.USER_ID;
                }
                if ((int)user.FK_ROLE_ID == 3)
                {
                    MainWindow.UserInfo.userId = user.USER_ID;
                    MainWindow.UserInfo.groupId = (int)user.FK_GROUP_ID;
                }
                MainWindow.MainFrame.Frame.Navigate(new AdminStudentListPage());
            }
            else
            {
                lblError.Content = "Неверный логин или пароль";
                lblError.Visibility = Visibility.Visible;
                txtLogin.BorderBrush = Brushes.Gray;
                txtPassword.BorderBrush = Brushes.Gray;
            }
        }
    }
}
