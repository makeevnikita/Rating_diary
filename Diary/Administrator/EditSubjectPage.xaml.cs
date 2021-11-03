using Diary.Student;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diary.Administrator
{
    /// <summary>
    /// Логика взаимодействия для EditSubjectPage.xaml
    /// </summary>
    public partial class EditSubjectPage : Page
    {
        public static Dictionary<int, string> Teachers = new Dictionary<int, string>();
        public static int selectedTeacher { get; set; }
        public static int subjectId { get; set; }
        public EditSubjectPage(string subject, int id)
        {
            Teachers.Clear();
            subjectId = id;
            InitializeComponent();
            txtSubject.Text = subject;

            gr691_mnmEntities db = new gr691_mnmEntities();

            foreach (var teacher in db.USER.Where(w => w.FK_ROLE_ID == 2))
            {
                comboBoxTeacher.Items.Add(teacher.SURNAME + " " + teacher.NAME + " " + teacher.PATRONYMIC);
                Teachers.Add(teacher.USER_ID, teacher.SURNAME + " " + teacher.NAME + " " + teacher.PATRONYMIC);
            }
            comboBoxTeacher.Items.Add("Нет преподавателя");
        }

        private void comboBoxTeacher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var teacher in Teachers)
            {
                if (teacher.Value == comboBoxTeacher.SelectedItem.ToString())
                {
                    selectedTeacher = teacher.Key;
                }
            }
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (txtSubject.Text == "")
            {
                lblError.Content = "Поле не может быть пустым";
                lblError.Visibility = Visibility.Visible;
            }
            else
            {
                gr691_mnmEntities db = new gr691_mnmEntities();
                var subject = db.SUBJECT.Where(w => w.SUBJECT_ID == subjectId).FirstOrDefault();
                subject.NAME = txtSubject.Text;

                if (comboBoxTeacher.SelectedItem == null || comboBoxTeacher.SelectedItem.ToString() == "Нет преподавателя")
                {
                    subject.FK_TEACHER_ID = null;
                }
                else
                {
                    subject.FK_TEACHER_ID = selectedTeacher;
                }

                db.SaveChanges();
                Teachers.Clear();
                MainWindow.MainFrame.Frame.Navigate(new AdminStudentListPage());
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrame.Frame.GoBack();
        }
    }
}
