using Diary.Administrator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Diary.Student
{
    public partial class AdminStudentListPage : Page
    {
        public static int tabControlIndex { get; set; }

        public static string AvgMark(int fkSubject, int userId, DateTime date1, DateTime date2)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            var avgMark = from mark in db.MARK
                          where mark.FK_STUDENT_ID == userId
                          &&
                          mark.FK_SUBJECT_ID == fkSubject
                          &&
                          mark.MARK1 != "H" && mark.MARK1 != "Н" && mark.MARK1 != ""
                          &&
                          mark.DATE >= date1 && mark.DATE <= date2
                          select new
                          {
                              avg = mark.MARK1
                          };

            double avg = 0;
            foreach (var mark in avgMark)
            {
                avg += Convert.ToDouble(mark.avg);
            }
            avg /= avgMark.Count();

            if (Regex.IsMatch(avg.ToString(), @"\d"))
            {
                if (avg.ToString().Length > 3)
                {
                    return avg.ToString().Substring(0, 3);
                }
                else
                {
                    return avg.ToString();
                }
            }

            return "";
        }

        public static string SkipMark(int fkSubject, int userId)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            return db.MARK.Where(w => w.FK_STUDENT_ID == userId && w.FK_SUBJECT_ID == fkSubject && (w.MARK1 == "H" ||w.MARK1 == "Н")).Count().ToString();
        }

        public void ShowMarkToStudent()
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            stackPanelMarkStudent.Children.Clear();
            stackPanelMark.Children.Clear();
            gridMark.Children.Remove(stackPanelMark);
            stackPanelDate.Children.Clear();

            var subjects = from subject in db.SUBJECT
                           join subjectGroup in db.SUBJECT_GROUP on subject.SUBJECT_ID equals subjectGroup.FK_SUBJECT_ID
                           join _group in db.GROUP on subjectGroup.FK_GROUP_ID equals _group.GROUP_ID
                           where _group.GROUP_ID == MainWindow.UserInfo.groupId
                           select new
                           {
                               subject = subject.NAME
                           };

            foreach (var subject in subjects)
            {
                Label lblSubject = new Label();
                lblSubject.Content = subject.subject;
                lblSubject.FontSize = 12;
                lblSubject.Height = 45;
                lblSubject.Background = Brushes.WhiteSmoke;
                lblSubject.Margin = new Thickness(0, 0, 0, 5);
                stackPanelMarkStudent.Children.Add(lblSubject);
            }

            DateTime date2 = dp1.SelectedDate.Value.AddDays(6);

            var markDate = (from mark in db.MARK
                            where
                            mark.FK_STUDENT_ID == MainWindow.UserInfo.userId
                            &&
                            mark.DATE >= dp1.SelectedDate.Value && mark.DATE <= date2
                            select new
                            {
                                markId = mark.MARK_ID,
                                date = mark.DATE,
                                mark = mark.MARK1,
                                subjectId = mark.FK_SUBJECT_ID
                            });

            foreach (var date in markDate.Distinct().GroupBy(g => g.date))
            {
                Label dateMark = new Label();
                dateMark.Width = 100;
                dateMark.Background = Brushes.WhiteSmoke;
                dateMark.Content = date.Key.Value.ToLongDateString();
                dateMark.Margin = new Thickness(0, 0, 5, 0);

                stackPanelDate.Children.Add(dateMark);
            }

            stackPanelMark.Children.Add(stackPanelDate);

            ObservableCollection<StackPanel> panels = new ObservableCollection<StackPanel>();
            foreach (var subject in subjects)
            {
                var subjectId = db.SUBJECT.Where(w => w.NAME == subject.subject).FirstOrDefault().SUBJECT_ID;

                var marks = (from mark1 in db.MARK
                             join student in db.USER on mark1.FK_STUDENT_ID equals student.USER_ID
                             where
                             mark1.FK_SUBJECT_ID == subjectId
                             &&
                             mark1.FK_STUDENT_ID == MainWindow.UserInfo.userId
                             select new
                             {
                                 mark = mark1.MARK1
                             });

                StackPanel stack = new StackPanel();
                stack.Tag = subjectId;
                stack.Orientation = Orientation.Horizontal;
                stack.Height = 45;
                stack.Margin = new Thickness(0, 0, 0, 5);

                foreach (var date in markDate.GroupBy(w => w.date))//Оценки
                {
                    var mark = db.MARK.Where(w => w.DATE == date.Key.Value &&
                    w.FK_STUDENT_ID == MainWindow.UserInfo.userId && w.FK_SUBJECT_ID == subjectId).FirstOrDefault();

                    Label lblMark = new Label();
                    lblMark.VerticalContentAlignment = VerticalAlignment.Center;
                    lblMark.HorizontalContentAlignment = HorizontalAlignment.Center;
                    lblMark.FontSize = 16;
                    lblMark.Height = 50;
                    lblMark.Width = 100;
                    lblMark.ToolTip = date.Key.Value.ToLongDateString() + '\n' + subject.subject;
                    lblMark.Background = Brushes.SkyBlue;
                    lblMark.Margin = new Thickness(0, 0, 5, 0);

                    if (mark != null)
                    {
                        lblMark.Content = mark.MARK1;
                    }
                    stack.Children.Add(lblMark);
                }
                stackPanelMark.Children.Add(stack);

                panels.Add(stack);
            }

            Label skipMark = new Label();
            skipMark.Width = 110;
            skipMark.Background = Brushes.WhiteSmoke;
            skipMark.Content = "Пропуски";
            skipMark.Margin = new Thickness(0, 0, 5, 0);
            skipMark.HorizontalContentAlignment = HorizontalAlignment.Center;

            stackPanelDate.Children.Add(skipMark);

            Label semestr1 = new Label();
            semestr1.Width = 110;
            semestr1.Background = Brushes.WhiteSmoke;
            semestr1.Content = "Семестр 1";
            semestr1.Margin = new Thickness(0, 0, 5, 0);
            semestr1.HorizontalContentAlignment = HorizontalAlignment.Center;

            stackPanelDate.Children.Add(semestr1);

            Label semestr2 = new Label();
            semestr2.Width = 110;
            semestr2.Background = Brushes.WhiteSmoke;
            semestr2.Content = "Семестр 2";
            semestr2.Margin = new Thickness(0, 0, 5, 0);
            semestr2.HorizontalContentAlignment = HorizontalAlignment.Center;

            stackPanelDate.Children.Add(semestr2);

            Label lblFinalMark = new Label();
            lblFinalMark.Width = 110;
            lblFinalMark.Background = Brushes.WhiteSmoke;
            lblFinalMark.Content = "Годовая оценка";
            lblFinalMark.Margin = new Thickness(0, 0, 5, 0);
            lblFinalMark.HorizontalContentAlignment = HorizontalAlignment.Center;

            stackPanelDate.Children.Add(lblFinalMark);

            foreach (var stack in panels)
            {
                int id = Convert.ToInt32(stack.Tag);

                Label skip = new Label();//Количество пропусков
                skip.FontSize = 16;
                skip.VerticalContentAlignment = VerticalAlignment.Center;
                skip.HorizontalContentAlignment = HorizontalAlignment.Center;
                skip.Height = 45;
                skip.Width = 110;
                skip.Margin = new Thickness(0, 0, 5, 0);
                skip.HorizontalContentAlignment = HorizontalAlignment.Center;
                skip.Background = Brushes.SkyBlue;
                skip.Content = SkipMark(id, MainWindow.UserInfo.userId);

                stack.Children.Add(skip);

                double yearMark = 0;

                Label sem1 = new Label();//Первый семестр
                sem1.FontSize = 16;
                sem1.VerticalContentAlignment = VerticalAlignment.Center;
                sem1.HorizontalContentAlignment = HorizontalAlignment.Center;
                sem1.Height = 45;
                sem1.Width = 110;
                sem1.Margin = new Thickness(0, 0, 5, 0);
                sem1.HorizontalContentAlignment = HorizontalAlignment.Center;
                sem1.Background = Brushes.SkyBlue;

                if (DateTime.Today.Month >= 9 && DateTime.Today.Month <= 12)
                {
                    sem1.Content = AvgMark(id, MainWindow.UserInfo.userId, new DateTime(DateTime.Today.Year, 9, 1), new DateTime(DateTime.Today.Year, 12, 31));
                }
                else if (DateTime.Today.Month >= 1 && DateTime.Today.Month <= 6)
                {
                    sem1.Content = AvgMark(id, MainWindow.UserInfo.userId, new DateTime(DateTime.Today.Year - 1, 9, 1), new DateTime(DateTime.Today.Year - 1, 12, 31));
                }
                if (sem1.Content != "")
                {
                    yearMark += Convert.ToDouble(sem1.Content);
                }

                stack.Children.Add(sem1);

                Label sem2 = new Label();//Второй семестр
                sem2.FontSize = 16;
                sem2.VerticalContentAlignment = VerticalAlignment.Center;
                sem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                sem2.Height = 45;
                sem2.Width = 110;
                sem2.Margin = new Thickness(0, 0, 5, 0);
                sem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                sem2.Background = Brushes.SkyBlue;

                if (DateTime.Today.Month >= 1 && DateTime.Today.Month <= 6)
                {
                    sem2.Content = AvgMark(id, MainWindow.UserInfo.userId, new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 6, 30));
                }
                if (sem2.Content != "")
                {
                    yearMark += Convert.ToDouble(sem2.Content);
                }

                stack.Children.Add(sem2);

                Label label = new Label();//Годовая
                label.FontSize = 16;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.Height = 45;
                label.Width = 110;
                label.Margin = new Thickness(0, 0, 10, 0);
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.Background = Brushes.SkyBlue;

                if (sem1.Content != "" & sem2.Content != "")
                {
                    label.Content = (yearMark / 2).ToString();
                    if (label.Content.ToString().Length > 3)
                    {
                        label.Content = (label.Content).ToString().Substring(0, 3);
                    }
                }
                

                
                stack.Children.Add(label);
            }

            stackPanelMark.Margin = new Thickness(100, 0, 0, 0);
            gridMark.Children.Add(stackPanelMark);
        }
        public AdminStudentListPage()
        {
            InitializeComponent();
            dp1.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // 1 - admin, 2 - teacher, 3 - student
            gr691_mnmEntities db = new gr691_mnmEntities();

            comboboxGroup.Items.Clear();

            if (MainWindow.UserInfo.role == 1) //ADMIN
            {
                tabItemMarks.Visibility = Visibility.Hidden;
                mainTabControl.SelectedIndex = 1;

                foreach (var item in db.GROUP)
                {
                    comboboxGroup.Items.Add(item.GROUP1);
                }

                if (tabControlIndex != 0)
                {
                    mainTabControl.SelectedIndex = tabControlIndex;
                }

                if (db.SUBJECT.Where(w => w.NAME == "Нет урока").FirstOrDefault() == null)
                {
                    SUBJECT subject = new SUBJECT();
                    subject.NAME = "Нет урока";
                    subject.FK_TEACHER_ID = null;

                    db.SUBJECT.Add(subject);
                    db.SaveChanges();
                }

                ShowStudentList(comboboxGroup, null);
                ShowGroupList(null, null);
                ShowTeacherList(null, null);
                ShowSpecialityList(null, null);
                ShowFacultyList(null, null);
                ShowSubjectList(null, null);
            }
            if (MainWindow.UserInfo.role == 2)
            {
                if (db.SUBJECT.Where(w => w.NAME == "Нет урока").FirstOrDefault() == null)
                {
                    SUBJECT subject = new SUBJECT();
                    subject.NAME = "Нет урока";
                    subject.FK_TEACHER_ID = null;

                    db.SUBJECT.Add(subject);
                    db.SaveChanges();
                }

                var query = (from subject in db.SUBJECT
                             join teacher in db.USER on subject.FK_TEACHER_ID equals teacher.USER_ID
                             join subjectGroup in db.SUBJECT_GROUP on subject.SUBJECT_ID equals subjectGroup.FK_SUBJECT_ID
                             join _group in db.GROUP on subjectGroup.FK_GROUP_ID equals _group.GROUP_ID
                             where teacher.USER_ID == MainWindow.UserInfo.userId
                             select new
                             {
                                 _group = _group.GROUP1
                             }).Distinct();

                var queryGroup = (from subject in db.SUBJECT
                                  join teacher in db.USER on subject.FK_TEACHER_ID equals teacher.USER_ID
                                  join subjectGroup in db.SUBJECT_GROUP on subject.SUBJECT_ID equals subjectGroup.FK_SUBJECT_ID
                                  join _group in db.GROUP on subjectGroup.FK_GROUP_ID equals _group.GROUP_ID
                                  where teacher.USER_ID == MainWindow.UserInfo.userId
                                  select new
                                  {
                                      _group = _group.GROUP1
                                  }).Distinct();

                foreach (var item in query)
                {
                    markGroup.Items.Add(item._group);
                }

                tabItemStudent.Visibility = Visibility.Hidden;
                tabItemGroup.Visibility = Visibility.Hidden;
                tabItemTeacher.Visibility = Visibility.Hidden;
                tabItemSpeciality.Visibility = Visibility.Hidden;
                tabItemFaculty.Visibility = Visibility.Hidden;
                tabItemSubject.Visibility = Visibility.Hidden;
            }
            if (MainWindow.UserInfo.role == 3)
            {
                lblSubject.Visibility = Visibility.Hidden;
                markGroup.Visibility = Visibility.Hidden;
                lblGroup.Visibility = Visibility.Hidden;
                SaveMarkbtn.Visibility = Visibility.Hidden;
                tabItemStudent.Visibility = Visibility.Hidden;
                tabItemGroup.Visibility = Visibility.Hidden;
                tabItemTeacher.Visibility = Visibility.Hidden;
                tabItemSpeciality.Visibility = Visibility.Hidden;
                tabItemFaculty.Visibility = Visibility.Hidden;
                tabItemSubject.Visibility = Visibility.Hidden;
                markSubject.Visibility = Visibility.Hidden;

                stackPanelDate.Orientation = Orientation.Horizontal;
                stackPanelDate.Height = 60;
                stackPanelDate.Children.Clear();
                stackPanelDate.VerticalAlignment = VerticalAlignment.Top;

                ShowMarkToStudent();
            }
        }

        public static Dictionary<int, string> Teachers = new Dictionary<int, string>();

        public static List<int> Student_id = new List<int>();
        public static List<int> Group_id = new List<int>();
        public static List<int> Teacher_id = new List<int>();
        public static List<int> Speciality_id = new List<int>();
        public static List<int> Faculty_id = new List<int>();
        public static List<int> Subject_id = new List<int>();

        public static ObservableCollection<Label> labelsStudent = new ObservableCollection<Label>();
        public static ObservableCollection<Button> buttonsStudent = new ObservableCollection<Button>();

        public static ObservableCollection<Label> labelsGroups = new ObservableCollection<Label>();
        public static ObservableCollection<Button> buttonsGroups = new ObservableCollection<Button>();

        public static ObservableCollection<Label> labelsTeachers = new ObservableCollection<Label>();
        public static ObservableCollection<Button> buttonsTeachers = new ObservableCollection<Button>();

        public static ObservableCollection<Label> labelsSpeciality = new ObservableCollection<Label>();
        public static ObservableCollection<Button> buttonsSpeciality = new ObservableCollection<Button>();

        public static ObservableCollection<Label> labelsFaculty = new ObservableCollection<Label>();
        public static ObservableCollection<Button> buttonsFaculty = new ObservableCollection<Button>();
        public static ObservableCollection<Button> buttonsSaveFaculty = new ObservableCollection<Button>();
        public static ObservableCollection<Button> buttonsCancelFaculty = new ObservableCollection<Button>();
        public static ObservableCollection<TextBox> textBoxFaculty = new ObservableCollection<TextBox>();

        public static ObservableCollection<Label> labelsSubject = new ObservableCollection<Label>();
        public static ObservableCollection<Label> labelsSubjectTeacher = new ObservableCollection<Label>();
        public static ObservableCollection<Button> buttonsSubject = new ObservableCollection<Button>();

        public static object sender_comboBox { get; set; }
        public static RoutedEventArgs e_comboBox { get; set; }
        public static string selectedGroup { get; set; }

        private void comboboxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            labelsStudent.Clear();
            buttonsStudent.Clear();

            confirmDeleteStudent.Visibility = Visibility.Hidden;
            selectedGroup = comboboxGroup.SelectedItem.ToString();
            sender_comboBox = sender;
            e_comboBox = e;
            ShowStudentList(sender_comboBox, e_comboBox);
        }

        private void DeleteStudent(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int id = Convert.ToInt32(clickedButton.Tag);



            if (buttonsStudent.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content.ToString() == "Отменить удаление")
            {
                buttonsStudent.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Удалить";
                labelsStudent.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Black;

                Student_id.Remove(Convert.ToInt32(clickedButton.Tag));
            }
            else
            {
                buttonsStudent.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Отменить удаление";
                labelsStudent.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Coral;

                Student_id.Add(Convert.ToInt32(clickedButton.Tag));
            }

            confirmDeleteStudent.Visibility = Visibility.Visible;
        }

        private void EditStudent(object sender, RoutedEventArgs e)
        {
            tabControlIndex = mainTabControl.SelectedIndex;
            Button clickedButton = (Button)sender;
            int id = Convert.ToInt32(clickedButton.Tag);

            gr691_mnmEntities db = new gr691_mnmEntities();

            var students = (from student in db.USER
                            join _group in db.GROUP on student.FK_GROUP_ID equals _group.GROUP_ID
                            where _group.GROUP1 == selectedGroup
                            where student.USER_ID == id
                            &&
                            student.FK_ROLE_ID == 3
                            select new
                            {
                                studId = student.USER_ID,
                                studSurname = student.SURNAME,
                                studName = student.NAME,
                                studPatr = student.PATRONYMIC,
                                selectedGroup = _group.GROUP1,
                                studLogin = student.LOGIN,
                                studPassword = student.PASSWORD
                            }).FirstOrDefault();

            MainWindow.MainFrame.Frame.Navigate(new EditStudentPage(students.studName, students.studSurname, students.studPatr,
                                                                    students.studLogin, students.studPassword, comboboxGroup.SelectedItem,
                                                                    students.studId, "edit"));
        }

        private void ConfirmDeleteStudent(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            foreach (int item in Student_id)
            {
                var student = db.USER.Where(w => w.USER_ID == item).FirstOrDefault();
                db.USER.Remove(student);

                foreach (var mark in db.MARK.Where(w => w.FK_STUDENT_ID == item))
                {
                    db.MARK.Remove(mark);
                }
            }
            db.SaveChanges();

            ShowStudentList(sender_comboBox, e_comboBox);
        }

        public void ShowStudentList(object sender, RoutedEventArgs e)
        {
            Student_id.Clear();

            if (confirmDeleteStudent.IsVisible)
            {
                confirmDeleteStudent.Visibility = Visibility.Hidden;
            }

            dockPanel.Children.Clear();
            dockPanel.Height = 284;

            gr691_mnmEntities db = new gr691_mnmEntities();
            ComboBox comboBox = (ComboBox)sender;

            string selectedItem;
            if (comboBox.SelectedItem != null)
            {
                selectedItem = comboBox.SelectedItem.ToString();
            }
            else
            {
                selectedItem = selectedGroup;
            }

            var students = from student in db.USER
                           join _group in db.GROUP on student.FK_GROUP_ID equals _group.GROUP_ID
                           where _group.GROUP1 == selectedItem
                           &&
                           student.FK_ROLE_ID == 3
                           select new
                           {
                               studId = student.USER_ID,
                               studSurname = student.SURNAME,
                               studName = student.NAME,
                               studPatr = student.PATRONYMIC
                           };

            StackPanel stackPanelStudent = new StackPanel();
            stackPanelStudent.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelStudent.Orientation = Orientation.Vertical;

            StackPanel stackPanelDelete = new StackPanel();
            stackPanelDelete.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelDelete.Orientation = Orientation.Vertical;

            StackPanel stackPanelEdit = new StackPanel();
            stackPanelEdit.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelEdit.Orientation = Orientation.Vertical;

            foreach (var student in students)
            {
                Label lblPerson = new Label();
                lblPerson.Content = student.studSurname + " " + student.studName + " " + student.studPatr;
                lblPerson.Background = Brushes.Azure;
                lblPerson.Foreground = Brushes.Black;
                lblPerson.HorizontalAlignment = HorizontalAlignment.Left;
                lblPerson.MinWidth = 270;
                lblPerson.MinHeight = 32;
                lblPerson.Margin = new Thickness(0, 15, 0, 0);
                labelsStudent.Add(lblPerson);

                Button deleteBtn = new Button();
                deleteBtn.MinWidth = 130;
                deleteBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                deleteBtn.Content = "Удалить";
                deleteBtn.Margin = new Thickness(15, 15, 0, 0);
                deleteBtn.HorizontalAlignment = HorizontalAlignment.Left;
                deleteBtn.MinHeight = 30;
                deleteBtn.Tag = student.studId;
                deleteBtn.Click += new RoutedEventHandler(this.DeleteStudent);

                Button editBtn = new Button();
                editBtn.MinWidth = 70;
                editBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                editBtn.MinHeight = 30;
                editBtn.Margin = new Thickness(15, 15, 0, 0);
                editBtn.Content = "Изменить";
                editBtn.HorizontalAlignment = HorizontalAlignment.Left;
                editBtn.Tag = student.studId;
                editBtn.Click += new RoutedEventHandler(this.EditStudent);

                stackPanelStudent.Children.Add(lblPerson);
                stackPanelDelete.Children.Add(deleteBtn);
                stackPanelEdit.Children.Add(editBtn);

                dockPanel.Height = dockPanel.Height + lblPerson.Height;

                lblPerson.Tag = deleteBtn.Tag;
                labelsStudent.Add(lblPerson);
                buttonsStudent.Add(deleteBtn);
            }
            dockPanel.Children.Add(stackPanelStudent);
            dockPanel.Children.Add(stackPanelDelete);
            dockPanel.Children.Add(stackPanelEdit);
        }

        private void AddStudent(object sender, RoutedEventArgs e)
        {
            tabControlIndex = mainTabControl.SelectedIndex;
            MainWindow.MainFrame.Frame.Navigate(new EditStudentPage(null, null, null, null, null, null, -1, "add"));
        }

        public void ShowGroupList(object sender, RoutedEventArgs e)
        {
            Group_id.Clear();
            dockPanelGroups.Children.Clear();

            if (confirmDeleteGroup.IsVisible)
            {
                confirmDeleteGroup.Visibility = Visibility.Hidden;
            }

            gr691_mnmEntities db = new gr691_mnmEntities();

            var groups = from _group in db.GROUP
                         select new
                         {
                             gr = _group.GROUP1,
                             grId = _group.GROUP_ID
                         };

            StackPanel stackPanelGroup = new StackPanel();
            stackPanelGroup.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelGroup.Orientation = Orientation.Vertical;

            StackPanel stackPanelDelete = new StackPanel();
            stackPanelDelete.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelDelete.Orientation = Orientation.Vertical;

            StackPanel stackPanelEdit = new StackPanel();
            stackPanelEdit.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelEdit.Orientation = Orientation.Vertical;

            foreach (var group in groups)
            {
                Label lblGroup = new Label();
                lblGroup.Content = group.gr;
                lblGroup.Background = Brushes.Azure;
                lblGroup.Foreground = Brushes.Black;
                lblGroup.HorizontalAlignment = HorizontalAlignment.Left;
                lblGroup.MinWidth = 120;
                lblGroup.MinHeight = 32;
                lblGroup.Margin = new Thickness(0, 15, 0, 0);

                Button deleteBtn = new Button();
                deleteBtn.MinWidth = 130;
                deleteBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                deleteBtn.Content = "Удалить";
                deleteBtn.Margin = new Thickness(15, 15, 0, 0);
                deleteBtn.HorizontalAlignment = HorizontalAlignment.Left;
                deleteBtn.MinHeight = 30;
                deleteBtn.Click += new RoutedEventHandler(this.DeleteGroup);

                Button editBtn = new Button();
                editBtn.MinWidth = 70;
                editBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                editBtn.MinHeight = 30;
                editBtn.Margin = new Thickness(15, 15, 0, 0);
                editBtn.Content = "Изменить";
                editBtn.HorizontalAlignment = HorizontalAlignment.Left;
                editBtn.Click += new RoutedEventHandler(this.EditGroup);

                deleteBtn.Tag = group.grId;
                lblGroup.Tag = deleteBtn.Tag;
                editBtn.Tag = deleteBtn.Tag;

                stackPanelGroup.Children.Add(lblGroup);
                stackPanelDelete.Children.Add(deleteBtn);
                stackPanelEdit.Children.Add(editBtn);

                dockPanelGroups.Height = dockPanel.Height + lblGroup.Height;


                labelsGroups.Add(lblGroup);
                buttonsGroups.Add(deleteBtn);
            }

            dockPanelGroups.Children.Add(stackPanelGroup);
            dockPanelGroups.Children.Add(stackPanelDelete);
            dockPanelGroups.Children.Add(stackPanelEdit);
        }

        private void DeleteGroup(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int id = Convert.ToInt32(clickedButton.Tag);

            if (buttonsGroups.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content.ToString() == "Отменить удаление")
            {
                buttonsGroups.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Удалить";
                labelsGroups.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Black;

                Group_id.Remove(Convert.ToInt32(clickedButton.Tag));

                if (Group_id.Count() == 0)
                {
                    confirmDeleteGroup.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                buttonsGroups.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Отменить удаление";
                labelsGroups.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Coral;

                Group_id.Add(Convert.ToInt32(clickedButton.Tag));
                confirmDeleteGroup.Visibility = Visibility.Visible;
            }
        }

        private void ConfirmDeleteGroup(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int id = Convert.ToInt32(button.Tag);

            gr691_mnmEntities db = new gr691_mnmEntities();

           
            foreach (var person in db.USER.Where(w => w.FK_GROUP_ID == id))
            {
                foreach (var mark in db.MARK.Where(w => w.FK_STUDENT_ID == person.USER_ID))
                {
                    db.MARK.Remove(mark);
                }
                db.USER.Remove(person);
            }

            foreach (int item in Group_id)
            {
                var group = db.GROUP.Where(w => w.GROUP_ID == item).FirstOrDefault();
                db.GROUP.Remove(group);
            }

            db.SaveChanges();

            comboboxGroup.Items.Clear();
            foreach (var item in db.GROUP)
            {
                comboboxGroup.Items.Add(item.GROUP1);
            }

            ShowGroupList(null, null);
            ShowStudentList(comboboxGroup, null);
        }

        public void ShowTeacherList(object sender, RoutedEventArgs e)
        {
            Teacher_id.Clear();

            if (confirmDeleteTeacher.IsVisible)
            {
                confirmDeleteTeacher.Visibility = Visibility.Hidden;
            }

            dockPanelTeachers.Children.Clear();
            dockPanelTeachers.Height = 284;

            gr691_mnmEntities db = new gr691_mnmEntities();

            var teachers = from _teacher in db.USER
                           where _teacher.FK_ROLE_ID == 2
                           select new
                           {
                               teacherId = _teacher.USER_ID,
                               name = _teacher.NAME,
                               surname = _teacher.SURNAME,
                               patronymic = _teacher.PATRONYMIC,

                           };

            StackPanel stackPanelTeacher = new StackPanel();
            stackPanelTeacher.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelTeacher.Orientation = Orientation.Vertical;

            StackPanel stackPanelDelete = new StackPanel();
            stackPanelDelete.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelDelete.Orientation = Orientation.Vertical;

            StackPanel stackPanelEdit = new StackPanel();
            stackPanelEdit.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelEdit.Orientation = Orientation.Vertical;

            foreach (var teacher in teachers)
            {
                Label lblTeacher = new Label();
                lblTeacher.Content = teacher.surname + " " + teacher.name + " " + teacher.patronymic;
                lblTeacher.Background = Brushes.Azure;
                lblTeacher.Foreground = Brushes.Black;
                lblTeacher.HorizontalAlignment = HorizontalAlignment.Left;
                lblTeacher.MinWidth = 270;
                lblTeacher.MinHeight = 32;
                lblTeacher.Margin = new Thickness(0, 15, 0, 0);
                labelsStudent.Add(lblTeacher);

                Button deleteBtn = new Button();
                deleteBtn.MinWidth = 130;
                deleteBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                deleteBtn.Content = "Удалить";
                deleteBtn.Margin = new Thickness(15, 15, 0, 0);
                deleteBtn.HorizontalAlignment = HorizontalAlignment.Left;
                deleteBtn.MinHeight = 30;
                deleteBtn.Tag = teacher.teacherId;
                deleteBtn.Click += new RoutedEventHandler(this.DeleteTeacher);

                Button editBtn = new Button();
                editBtn.MinWidth = 70;
                editBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                editBtn.MinHeight = 30;
                editBtn.Margin = new Thickness(15, 15, 0, 0);
                editBtn.Content = "Изменить";
                editBtn.HorizontalAlignment = HorizontalAlignment.Left;
                editBtn.Tag = teacher.teacherId;
                editBtn.Click += new RoutedEventHandler(this.EditTeacher);

                stackPanelTeacher.Children.Add(lblTeacher);
                stackPanelDelete.Children.Add(deleteBtn);
                stackPanelEdit.Children.Add(editBtn);

                dockPanelTeachers.Height = dockPanel.Height + lblTeacher.Height;

                lblTeacher.Tag = deleteBtn.Tag;
                labelsTeachers.Add(lblTeacher);
                buttonsTeachers.Add(deleteBtn);
            }
            dockPanelTeachers.Children.Add(stackPanelTeacher);
            dockPanelTeachers.Children.Add(stackPanelDelete);
            dockPanelTeachers.Children.Add(stackPanelEdit);
        }

        private void DeleteTeacher(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

            if (buttonsTeachers.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content.ToString() == "Отменить удаление")
            {
                buttonsTeachers.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Удалить";
                labelsTeachers.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Black;

                Teacher_id.Remove(Convert.ToInt32(clickedButton.Tag));
            }
            else
            {
                buttonsTeachers.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Отменить удаление";
                labelsTeachers.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Coral;

                Teacher_id.Add(Convert.ToInt32(clickedButton.Tag));
            }

            if (Teacher_id.Count() == 0)
            {
                confirmDeleteTeacher.Visibility = Visibility.Hidden;
            }
            else
            {
                confirmDeleteTeacher.Visibility = Visibility.Visible;
            }
        }

        private void AddTeacher(object sender, RoutedEventArgs e)
        {
            tabControlIndex = mainTabControl.SelectedIndex;
            MainWindow.MainFrame.Frame.Navigate(new EditTeacherPage(null, null, null, null, null, -1, "add"));

            gr691_mnmEntities db = new gr691_mnmEntities();

            Teachers.Clear();
            foreach (var teacher in db.USER.Where(w => w.FK_ROLE_ID == 2))
            {
                comboboxTeacher.Items.Add(teacher.SURNAME + " " + teacher.NAME + " " + teacher.PATRONYMIC);
                Teachers.Add(teacher.USER_ID, teacher.SURNAME + " " + teacher.NAME + " " + teacher.PATRONYMIC);
            }
        }

        private void ConfirmDeleteTeacher(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            foreach (int teacherId in Teacher_id)
            {
                foreach (var group in db.GROUP.Where(w => w.FK_TEACHER_ID == teacherId))
                {
                    group.FK_TEACHER_ID = null;
                }
                foreach (var subject in db.SUBJECT.Where(w => w.FK_TEACHER_ID == teacherId))
                {
                    subject.FK_TEACHER_ID = null;
                }
            }

            foreach (int item in Teacher_id)
            {
                var teacher = db.USER.Where(w => w.USER_ID == item).FirstOrDefault();
                db.USER.Remove(teacher);
            }

            db.SaveChanges();

            ShowTeacherList(null, null);
            ShowSubjectList(null, null);
        }

        private void EditTeacher(object sender, RoutedEventArgs e)
        {
            tabControlIndex = mainTabControl.SelectedIndex;
            Button clickedButton = (Button)sender;
            int id = Convert.ToInt32(clickedButton.Tag);

            gr691_mnmEntities db = new gr691_mnmEntities();

            var _teacher = (from teacher in db.USER
                            where teacher.USER_ID == id
                            select new
                            {
                                name = teacher.NAME,
                                surname = teacher.SURNAME,
                                patronymic = teacher.PATRONYMIC,
                                login = teacher.LOGIN,
                                password = teacher.PASSWORD
                            }).FirstOrDefault();

            MainWindow.MainFrame.Frame.Navigate(new EditTeacherPage(_teacher.name, _teacher.surname, _teacher.patronymic, _teacher.password,
                _teacher.login, id, "edit"));
        }

        private void EditGroup(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            Button clickedButton = (Button)sender;

            tabControlIndex = mainTabControl.SelectedIndex;

            int id = Convert.ToInt32(clickedButton.Tag);

            var group = db.GROUP.Where(w => w.GROUP_ID == id).FirstOrDefault();
            string course = group.COURSE.ToString();

            MainWindow.MainFrame.Frame.Navigate(new EditGroupPage(labelsGroups.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content.ToString(),
               id, "edit"));
        }

        private void AddGroup(object sender, RoutedEventArgs e)
        {
            tabControlIndex = mainTabControl.SelectedIndex;
            MainWindow.MainFrame.Frame.Navigate(new EditGroupPage("", 0, "add"));
        }

        private void ShowSpecialityList(object sender, RoutedEventArgs e)
        {
            Speciality_id.Clear();
            dockPanelSpeciality.Children.Clear();

            if (confirmDeleteSpeciality.IsVisible)
            {
                confirmDeleteSpeciality.Visibility = Visibility.Hidden;
            }

            gr691_mnmEntities db = new gr691_mnmEntities();

            StackPanel stackPanelSpeciality = new StackPanel();
            stackPanelSpeciality.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelSpeciality.Orientation = Orientation.Vertical;

            StackPanel stackPanelDelete = new StackPanel();
            stackPanelDelete.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelDelete.Orientation = Orientation.Vertical;

            StackPanel stackPanelEdit = new StackPanel();
            stackPanelEdit.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelEdit.Orientation = Orientation.Vertical;

            foreach (var speciality in db.SPECIALITY)
            {
                Label lblGroup = new Label();
                lblGroup.Content = speciality.SPECIALITY1 + " " + speciality.CODE;
                lblGroup.Background = Brushes.Azure;
                lblGroup.Foreground = Brushes.Black;
                lblGroup.HorizontalAlignment = HorizontalAlignment.Left;
                lblGroup.MinWidth = 240;
                lblGroup.MinHeight = 32;
                lblGroup.Margin = new Thickness(0, 15, 0, 0);

                Button deleteBtn = new Button();
                deleteBtn.MinWidth = 130;
                deleteBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                deleteBtn.Content = "Удалить";
                deleteBtn.Margin = new Thickness(15, 15, 0, 0);
                deleteBtn.HorizontalAlignment = HorizontalAlignment.Left;
                deleteBtn.MinHeight = 30;
                deleteBtn.Click += new RoutedEventHandler(this.DeleteSpeciality);

                Button editBtn = new Button();
                editBtn.MinWidth = 70;
                editBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                editBtn.MinHeight = 30;
                editBtn.Margin = new Thickness(15, 15, 0, 0);
                editBtn.Content = "Изменить";
                editBtn.HorizontalAlignment = HorizontalAlignment.Left;
                editBtn.Click += new RoutedEventHandler(this.EditSpeciality);

                deleteBtn.Tag = speciality.SPECIALITY_ID;
                lblGroup.Tag = deleteBtn.Tag;
                editBtn.Tag = deleteBtn.Tag;

                stackPanelSpeciality.Children.Add(lblGroup);
                stackPanelDelete.Children.Add(deleteBtn);
                stackPanelEdit.Children.Add(editBtn);


                dockPanelSpeciality.Height = dockPanel.Height + lblGroup.Height;

                labelsSpeciality.Add(lblGroup);
                buttonsSpeciality.Add(deleteBtn);
            }

            dockPanelSpeciality.Children.Add(stackPanelSpeciality);
            dockPanelSpeciality.Children.Add(stackPanelDelete);
            dockPanelSpeciality.Children.Add(stackPanelEdit);
        }

        private void DeleteSpeciality(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int id = Convert.ToInt32(clickedButton.Tag);

            if (buttonsSpeciality.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content.ToString() == "Отменить удаление")
            {
                buttonsSpeciality.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Удалить";
                labelsSpeciality.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Black;

                Speciality_id.Remove(Convert.ToInt32(clickedButton.Tag));

                if (Speciality_id.Count() == 0)
                {
                    confirmDeleteSpeciality.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                buttonsSpeciality.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Отменить удаление";
                labelsSpeciality.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Coral;

                Speciality_id.Add(Convert.ToInt32(clickedButton.Tag));
                confirmDeleteSpeciality.Visibility = Visibility.Visible;
            }
        }

        private void ConfirmDeleteSpeciality(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            foreach (int specialityId in Speciality_id)
            {
                foreach (var group in db.GROUP.Where(w => w.FK_SPECIALITY_ID == specialityId))
                {
                    group.FK_SPECIALITY_ID = null;
                }
            }

            foreach (int item in Speciality_id)
            {
                var speciality = db.SPECIALITY.Where(w => w.SPECIALITY_ID == item).FirstOrDefault();
                db.SPECIALITY.Remove(speciality);
            }

            db.SaveChanges();

            ShowSpecialityList(null, null);
        }

        private void AddSpeciality(object sender, RoutedEventArgs e)
        {
            Validation validation = new Validation();

            if (txtSpeciality.Text == "" || txtCode.Text == "")
            {
                lblErrorSpeciality.Content = "Поле не может быть пустым";
                lblErrorSpeciality.Visibility = Visibility.Visible;
            }
            else if (validation.Integer(txtCode.Text))
            {
                lblErrorSpeciality.Content = "Только целочисленнй тип";
                lblErrorSpeciality.Visibility = Visibility.Visible;
            }
            else
            {
                gr691_mnmEntities db = new gr691_mnmEntities();

                SPECIALITY newSpeciality = new SPECIALITY();
                newSpeciality.SPECIALITY1 = txtSpeciality.Text;
                newSpeciality.CODE = Convert.ToInt32(txtCode.Text);

                db.SPECIALITY.Add(newSpeciality);
                db.SaveChanges();

                txtSpeciality.Text = "";
                txtCode.Text = "";

                lblErrorSpeciality.Visibility = Visibility.Hidden;
                ShowSpecialityList(null, null);
            }
        }

        private void EditSpeciality(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

            gr691_mnmEntities db = new gr691_mnmEntities();
            int id = Convert.ToInt32(clickedButton.Tag);

            var speciality = db.SPECIALITY.Where(w => w.SPECIALITY_ID == id).FirstOrDefault();

            tabControlIndex = mainTabControl.SelectedIndex;

            MainWindow.MainFrame.Frame.Navigate(new EditSpecialityPage(speciality.SPECIALITY1, speciality.CODE.ToString(), id));
        }

        private void ShowFacultyList(object sender, RoutedEventArgs e)
        {
            Faculty_id.Clear();
            dockPanelFaculty.Children.Clear();

            confirmDeleteFaculty.Visibility = Visibility.Hidden;

            gr691_mnmEntities db = new gr691_mnmEntities();

            StackPanel stackPanelFaculty = new StackPanel();
            stackPanelFaculty.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelFaculty.Orientation = Orientation.Vertical;

            StackPanel stackPanelDelete = new StackPanel();
            stackPanelDelete.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelDelete.Orientation = Orientation.Vertical;

            StackPanel stackPanelEdit = new StackPanel();
            stackPanelEdit.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelEdit.Orientation = Orientation.Vertical;

            StackPanel stackPanelTextBox = new StackPanel();
            stackPanelTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelTextBox.Orientation = Orientation.Vertical;


            StackPanel stackPanelSave = new StackPanel();
            stackPanelSave.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelSave.Orientation = Orientation.Vertical;

            StackPanel stackPanelCancel = new StackPanel();
            stackPanelCancel.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelCancel.Orientation = Orientation.Vertical;

            foreach (var faculty in db.FACULTY)
            {
                Label lblFaculty = new Label();
                lblFaculty.Content = faculty.FACULTY1;
                lblFaculty.Background = Brushes.Azure;
                lblFaculty.Foreground = Brushes.Black;
                lblFaculty.HorizontalAlignment = HorizontalAlignment.Left;
                lblFaculty.MinWidth = 240;
                lblFaculty.MinHeight = 32;
                lblFaculty.Margin = new Thickness(0, 15, 0, 0);

                Button deleteBtn = new Button();
                deleteBtn.MinWidth = 130;
                deleteBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                deleteBtn.Content = "Удалить";
                deleteBtn.Margin = new Thickness(15, 15, 0, 0);
                deleteBtn.HorizontalAlignment = HorizontalAlignment.Left;
                deleteBtn.MinHeight = 32;
                deleteBtn.Click += new RoutedEventHandler(this.DeleteFaculty);

                Button editBtn = new Button();
                editBtn.MinWidth = 70;
                editBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                editBtn.MinHeight = 30;
                editBtn.Margin = new Thickness(15, 15, 0, 0);
                editBtn.Content = "Изменить";
                editBtn.HorizontalAlignment = HorizontalAlignment.Left;
                editBtn.Click += new RoutedEventHandler(this.EditFaculty);

                TextBox textBox = new TextBox();
                textBox.MinWidth = 280;
                textBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                textBox.VerticalContentAlignment = VerticalAlignment.Center;
                textBox.MinHeight = 30;
                textBox.Margin = new Thickness(15, 15, 0, 0);
                textBox.HorizontalAlignment = HorizontalAlignment.Left;
                textBox.Visibility = Visibility.Hidden;
                textBox.MaxLength = 40;

                Button saveBtn = new Button();
                saveBtn.MinWidth = 100;
                saveBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                saveBtn.Content = "Сохранить";
                saveBtn.Margin = new Thickness(15, 15, 0, 0);
                saveBtn.HorizontalAlignment = HorizontalAlignment.Left;
                saveBtn.MinHeight = 30;
                saveBtn.Click += new RoutedEventHandler(this.SaveFaculty);
                saveBtn.Visibility = Visibility.Hidden;

                Button cancelBtn = new Button();
                cancelBtn.MinWidth = 100;
                cancelBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                cancelBtn.Content = "Отмена";
                cancelBtn.Margin = new Thickness(15, 15, 0, 0);
                cancelBtn.HorizontalAlignment = HorizontalAlignment.Right;
                cancelBtn.VerticalContentAlignment = VerticalAlignment.Center;
                cancelBtn.MinHeight = 30;
                cancelBtn.Click += new RoutedEventHandler(this.CancelSaveFaculty);
                cancelBtn.Visibility = Visibility.Hidden;

                deleteBtn.Tag = faculty.FACULTY_ID;
                saveBtn.Tag = deleteBtn.Tag;
                cancelBtn.Tag = deleteBtn.Tag;
                lblFaculty.Tag = deleteBtn.Tag;
                editBtn.Tag = deleteBtn.Tag;
                textBox.Tag = deleteBtn.Tag;
                cancelBtn.Tag = deleteBtn.Tag;

                stackPanelTextBox.Children.Add(textBox);
                stackPanelFaculty.Children.Add(lblFaculty);
                stackPanelDelete.Children.Add(deleteBtn);
                stackPanelEdit.Children.Add(editBtn);
                stackPanelSave.Children.Add(saveBtn);
                stackPanelCancel.Children.Add(cancelBtn);

                dockPanelFaculty.Height = dockPanelFaculty.Height + lblFaculty.Height;

                buttonsSaveFaculty.Add(saveBtn);
                buttonsCancelFaculty.Add(cancelBtn);
                labelsFaculty.Add(lblFaculty);
                buttonsFaculty.Add(deleteBtn);
                textBoxFaculty.Add(textBox);
            }

            dockPanelFaculty.Children.Add(stackPanelFaculty);
            dockPanelFaculty.Children.Add(stackPanelDelete);
            dockPanelFaculty.Children.Add(stackPanelEdit);
            dockPanelFaculty.Children.Add(stackPanelTextBox);
            dockPanelFaculty.Children.Add(stackPanelSave);
            dockPanelFaculty.Children.Add(stackPanelCancel);
        }

        private void DeleteFaculty(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int id = Convert.ToInt32(clickedButton.Tag);

            if (buttonsFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content.ToString() == "Отменить удаление")
            {
                buttonsFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Удалить";
                labelsFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Black;

                Faculty_id.Remove(Convert.ToInt32(clickedButton.Tag));

                if (Faculty_id.Count() == 0)
                {
                    confirmDeleteFaculty.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                buttonsFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Отменить удаление";
                labelsFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Coral;

                Faculty_id.Add(Convert.ToInt32(clickedButton.Tag));
                confirmDeleteFaculty.Visibility = Visibility.Visible;
            }
        }

        private void ConfirmDeleteFaculty(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            foreach (int facultyId in Faculty_id)
            {
                foreach (var group in db.GROUP.Where(w => w.FK_FACULTY_ID == facultyId))
                {
                    group.FK_SPECIALITY_ID = null;
                }
            }

            foreach (int item in Faculty_id)
            {
                var faculty = db.FACULTY.Where(w => w.FACULTY_ID == item).FirstOrDefault();
                db.FACULTY.Remove(faculty);
            }

            db.SaveChanges();

            ShowFacultyList(null, null);
        }

        private void AddFaculty(object sender, RoutedEventArgs e)
        {
            Validation validation = new Validation();

            if (txtFaculty.Text == "")
            {
                lblErrorFaculty.Content = "Поле не может быть пустым";
                lblErrorFaculty.Visibility = Visibility.Visible;
            }
            else
            {
                gr691_mnmEntities db = new gr691_mnmEntities();

                FACULTY newFaculty = new FACULTY();
                newFaculty.FACULTY1 = txtFaculty.Text;
                db.FACULTY.Add(newFaculty);
                db.SaveChanges();

                lblErrorFaculty.Visibility = Visibility.Hidden;
                ShowFacultyList(null, null);
            }
        }

        private void EditFaculty(object sender, RoutedEventArgs e)
        {
            lblErrorFaculty.Visibility = Visibility.Hidden;
            Button clickedButton = (Button)sender;

            textBoxFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Visibility = Visibility.Visible;
            buttonsSaveFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Visibility = Visibility.Visible;
            buttonsCancelFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Visibility = Visibility.Visible;
        }

        private void SaveFaculty(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

            string text = textBoxFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Text;

            if (text == "")
            {
                lblErrorFaculty.Visibility = Visibility.Visible;
                lblErrorFaculty.Content = "Поле не может быть пустым";
            }
            else
            {
                gr691_mnmEntities db = new gr691_mnmEntities();
                int id = Convert.ToInt32(clickedButton.Tag);

                var faculty = db.FACULTY.Where(w => w.FACULTY_ID == id).FirstOrDefault();

                faculty.FACULTY1 = text;
                db.SaveChanges();

                dockPanelFaculty.Children.Clear();

                lblErrorFaculty.Visibility = Visibility.Hidden;
                ShowFacultyList(null, null);
            }
        }

        private void CancelSaveFaculty(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

            textBoxFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Visibility = Visibility.Hidden;
            buttonsSaveFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Visibility = Visibility.Hidden;
            buttonsCancelFaculty.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Visibility = Visibility.Hidden;
        }

        public static int selectedTeacher { get; set; }
        private void ShowSubjectList(object sender, RoutedEventArgs e)
        {
            Teachers.Clear();
            Subject_id.Clear();
            dockPanelSubject.Children.Clear();
            comboboxTeacher.Items.Clear();

            confirmDeleteSubject.Visibility = Visibility.Hidden;

            gr691_mnmEntities db = new gr691_mnmEntities();

            foreach (var teacher in db.USER.Where(w => w.FK_ROLE_ID == 2))
            {
                comboboxTeacher.Items.Add(teacher.SURNAME + " " + teacher.NAME + " " + teacher.PATRONYMIC);
                Teachers.Add(teacher.USER_ID, teacher.SURNAME + " " + teacher.NAME + " " + teacher.PATRONYMIC);
            }
            comboboxTeacher.Items.Add("Нет преподавателя");

            StackPanel stackPanelSubject = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Vertical
            };

            StackPanel stackPanelTeacher = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Vertical
            };

            StackPanel stackPanelDelete = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Vertical
            };

            StackPanel stackPanelEdit = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Vertical
            };

            var query = from subject in db.SUBJECT
                        join teacher in db.USER on subject.FK_TEACHER_ID equals teacher.USER_ID
                        into GroupJoin
                        from teacher in GroupJoin.DefaultIfEmpty()
                        select new
                        {
                            subject_id = subject.SUBJECT_ID,
                            subject = subject.NAME,
                            teacherName = (teacher == null ? "Нет преподавателя" : teacher.SURNAME +
                            " " + teacher.NAME + " " + teacher.PATRONYMIC)
                        };


            foreach (var subject in query.Where(w => w.subject != "Нет урока"))
            {
                Label lblSubject = new Label
                {
                    Content = subject.subject,
                    Background = Brushes.Azure,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    MinWidth = 120,
                    MinHeight = 32,
                    Margin = new Thickness(0, 15, 0, 0)
                };

                Label lblTeacher = new Label
                {
                    Content = subject.teacherName,
                    Background = Brushes.Azure,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    MinWidth = 240,
                    MinHeight = 32,
                    Margin = new Thickness(15, 15, 0, 0)
                };

                Button deleteBtn = new Button
                {
                    MinWidth = 130,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Content = "Удалить",
                    Margin = new Thickness(15, 15, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    MinHeight = 30
                };
                deleteBtn.Click += new RoutedEventHandler(this.DeleteSubject);

                Button editBtn = new Button
                {
                    MinWidth = 70,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    MinHeight = 30,
                    Margin = new Thickness(15, 15, 0, 0),
                    Content = "Изменить",
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                editBtn.Click += new RoutedEventHandler(this.EditSubject);

                deleteBtn.Tag = subject.subject_id;
                lblTeacher.Tag = deleteBtn.Tag;
                editBtn.Tag = deleteBtn.Tag;
                lblSubject.Tag = deleteBtn.Tag;

                stackPanelSubject.Children.Add(lblSubject);
                stackPanelTeacher.Children.Add(lblTeacher);
                stackPanelDelete.Children.Add(deleteBtn);
                stackPanelEdit.Children.Add(editBtn);


                labelsSubject.Add(lblSubject);
                labelsSubjectTeacher.Add(lblTeacher);
                buttonsSubject.Add(deleteBtn);

                dockPanelSubject.Height += 15 + lblSubject.Height;
            }
            dockPanelSubject.Children.Add(stackPanelSubject);
            dockPanelSubject.Children.Add(stackPanelTeacher);
            dockPanelSubject.Children.Add(stackPanelDelete);
            dockPanelSubject.Children.Add(stackPanelEdit);
        }

        private void comboboxTeacher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            foreach (var teacher in Teachers)
            {
                if (teacher.Value == comboboxTeacher.SelectedItem.ToString())
                {
                    selectedTeacher = teacher.Key;
                }
            }
        }

        private void AddSubject(object sender, RoutedEventArgs e)
        {
            Validation validation = new Validation();

            if (txtSubject.Text == "")
            {
                lblErrorSubject.Content = "Поле не может быть пустым";
                lblErrorSubject.Visibility = Visibility.Visible;
            }
            else
            {
                gr691_mnmEntities db = new gr691_mnmEntities();
                SUBJECT subject = new SUBJECT();

                if (comboboxTeacher.SelectedItem == null || comboboxTeacher.SelectedItem.ToString() == "Нет преподавателя")
                {
                    subject.FK_TEACHER_ID = null;
                }
                else
                {
                    subject.FK_TEACHER_ID = selectedTeacher;
                }
                subject.NAME = txtSubject.Text;

                db.SUBJECT.Add(subject);
                db.SaveChanges();

                txtSubject.Text = "";
                lblErrorSubject.Visibility = Visibility.Hidden;
                ShowSubjectList(null, null);
            }
        }

        private void DeleteSubject(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

            if (buttonsSubject.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content.ToString() == "Отменить удаление")
            {
                buttonsSubject.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Удалить";
                labelsSubject.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Black;

                Subject_id.Remove(Convert.ToInt32(clickedButton.Tag));
            }
            else
            {
                buttonsSubject.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Content = "Отменить удаление";
                labelsSubject.Where(w => w.Tag == clickedButton.Tag).FirstOrDefault().Foreground = Brushes.Coral;

                Subject_id.Add(Convert.ToInt32(clickedButton.Tag));
            }

            if (Subject_id.Count() == 0)
            {
                confirmDeleteSubject.Visibility = Visibility.Hidden;
            }
            else
            {
                confirmDeleteSubject.Visibility = Visibility.Visible;
            }
        }

        private void ConfirmDeleteSubject(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            foreach (int id in Subject_id)
            {
                foreach (var mark in db.MARK.Where(w => w.FK_SUBJECT_ID == id))
                {
                    db.MARK.Remove(mark);
                }

                var subject = db.SUBJECT.Where(w => w.SUBJECT_ID == id).FirstOrDefault();
                db.SUBJECT.Remove(subject);
            }

            db.SaveChanges();

            ShowSubjectList(null, null);
        }

        private void EditSubject(object sender, RoutedEventArgs e)
        {
            Button clickebtButton = (Button)sender;

            tabControlIndex = mainTabControl.SelectedIndex;
            MainWindow.MainFrame.Frame.Navigate
                (new EditSubjectPage(labelsSubject.Where(w => w.Tag == clickebtButton.Tag).FirstOrDefault().Content.ToString(),
                Convert.ToInt32(clickebtButton.Tag)));
        }


        public static ObservableCollection<ComboBox> scheduleCombobox = new ObservableCollection<ComboBox>();

        StackPanel panel = new StackPanel();
        StackPanel stackPanelPersons = new StackPanel();

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!editMark.Contains(textBox))
            {
                editMark.Add(textBox);
            }
        }

        List<int> studentsList = new List<int>();

        StackPanel stackPanelMark = new StackPanel();
        
        private void markGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stackPanelMarkStudent.Children.Clear();
            stackPanelMark.Children.Clear();

            studentsList.Clear();
            textboxNewMark.Clear();
            stackPanelStudent.Clear();
            markSubject.Items.Clear();
            editMark.Clear();
            addTextBox.Clear();
            newTextBox.Clear();


            gr691_mnmEntities db = new gr691_mnmEntities();
            if (markGroup.SelectedItem != null)//Вывод групп, в которых преподаёт преподаватель
            {
                var subjects = (from subject in db.SUBJECT
                                join teacher in db.USER on subject.FK_TEACHER_ID equals teacher.USER_ID
                                join subjectGroup in db.SUBJECT_GROUP on subject.SUBJECT_ID equals subjectGroup.FK_SUBJECT_ID
                                join _group in db.GROUP on subjectGroup.FK_GROUP_ID equals _group.GROUP_ID
                                where teacher.USER_ID == MainWindow.UserInfo.userId
                                &&
                                _group.GROUP_ID == db.GROUP.Where(w => w.GROUP1 == markGroup.SelectedItem.ToString()).FirstOrDefault().GROUP_ID
                                select new
                                {
                                    _subject = subject.NAME
                                }).Distinct();

                foreach (var item in subjects)
                {
                    markSubject.Items.Add(item._subject);
                }

                var students = from student in db.USER
                               join _group in db.GROUP on student.FK_GROUP_ID equals _group.GROUP_ID
                               where _group.GROUP1 == markGroup.SelectedItem.ToString()
                               &&
                               student.FK_ROLE_ID == 3
                               select new
                               {
                                   id = student.USER_ID,
                                   surname = student.SURNAME,
                                   name = student.NAME,
                                   patronymic = student.PATRONYMIC
                               };

                foreach (var item in students)//Вывод студентов
                {
                    studentsList.Add(item.id);
                    Label person = new Label();
                    person.Content = item.surname + '\n' + item.name + '\n' + item.patronymic;
                    person.FontSize = 12;
                    person.Height = 60;
                    person.Background = Brushes.WhiteSmoke;
                    person.Margin = new Thickness(0, 0, 0, 5);
                    stackPanelMarkStudent.Children.Add(person);
                }
                stackPanelMarkStudent.Children.Add(stackPanelPersons);
            }
        }

        DatePicker datePickerMark = new DatePicker();
        StackPanel stackPanelDate = new StackPanel();
        ObservableCollection<TextBox> textboxNewMark = new ObservableCollection<TextBox>();
        ObservableCollection<TextBox> newTextBox = new ObservableCollection<TextBox>();
        ObservableCollection<TextBox> addTextBox = new ObservableCollection<TextBox>();
        ObservableCollection<StackPanel> stackPanelStudent = new ObservableCollection<StackPanel>();
        private void markSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridMark.Children.Remove(stackPanelMark);

            editMark.Clear();
            addTextBox.Clear();
            newTextBox.Clear();
            textboxNewMark.Clear();

            stackPanelMark.Children.Clear();
            stackPanelPersons.Children.Clear();

            panel.Children.Clear();

            if (MainWindow.UserInfo.role == 2)
            {
                ShowMark();
            }
        }

        private void DatePickerMark_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            DatePicker datePicker = (DatePicker)sender;

            if ((datePicker.Name == "dp1"))
            {
                dateToDate.Content = dp1.SelectedDate.Value.ToLongDateString() + " - " + dp1.SelectedDate.Value.AddDays(6).ToLongDateString();

                editMark.Clear();
                addTextBox.Clear();
                newTextBox.Clear();


                if (MainWindow.UserInfo.role == 2)
                {
                    ShowMark();
                }
                else
                {
                    ShowMarkToStudent();
                }
            }
            else
            {
                foreach (var student in stackPanelStudent)
                {
                    foreach (var textbox in newTextBox.Where(w => w.Parent == student))
                    {
                        textbox.Tag = datePickerMark.SelectedDate;

                        int subjectId = db.SUBJECT.Where(w => w.NAME == markSubject.SelectedItem.ToString()).FirstOrDefault().SUBJECT_ID;
                        int studentId = Convert.ToInt32(student.Tag);

                        var mark = db.MARK.Where(w => w.FK_SUBJECT_ID == subjectId && w.FK_STUDENT_ID == studentId
                                                   && w.DATE == datePickerMark.SelectedDate).FirstOrDefault();

                        if (mark != null)
                        {
                            textbox.Text = mark.MARK1.ToString();
                            editMark.Add(textbox);
                        }
                        else
                        {
                            textbox.Text = "";
                            addTextBox.Add(textbox);
                        }
                    }
                }
            }
        }

        ObservableCollection<TextBox> editMark = new ObservableCollection<TextBox>();

        private void SaveMark(object sender, RoutedEventArgs e)
        {
            gr691_mnmEntities db = new gr691_mnmEntities();
            foreach (var stack in stackPanelStudent)
            {
                foreach (var box in editMark.Where(w => w.Parent == stack))
                {
                    string mark = box.Text;
                    int student = Convert.ToInt32(stack.Tag);
                    DateTime date = Convert.ToDateTime(box.Tag);
                    int subject = db.SUBJECT.Where(w2 => w2.NAME == markSubject.SelectedItem.ToString()).FirstOrDefault().SUBJECT_ID;

                    if (int.TryParse(mark, out int n) || mark == "H" || mark == "Н")
                    {
                        var editMark = db.MARK.Where(w => w.FK_STUDENT_ID == student && w.FK_SUBJECT_ID == subject && w.DATE == date).FirstOrDefault();

                        if (editMark != null)
                        {
                            editMark.MARK1 = box.Text;
                        }
                        else
                        {
                            MARK newMark = new MARK();
                            newMark.MARK1 = box.Text;
                            newMark.FK_SUBJECT_ID = subject;
                            newMark.FK_STUDENT_ID = student;
                            newMark.DATE = date;
                            db.MARK.Add(newMark);
                        }
                    }
                    else if (mark == "")
                    {
                        var deleteMark = db.MARK.Where(w => w.FK_STUDENT_ID == student && w.FK_SUBJECT_ID == subject && w.DATE == date).FirstOrDefault();

                        if (deleteMark != null)
                        {
                            db.MARK.Remove(deleteMark);
                        }
                    }
                }

                if (datePickerMark.SelectedDate != null && addTextBox.Count() > 0)
                {
                    if (addTextBox.Where(w => w.Parent == stack && Convert.ToDateTime(w.Tag) == datePickerMark.SelectedDate).FirstOrDefault() != null)
                    {
                        string mark = addTextBox.Where(w => w.Parent == stack && Convert.ToDateTime(w.Tag) == datePickerMark.SelectedDate).FirstOrDefault().Text;
                        if ((int.TryParse(mark, out int n) || mark == "H" || mark == "Н"))
                        {
                            MARK newMark = new MARK();

                            newMark.MARK1 = mark;
                            newMark.FK_SUBJECT_ID = db.SUBJECT.Where(w => w.NAME ==
                            markSubject.SelectedItem.ToString()).FirstOrDefault().SUBJECT_ID;
                            newMark.FK_STUDENT_ID = Convert.ToInt32(stack.Tag);
                            newMark.DATE = datePickerMark.SelectedDate;

                            db.MARK.Add(newMark);
                        }
                    }
                }
                db.SaveChanges();
            }

            stackPanelStudent.Clear();

            editMark.Clear();
            addTextBox.Clear();
            newTextBox.Clear();

            textboxNewMark.Clear();
            stackPanelMark.Children.Clear();
            stackPanelPersons.Children.Clear();
            panel.Children.Clear();
            datePickerMark.SelectedDate = null;


            ShowMark();
        }

        public void ShowMark()
        {
            gr691_mnmEntities db = new gr691_mnmEntities();

            if (markSubject.SelectedItem != null)
            {
                gridMark.Children.Remove(stackPanelMark);

                stackPanelDate.Children.Clear();
                stackPanelMark.Children.Clear();

                stackPanelMark.Margin = new Thickness(100, 0, 0, 0);
                stackPanelDate.Orientation = Orientation.Horizontal;
                stackPanelDate.Height = 60;
                stackPanelDate.VerticalAlignment = VerticalAlignment.Top;

                int subjectId = db.SUBJECT.Where(w => w.NAME == markSubject.SelectedItem.ToString()).FirstOrDefault().SUBJECT_ID;

                DateTime date2 = dp1.SelectedDate.Value.AddDays(6);

                var markDate = (from mark in db.MARK
                                join student in db.USER on mark.FK_STUDENT_ID equals student.USER_ID
                                join _group in db.GROUP on student.FK_GROUP_ID equals _group.GROUP_ID
                                where _group.GROUP1 == markGroup.SelectedItem.ToString()
                                &&
                                mark.FK_SUBJECT_ID == subjectId
                                &&
                                mark.DATE >= dp1.SelectedDate.Value
                                &&
                                mark.DATE <= date2
                                select new
                                {
                                    markId = mark.MARK_ID,
                                    studentId = student.USER_ID,
                                    date = mark.DATE,
                                    mark = mark.MARK1,
                                    subjectId = mark.FK_SUBJECT_ID
                                });

                foreach (var date in markDate.Distinct().GroupBy(g => g.date))//Прорисовка дат
                {
                    Label dateMark = new Label();
                    dateMark.Width = 100;
                    dateMark.Background = Brushes.WhiteSmoke;
                    dateMark.Content = date.Key.Value.ToLongDateString();
                    dateMark.Margin = new Thickness(0, 0, 5, 0);

                    stackPanelDate.Children.Add(dateMark);
                }
                

                datePickerMark.Width = 100;
                datePickerMark.Height = 60;
                datePickerMark.SelectedDateChanged += DatePickerMark_SelectedDateChanged;
                stackPanelDate.Children.Add(datePickerMark);

                Label skipMark = new Label();
                skipMark.Width = 110;
                skipMark.Background = Brushes.WhiteSmoke;
                skipMark.Content = "Пропуски";
                skipMark.Margin = new Thickness(5, 0, 0, 0);
                skipMark.HorizontalContentAlignment = HorizontalAlignment.Center;

                stackPanelDate.Children.Add(skipMark);

                Label semestr1 = new Label();
                semestr1.Width = 110;
                semestr1.Background = Brushes.WhiteSmoke;
                semestr1.Content = "Семестр 1";
                semestr1.Margin = new Thickness(0, 0, 5, 0);
                semestr1.HorizontalContentAlignment = HorizontalAlignment.Center;

                stackPanelDate.Children.Add(semestr1);

                Label semestr2 = new Label();
                semestr2.Width = 110;
                semestr2.Background = Brushes.WhiteSmoke;
                semestr2.Content = "Семестр 2";
                semestr2.Margin = new Thickness(0, 0, 5, 0);
                semestr2.HorizontalContentAlignment = HorizontalAlignment.Center;

                stackPanelDate.Children.Add(semestr2);

                Label lblFinalMark = new Label();
                lblFinalMark.Width = 110;
                lblFinalMark.Background = Brushes.WhiteSmoke;
                lblFinalMark.Content = "Итоговая оценка";
                lblFinalMark.Margin = new Thickness(5, 0, 0, 0);
                lblFinalMark.HorizontalContentAlignment = HorizontalAlignment.Center;

                stackPanelDate.Children.Add(lblFinalMark);

                stackPanelMark.Children.Add(stackPanelDate);

                foreach (int id in studentsList)// Студенты
                {
                    StackPanel stack = new StackPanel();
                    stackPanelStudent.Add(stack);
                    stack.Orientation = Orientation.Horizontal;
                    stack.Height = 65;
                    stack.Tag = id;
                    stack.HorizontalAlignment = HorizontalAlignment.Left;

                    int fkSubjectId = db.SUBJECT.Where(w => w.NAME == markSubject.SelectedItem.ToString()).FirstOrDefault().SUBJECT_ID;

                    foreach (var row in markDate.Distinct().GroupBy(g => g.date))
                    {
                        TextBox textBox = new TextBox();

                        textBox.MaxLength = 1;
                        textBox.Width = 100;
                        textBox.Height = 65;
                        textBox.HorizontalAlignment = HorizontalAlignment.Left;
                        textBox.Tag = row.Key.Value;
                        textBox.Margin = new Thickness(0, 0, 5, 0);

                        var markRow = markDate.Where(w => w.date == row.Key.Value && w.studentId == id).FirstOrDefault();
                        if (markDate.Where(w => w.date == row.Key.Value && w.studentId == id).FirstOrDefault() != null)
                        {
                            textBox.Text = markDate.Where(w => w.date == row.Key.Value && w.studentId == id &&
                            w.subjectId == fkSubjectId).FirstOrDefault().mark;
                        }
                        else
                        {
                            textBox.Text = "";
                        }
                        stack.Children.Add(textBox);
                        textBox.TextChanged += TextBox_TextChanged;
                    }

                    TextBox mark = new TextBox();//Новая оценка
                    mark.Width = 100;
                    mark.Height = 65;
                    mark.MaxLength = 1;
                    mark.Margin = new Thickness(0, 0, 5, 0);

                    stack.Children.Add(mark);
                    newTextBox.Add(mark);

                    Label skip = new Label();//Пропуски
                    skip.Width = 100;
                    skip.Height = 65;
                    skip.HorizontalContentAlignment = HorizontalAlignment.Center;
                    skip.Margin = new Thickness(0, 0, 5, 0);
                    skip.Content = SkipMark(fkSubjectId, id);

                    stack.Children.Add(skip);

                    double yearMark = 0;

                    Label sem1 = new Label();//Первый семестр
                    sem1.FontSize = 16;
                    sem1.VerticalContentAlignment = VerticalAlignment.Center;
                    sem1.HorizontalContentAlignment = HorizontalAlignment.Center;
                    sem1.Height = 45;
                    sem1.Width = 110;
                    sem1.Margin = new Thickness(0, 0, 5, 0);

                    if (DateTime.Today.Month >= 9 && DateTime.Today.Month <= 12)
                    {
                        sem1.Content = AvgMark(fkSubjectId, id, new DateTime(DateTime.Today.Year, 9, 1), new DateTime(DateTime.Today.Year, 12, 31));
                    }
                    else if (DateTime.Today.Month >= 1 && DateTime.Today.Month <= 6)
                    {
                        sem1.Content = AvgMark(fkSubjectId, id, new DateTime(DateTime.Today.Year - 1, 9, 1), new DateTime(DateTime.Today.Year - 1, 12, 31));
                    }

                    if (sem1.Content != "")
                        yearMark += Convert.ToDouble(sem1.Content);

                    stack.Children.Add(sem1);

                    Label sem2 = new Label();//Второй семестр
                    sem2.FontSize = 16;
                    sem2.VerticalContentAlignment = VerticalAlignment.Center;
                    sem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                    sem2.Height = 45;
                    sem2.Width = 110;
                    sem2.Margin = new Thickness(0, 0, 5, 0);

                    if (DateTime.Today.Month >= 1 && DateTime.Today.Month <= 6)
                    {
                        sem2.Content = AvgMark(fkSubjectId, id, new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 6, 30));
                    }
                    if (sem2.Content != "")
                        yearMark += Convert.ToDouble(sem2.Content);

                    stack.Children.Add(sem2);

                    Label finalMark = new Label();//Годовая оценка
                    finalMark.Width = 110;
                    finalMark.HorizontalContentAlignment = HorizontalAlignment.Center;
                    finalMark.Height = 65;
                    finalMark.Margin = new Thickness(0, 0, 5, 0);

                    if (sem1.Content != "" & sem2.Content != "")
                    {
                        finalMark.Content = (yearMark / 2).ToString();
                        if (finalMark.Content.ToString().Length > 3)
                        {
                            finalMark.Content = (finalMark.Content).ToString().Substring(0, 3);
                        }
                    }

                    

                    stack.Children.Add(finalMark);

                    stackPanelMark.Children.Add(stack);
                }

                gridMark.Children.Add(stackPanelMark);
            }
        }
    }
}
