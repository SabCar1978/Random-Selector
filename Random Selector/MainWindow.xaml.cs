﻿using Random_Selector.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Random_Selector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //---------------------------------------------------------------------
        // GLOBAL VARIABLES
        //---------------------------------------------------------------------

        // filepath locatede in bin directory of this project
        string filePath = Directory.GetCurrentDirectory() + "\\Studenten.txt";

        // Students list will show in listbox and is used to write(update, delete) to the CSV textfile Students.txt
        List<Student> students = new List<Student>();

        // StudentsGroup list will show in other listbox and is used to write(update, delete) to the CSV textfile GroepXX.txt
        List<Student> studentsGroup = new List<Student>();

        // declaring variable that represents the index of the selected student
        int selectedStudentIndex = 0;

        // declaring the randomgenerator for generating index to form the group of students
        Random random = new Random();

        //Main Window Constructor
        public MainWindow()
        {
            InitializeComponent();
            LoadStudents();
        }

        //---------------------------------------------------------------------
        //METHODS RELATED TO TEXTFILE EN LIST STUDENTS
        //---------------------------------------------------------------------

        // Method loading students from textfile into listbox.
        private void LoadStudents()
        {
            // if there are no students in the file, show message in listbox
            if (CheckStudentsCount() == 0)
            {
                lstAllStudents.Items.Add("Geef studenten in a.u.b.!");
            }
            else
            {
                foreach (var student in students)
                {
                    lstAllStudents.Items.Add(student);
                }
            }
        }
        // Method checking if there are students in the textfile
        private int CheckStudentsCount()
        {
            // add content of CSV-file to listbox
            lstAllStudents.Items.Clear();
            students.Clear();
            students = GetStudentsFromFile().ToList();
            return students.Count;
        }
        // Method reading all the students in the CSV-file and puts them in a list
        private IEnumerable<Student> GetStudentsFromFile()
        {

            // If CSV-file exists the read content and assign the values to the corresponding student properties
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
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

        //---------------------------------------------------------------------
        // SELECT STUDENT
        //---------------------------------------------------------------------

        // Showing selected student from the listbox in the corresponding textboxes.
        private void lstAllStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Student selectedStudent = new Student();
            selectedStudent = lstAllStudents.SelectedItem as Student;
            if (selectedStudent != null)
            {
                txtLevel.Text = selectedStudent.Level.ToString();
                txtFirstName.Text = selectedStudent.FirstName;
                txtLastName.Text = selectedStudent.LastName;
            }
            selectedStudentIndex = lstAllStudents.SelectedIndex;
        }

        //---------------------------------------------------------------------
        // INSERT NEW STUDENT
        //---------------------------------------------------------------------

        // Click on btnInsert wiil insert a new student into the textfile and then show the student in lstAllStudents
        private async void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            lstAllStudents.Items.Clear();
            Student student = new Student();
            if (String.IsNullOrEmpty(txtLevel.Text) || String.IsNullOrEmpty(txtFirstName.Text) || String.IsNullOrEmpty(txtLastName.Text))
            {
                MessageBox.Show("Vul alle velden in a.u.b.!");
            }
            else if (int.Parse(txtLevel.Text) <= 0 || int.Parse(txtLevel.Text) > 4)
            {
                MessageBox.Show("'Level' moet een getal van 1 tot en met 4 zijn!");
            }
            else
            {
                student.Level = int.Parse(txtLevel.Text);
                student.FirstName = txtFirstName.Text;
                student.LastName = txtLastName.Text;
                await InsertStudentAsync(student);
                ClearFields();
            }
        }
        // Method inserting student inputted in the textboxes
        private async Task InsertStudentAsync(Student student)
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
                    await writer.WriteLineAsync(student.Level + "," + student.FirstName + "," + student.LastName);
                }
            }
            LoadStudents();
        }

        //---------------------------------------------------------------------
        // UPDATE EXISTING STUDENT
        //---------------------------------------------------------------------

        // Click on btnUpdate will update the textfile with the changed values for the selected student and refresh the listbox
        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Student updatedStudent = new Student();
            if (String.IsNullOrEmpty(txtLevel.Text) || String.IsNullOrEmpty(txtFirstName.Text) || String.IsNullOrEmpty(txtLastName.Text))
            {
                MessageBox.Show("Vul alle velden in a.u.b.!");
            }
            else
            {
                updatedStudent.Level = int.Parse(txtLevel.Text);
                updatedStudent.FirstName = txtFirstName.Text;
                updatedStudent.LastName = txtLastName.Text;
                await UpdateStudent(updatedStudent);
                ClearFields();
                LoadStudents();
            }
        }
        // Method to alter the students list with the updated values of the selected student.
        // The updated list will be written to textfile in order to update the textfile. 
        private async Task UpdateStudent(Student updatedstudent)
        {
            // update students list with the changed student values based on its index retrieved fr
            students[selectedStudentIndex] = updatedstudent;
            await UpdateCSVStudentsAsync();
        }

        //---------------------------------------------------------------------
        // DELETE EXISTING STUDENT
        //---------------------------------------------------------------------

        // Click btnDelete will show a message to confirm if the user wants to delete the selected student.
        // If yes, it performs the deletion of the student from list and textfile
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Bent u zeker dat u volgende student wilt verwijderen:\n{txtLevel.Text} {txtFirstName.Text} {txtLastName.Text}",
                                                      "Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteStudent(selectedStudentIndex);
                ClearFields();
                LoadStudents();
            }
            else
            {
                ClearFields();
                LoadStudents();
            }
        }
        //Delete the student from list and textfile based on index retrieved from selected student in listbox
        private async Task DeleteStudent(int index)
        {
            students.RemoveAt(index);
            await UpdateCSVStudentsAsync();
        }

        //-------------------------------------------------------------------------
        // GENERATE GROUP OF RANDOMLY CHOSEN STUDENTS (1 LEVEL 1 STUDENT REQUIRED)
        //-------------------------------------------------------------------------

        // Click on btnGenerate will generate the group with number specified in txtGroup
        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            await GenerateGroup();
        }
        // Method generates a group based on value inputted in textbox
        private async Task GenerateGroup()
        {
            lstGroup.Items.Clear();
            int aantalAllStudents = students.Count;

            int counter = int.Parse(txtGroupText.Text);
            // Checking if there are enough students to group according inputted number
            if (aantalAllStudents == 0)
            {
                MessageBox.Show("Er zijn geen studenten in het bestand!\nVoer nieuwe studenten in!");
                txtGroupText.Text = string.Empty;
                counter = 0;
            }
            else if (aantalAllStudents < counter)
            {
                MessageBox.Show("Er zijn niet genoeg studenten!\nGeef een kleiner aantal in of voer nieuwe studenten in!");
                txtGroupText.Text = string.Empty;
                counter = 0;
            }
            else
            {
                // Get all level 1 students and put them into list
                var level1Students = students.Where(s => s.Level == 1).ToList();
                int aantalLevel1Students = level1Students.Count;
                int aantalNonLevel1Students = aantalAllStudents - aantalLevel1Students;
                if (aantalLevel1Students == 0)
                {
                    MessageBox.Show("Er zijn geen level-1 studenten!\nVoer een level-1 student in!");
                    txtGroupText.Text = string.Empty;
                    counter = 0;
                }
                else if (aantalNonLevel1Students < counter - 1)
                {
                    MessageBox.Show("Er zijn te weinig non-level-1 studenten!\nGeef een kleiner aantal in of voer nieuwe studenten in!");
                    txtGroupText.Text = string.Empty;
                    counter = 0;
                }
                else
                {   // Get one level 1 student randomly in the level1Students list
                    int index = random.Next(0, level1Students.Count);
                    studentsGroup.Add(level1Students[index]);
                    var result = students.IndexOf(students.Where(s => s.FirstName == level1Students[index].FirstName &&
                    s.LastName == level1Students[index].LastName).FirstOrDefault());
                    students.RemoveAt(result);
                    // counter - 1 because student with level 1 is added to the group.
                    counter = counter - 1;
                    {
                        // Add remaining students randomly to the group on condition that they are not of level 1.
                        for (int i = 0; i < counter; i++)
                        {
                            int randomindex = random.Next(0, students.Count);
                            if (students[randomindex].Level == 1)
                            {
                                i--; // decrement i because to reset the loopcounter to the value before encoutering a level 1 student
                                continue;
                            }
                            else
                            {
                                studentsGroup.Add(students[randomindex]); // add student to the group
                                students.RemoveAt(randomindex); // remove the student from the main list                
                            }
                            //}
                            if (counter != 0)
                            {
                                await UpdateCSVStudentsAsync();
                                LoadGroup();
                                LoadStudents();
                            }
                        }
                    }
                }
            }
        }
        // Method updating CSV-file by removing the grouped students
        private async Task UpdateCSVStudentsAsync()
        {
            // Append = false, because generating renewed CSV-file of students 
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                foreach (var student in students)
                {
                    await writer.WriteLineAsync(student.Level + "," + student.FirstName + "," + student.LastName);
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

        // Click on btnWriteCSVGroupedStudents will write the grouped student to new CSV textfile for the specific group
        private async void btnWriteCSVGroupedStudents_Click(object sender, RoutedEventArgs e)
        {
            await WriteCSVGroupedStudentsAsync();
            ClearFields();
            lstGroup.Items.Clear();
            studentsGroup.Clear();
            LoadStudents();
        }
        // Method writing grouped students to new CSV-file
        private async Task WriteCSVGroupedStudentsAsync()
        {
            //groupnumber++;
            string groupdatehour = DateTime.Now.ToString("_yyyyMMdd-HHmmss");
            string file = Directory.GetCurrentDirectory() + "\\Groep_" + groupdatehour + ".txt";

            using (StreamWriter writer = new StreamWriter(file, true))
            {
                foreach (var student in studentsGroup)
                {
                    await writer.WriteLineAsync(student.Level + "," + student.FirstName + "," + student.LastName);
                }
                if (File.Exists(file))
                {
                    MessageBox.Show("CSV Groep van studenten succesvol geschreven!");
                }
                else
                {
                    MessageBox.Show("Fout!\nCSV kon niet worden geschreven!");
                }
            }
        }

        //-----------------------------------------------------------
        // HELP WINDOW
        //-----------------------------------------------------------
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            // opening child window of (this) main window for reading the helptext
            WindowHelp windowHelp = new WindowHelp(this);
            windowHelp.ShowDialog();
        }

        //---------------------------------------------
        // CLEAR FIELDS
        //---------------------------------------------

        // Method clearing the textboxes
        private void ClearFields()
        {
            txtLevel.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtGroupText.Text = string.Empty;
        }
    }
}