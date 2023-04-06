using Random_Selector.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Random_Selector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath = Directory.GetCurrentDirectory() + "\\Students.txt";
        //string filePath = @"C:\Intec\RandomSelector\Students.txt";
        List<Student> students = new List<Student>();
        List<Student> studentsGroup = new List<Student>();
        public MainWindow()
        {
            InitializeComponent();
            LoadStudents();
        }

        private void LoadStudents()
        {
            lstAllStudents.Items.Clear();
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Students Database has no records.\nPlease insert a student record!");
            }
            else
            {
                students = GetStudentsFromFile();
                foreach (var student in students)
                {
                    lstAllStudents.Items.Add(student);
                }
            }

        }
        private List<Student> GetStudentsFromFile()
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Students Database has no records.\nPlease insert a student record!");
            }
            else
            {
                IEnumerable<string> lines = File.ReadAllLines(filePath).ToList();
                foreach (string line in lines)
                {
                    string[] entries = line.Split(',');
                    Student student = new Student();
                    student.Level = int.Parse(entries[0]);
                    student.FirstName = entries[1];
                    student.LastName = entries[2];
                    students.Add(student);
                }
            }
            return students;
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            Student student = new Student();
            student.Level = int.Parse(txtLevel.Text);
            student.FirstName = txtFirstName.Text;
            student.LastName = txtLastName.Text;
            InsertStudent(student);
        }
        private void InsertStudent(Student student)
        {
            //if (!File.Exists(filePath))
            //{
            //    File.Create(filePath);
            //}
            //else
            //{
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(student.Level + "," + student.FirstName + "," + student.LastName);
            }
            //}
            ClearFields();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            GenerateGroup();
        }
        private void GenerateGroup()
        {
            lstGroup.Items.Clear();

            int counter = int.Parse(txtGroupText.Text);

            Random random = new Random();

            var level1Students = students.Where(s => s.Level == 1).ToList();
            int index = random.Next(0, level1Students.Count-1);
            studentsGroup.Add(level1Students[index]);
            students.RemoveAt(index);

            counter = counter - 1;

            for (int i = 0; i < counter; i++)
            {
                int randomindex = random.Next(0, students.Count-1);
                if (students[randomindex].Level == 1)
                {
                    i--;
                   continue;                  
                }
                else
                {
                    studentsGroup.Add(students[randomindex]);
                    students.RemoveAt(randomindex);
                }           
            }
            LoadGroup();
            LoadStudents();
            studentsGroup.Clear();
           
        }
        private void LoadGroup()
        {
            lstGroup.Items.Clear();
            foreach (var student in studentsGroup)
            {
                lstGroup.Items.Add(student);
            }     
        }
        private void ClearFields()
        {
            txtLevel.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
        }
    }
}
