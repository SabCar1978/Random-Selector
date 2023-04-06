using Random_Selector.Models;
using System;
using System.Collections.Generic;
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
        string filePath = @"C:\Intec\RandomSelector\Students.txt";
        List<Student> students = new List<Student>();
        List<Student> studentsGroup = new List<Student>();    
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<Student> ReadFromFile()
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
            student.LastName = txtFirstName.Text;
            InsertStudent(student);
        }
        private void InsertStudent(Student student)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(student.Level + "," + student.FirstName + "," + student.LastName);
                }
            }
        }
    }
}
