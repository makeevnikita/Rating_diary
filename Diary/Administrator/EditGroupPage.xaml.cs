using Diary.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diary.Administrator
{
    /// <summary>
    /// Логика взаимодействия для EditGroupPage.xaml
    /// </summary>
    public partial class EditGroupPage : Page
    {
        public static string selectedCourse { get; set; }
        public static string selectedSpeciality { get; set; }
        public static string selectedFaculty { get; set; }
        public static string selectedOperation { get; set; }
        public static string selectedTeacher { get; set; }

        public static int selectedGroup { get; set; }

        public static Dictionary<int, string> Teachers = new Dictionary<int, string>();

        public EditGroupPage(string number, int idGroup, string operation)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            Teachers.Clear();
            InitializeComponent();

            selectedOperation = operation;

            txtNumber.Text = number;
            selectedGroup = idGroup;

            foreach (var course in db.COURSE)
            {
                comboBoxCourse.Items.Add(course.COURSE_ID);
            }

            foreach (var speciality in db.SPECIALITY)
            {
                comboBoxSpeciality.Items.Add(speciality.SPECIALITY1);
            }

            foreach (var faculty in db.FACULTY)
            {
                comboBoxFaculty.Items.Add(faculty.FACULTY1);
            }

            foreach (var teacher in db.USER.Where(w => w.FK_ROLE_ID == 2))
            {
                comboBoxTeacher.Items.Add(teacher.SURNAME + " " + teacher.NAME + " " + teacher.PATRONYMIC);
                Teachers.Add(teacher.USER_ID, teacher.SURNAME + " " + teacher.NAME + " " + teacher.PATRONYMIC);
            }

            ShowSubject();
        }

        private void comboBoxGroup_SelectionChangedCourse(object sender, SelectionChangedEventArgs e)
        {
            selectedCourse = comboBoxCourse.SelectedItem.ToString();
        }

        private void comboBoxGroup_SelectionChangedSpeciality(object sender, SelectionChangedEventArgs e)
        {
            selectedSpeciality = comboBoxSpeciality.SelectedItem.ToString();
        }

        private void comboBoxGroup_SelectionChangedFaculty(object sender, SelectionChangedEventArgs e)
        {
            selectedFaculty = comboBoxFaculty.SelectedItem.ToString();
        }

        private void comboBoxGroup_SelectionChangedTeacher(object sender, SelectionChangedEventArgs e)
        {
            selectedTeacher = comboBoxTeacher.SelectedItem.ToString();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (txtNumber.Text == "")
            {
                lblError.Content = "Поле не может быть пустым";
                lblError.Visibility = Visibility.Visible;
            }
            else
            {
                int id = 0;
                foreach (var teacher in Teachers)
                {
                    if (teacher.Value == selectedTeacher)
                    {
                        id = teacher.Key;
                        break;
                    }
                }

                gr691_mnmEntities db = new gr691_mnmEntities();

                if (selectedOperation == "add")
                {
                    GROUP group = new GROUP
                    {
                        GROUP1 = txtNumber.Text,
                        FK_COURSE_ID = Convert.ToInt32(selectedCourse),
                        FK_SPECIALITY_ID = db.SPECIALITY.Where(w => w.SPECIALITY1 == selectedSpeciality).FirstOrDefault().SPECIALITY_ID,
                        FK_FACULTY_ID = db.FACULTY.Where(w => w.FACULTY1 == selectedFaculty).FirstOrDefault().FACULTY_ID,
                        FK_TEACHER_ID = db.USER.Where(w => w.USER_ID == id).FirstOrDefault().USER_ID
                    };

                    db.GROUP.Add(group);

                    foreach (int subjectId in subjectGroup)
                    {

                        SUBJECT_GROUP subjectGroup = new SUBJECT_GROUP
                        {
                            FK_GROUP_ID = selectedGroup,
                            FK_SUBJECT_ID = subjectId
                        };

                        db.SUBJECT_GROUP.Add(subjectGroup);
                    }
                }
                else
                {
                    foreach (var item in db.SUBJECT_GROUP.Where(w => w.FK_GROUP_ID == selectedGroup))
                    {
                        db.SUBJECT_GROUP.Remove(item);
                    }

                    foreach (int subjectId in subjectGroup)
                    {

                        SUBJECT_GROUP subjectGroup = new SUBJECT_GROUP
                        {
                            FK_GROUP_ID = selectedGroup,
                            FK_SUBJECT_ID = subjectId
                        };

                        db.SUBJECT_GROUP.Add(subjectGroup);
                    }

                    var group = db.GROUP.Where(w => w.GROUP_ID == selectedGroup).FirstOrDefault();

                    if (selectedSpeciality != null)
                    {
                        group.FK_SPECIALITY_ID = db.SPECIALITY.Where(w => w.SPECIALITY1 == selectedSpeciality).FirstOrDefault().SPECIALITY_ID;
                    }
                    if (selectedFaculty != null)
                    {
                        group.FK_FACULTY_ID = db.FACULTY.Where(w => w.FACULTY1 == selectedFaculty).FirstOrDefault().FACULTY_ID;
                    }
                    if (selectedCourse != null)
                    {
                        group.FK_COURSE_ID = Convert.ToInt32(selectedCourse);
                    }
                    if (txtNumber.Text == null)
                    {
                        lblError.Content = "Номер группы не может быть пустмы";
                        lblError.Visibility = Visibility.Visible;
                        return;
                    }
                    else
                    {
                        group.GROUP1 = txtNumber.Text;
                    }
                    if (id != 0)
                    {
                        group.FK_TEACHER_ID = db.USER.Where(w => w.USER_ID == id).FirstOrDefault().USER_ID;
                    }
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

        List<int> list = new List<int>();

        public void ShowSubject()
        {
            stackPanelGroup1.Children.Clear();
            stackPanelGroup2.Children.Clear();

            gr691_mnmEntities db = new gr691_mnmEntities();

            var groupSubject = from subjectGroup in db.SUBJECT_GROUP
                               where subjectGroup.FK_GROUP_ID == selectedGroup
                               select new
                               {
                                   id = subjectGroup.FK_SUBJECT_ID
                               };

            if (groupSubject != null)
            {
                foreach (var subject in groupSubject)
                {
                    list.Add(subject.id);
                    subjectGroup.Add(subject.id);
                }

                foreach (int id in list)
                {
                    Button btn = new Button();
                    btn.Tag = id;
                    btn.Margin = new Thickness(0, 5, 0, 0);
                    btn.Content = db.SUBJECT.Where(w => w.SUBJECT_ID == id).FirstOrDefault().NAME;
                    btn.Click += Btn_Click;

                    stackPanelGroup1.Children.Add(btn);
                }
            }


            int noSubject = db.SUBJECT.Where(w => w.NAME == "Нет урока").FirstOrDefault().SUBJECT_ID;

            foreach (var subject in db.SUBJECT.Where(w => w.SUBJECT_ID != noSubject))
            {
                if (!list.Contains(Convert.ToInt32(subject.SUBJECT_ID)))
                {
                    Button btn = new Button();
                    btn.Tag = subject.SUBJECT_ID;
                    btn.Margin = new Thickness(0, 5, 0, 0);
                    btn.Content = db.SUBJECT.Where(w => w.SUBJECT_ID == subject.SUBJECT_ID).FirstOrDefault().NAME;
                    btn.Click += Btn_Click;

                    stackPanelGroup2.Children.Add(btn);
                }
            }
        }

        List<int> subjectGroup = new List<int>();
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            list.Clear();

            Button btn = (Button)sender;

            if (stackPanelGroup1.Children.Contains(btn))
            {
                stackPanelGroup1.Children.Remove(btn);
                stackPanelGroup2.Children.Add(btn);

                subjectGroup.Remove(Convert.ToInt32(btn.Tag));
            }
            else
            {
                stackPanelGroup2.Children.Remove(btn);
                stackPanelGroup1.Children.Add(btn);

                subjectGroup.Add(Convert.ToInt32(btn.Tag));
            }
        }
    }
}
