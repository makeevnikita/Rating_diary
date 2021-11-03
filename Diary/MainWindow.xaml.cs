using System.Windows;
using System.Windows.Controls;

namespace Diary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Frame = this.frame;
            MainFrame.Frame.Navigate(new AuthorizationPage());
        }

        public static class MainFrame
        {
            public static Frame Frame;
        }

        public static class Admin
        {
            public static string login = "admin";
            public static string password = "admin";
        }

        public static class UserInfo
        {
            public static int userId = 0;
            public static int role = 0;
            public static int groupId = 0;
        }
    }
}
