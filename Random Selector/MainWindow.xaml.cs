﻿using Random_Selector.Models;
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

        List<Student> students = new List<Student>(); // main list
        List<Student> studentsGroup = new List<Student>(); // grouped list
        public MainWindow()
        {
            InitializeComponent();
            LoadStudents();
        }
        // Method loading all the students in the listbox
        private void LoadStudents()
        {
            // add content of CSV-file to listbox
            lstAllStudents.Items.Clear();
            students.Clear();
            students = GetStudentsFromFile().ToList();
            foreach (var student in students)
            {
                lstAllStudents.Items.Add(student);
            }
        }
        // Method reading all the students in the CSV-file and puts them in a list
        private IEnumerable<Student> GetStudentsFromFile()
        {
            // If CSV-file exists the read content and assign the values to the corresponding student properties
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
            ClearFields();
        }
        // Method inserting student inputted in the textboxes
        private void InsertStudent(Student student)
        {
            // Create file if not already exists in the given directory
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
        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            GenerateGroup();
        }
        // Method generates a group based on value inputted in textbox
        private void GenerateGroup()
        {
            lstGroup.Items.Clear();

            int counter = int.Parse(txtGroupText.Text);

            Random random = new Random();
            // Get all level 1 students and put them into list
            var level1Students = students.Where(s => s.Level == 1).ToList();
            // Get one level 1 student randomly in the level1Students list
            int index = random.Next(0, level1Students.Count - 1);
            studentsGroup.Add(level1Students[index]);
            students.RemoveAt(index);
            // counter - 1 because student with level 1 is added to the group.
            counter = counter - 1;
            // Add remaining students randomly to the group on condition that they are not of level 1.
            for (int i = 0; i < counter; i++)
            {
                int randomindex = random.Next(0, students.Count - 1);
                if (students[randomindex].Level == 1)
                {
                    i--; // decrement i because the to rest the loopcounter to the value before encoutering a level 1 student
                    continue;
                }
                else
                {
                    studentsGroup.Add(students[randomindex]); // add student to the group
                    students.RemoveAt(randomindex); // remove the student from the main list
                }
            }
            UpdateCSVStudents();
            LoadGroup();
            LoadStudents();
        ;

        }
        // Method updating CSV-file by removing the grouped students
        private void UpdateCSVStudents()
        {
            // Append = false, because generating renewed CSV-file of students 
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                foreach (var student in students)
                {
                    writer.WriteLine(student.Level + "," + student.FirstName + "," + student.LastName);
                }
            }
        }
        
        // Method loading the randomly grouped students in the listbox
        private void LoadGroup()
        {
            lstGroup.Items.Clear();
            foreach (var student in studentsGroup)
            {
                lstGroup.Items.Add(student);
            }
        }
        // Method clearing the textboxes
        private void ClearFields()
        {
            txtLevel.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtGroupText.Text = string.Empty;
        }
        private void btnWriteCSVGroupedStudents_Click(object sender, RoutedEventArgs e)
        {
            WriteCSVGroupedStudents();
            ClearFields();
            lstGroup.Items.Clear();
            studentsGroup.Clear();
            LoadStudents();
        }
        // Method writing grouped students to new CSV-file
        private int groupnumber = 0;
        private void WriteCSVGroupedStudents()
        {
            groupnumber++;
            string file = Directory.GetCurrentDirectory() + "\\Group" + groupnumber + ".txt";
            if (!File.Exists(file))
            {
                File.Create(file);
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(file, true))
                {
                    foreach (var student in studentsGroup)
                    {
                        writer.WriteLine(student.Level + "," + student.FirstName + "," + student.LastName);
                    }
                    if (File.Exists(file))
                    {
                        MessageBox.Show("CSV Gropued Students has been written");
                    }
                    else
                    {
                        MessageBox.Show("Error!\nThe CSV has not been written!");
                    }
                }      
            }
        }
    }
}
