﻿namespace Random_Selector.Models
{
    public class Student
    {
        //private static int _id = 0;
        //public static int Id 
        //{ 
        //    get { return _id; }
        //    set { _id = _id + 1; } 
        //}
        public int Level { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{Level} {FirstName} {LastName}";
        }
    }
}
