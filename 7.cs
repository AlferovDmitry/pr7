using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EducationSystem
{
    // Основные классы данных
    public class User
    {
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public User(string fullName, string login, string password)
        {
            FullName = fullName;
            Login = login;
            Password = password;
        }
    }

    public class Student : User
    {
        public string Group { get; set; }
        public List<Grade> Grades { get; set; } = new();

        public Student(string fullName, string login, string password, string group)
            : base(fullName, login, password)
        {
            Group = group;
        }

        public void ViewGrades()
        {
            Console.Clear();
            Console.WriteLine($"Оценки студента {FullName}:\n");
            foreach (var grade in Grades)
            {
                Console.WriteLine($"{grade.Subject}: {grade.Value} (Дата: {grade.Date:dd.MM.yyyy})");
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
    }

    public class Teacher : User
    {
        public List<string> Subjects { get; set; }
        public List<string> Groups { get; set; }

        public Teacher(string fullName, string login, string password, List<string> subjects, List<string> groups)
            : base(fullName, login, password)
        {
            Subjects = subjects;
            Groups = groups;
        }

        public void AddGrade(Student student, string subject, int gradeValue)
        {
            if (Subjects.Contains(subject) && Groups.Contains(student.Group))
            {
                student.Grades.Add(new Grade(subject, gradeValue));
                Console.WriteLine("Оценка успешно добавлена.");
            }
            else
            {
                Console.WriteLine("Недостаточно прав для выставления оценки.");
            }
        }
    }

    public class Admin : User
    {
        public Admin(string login, string password)
            : base("Администратор", login, password)
        {
        }

        public void ManageUsers<T>(List<T> users, Func<T> createUser) where T : User
        {
            Console.Clear();
            Console.WriteLine("1. Добавить\n2. Удалить\n3. Назад");
            var choice = Console.ReadKey(true).Key;
            if (choice == ConsoleKey.D1)
            {
                users.Add(createUser());
                Console.WriteLine("Пользователь добавлен.");
            }
            else if (choice == ConsoleKey.D2)
            {
                Console.Write("Введите логин для удаления: ");
                string login = Console.ReadLine();
                var user = users.FirstOrDefault(u => u.Login == login);
                if (user != null)
                {
                    users.Remove(user);
                    Console.WriteLine("Пользователь удалён.");
                }
                else
                {
                    Console.WriteLine("Пользователь не найден.");
                }
            }
            Console.ReadKey();
        }
    }

    public class Grade
    {
        public string Subject { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; }

        public Grade(string subject, int value)
        {
            Subject = subject;
            Value = value;
            Date = DateTime.Now;
        }
    }

    class Program
    {
        static List<Student> students = new();
        static List<Teacher> teachers = new();
        static Admin admin = new Admin("admin", "admin123");
        const string FilePath = "data.bin";

        static void Main()
        {
            LoadData();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Вход как студент\n2. Вход как преподаватель\n3. Вход как администратор\n4. Выйти");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        Login(students, student => student.ViewGrades());
                        break;
                    case ConsoleKey.D2:
                        Login(teachers, teacher => TeacherMenu((Teacher)teacher));
                        break;
                    case ConsoleKey.D3:
                        if (Authenticate(admin))
                        {
                            AdminMenu();
                        }
                        break;
                    case ConsoleKey.D4:
                        SaveData();
                        return;
                }
            }
        }

        static void Login<T>(List<T> users, Action<T> onSuccess) where T : User
        {
            Console.Clear();
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();
            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                onSuccess(user);
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль.");
                Console.ReadKey();
            }
        }

        static bool Authenticate(User user)
        {
            Console.Clear();
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();
            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();
            return user.Login == login && user.Password == password;
        }

        static void AdminMenu()
        {
            Console.Clear();
            Console.WriteLine("1. Управление студентами\n2. Управление преподавателями\n3. Назад");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    admin.ManageUsers(students, CreateStudent);
                    break;
                case ConsoleKey.D2:
                    admin.ManageUsers(teachers, CreateTeacher);
                    break;
            }
        }

        static void TeacherMenu(Teacher teacher)
        {
            Console.Clear();
            Console.WriteLine("1. Добавить оценку студенту\n2. Назад");
            if (Console.ReadKey(true).Key == ConsoleKey.D1)
            {
                Console.Write("Введите логин студента: ");
                string login = Console.ReadLine();
                var student = students.FirstOrDefault(s => s.Login == login);
                if (student != null)
                {
                    Console.Write("Введите предмет: ");
                    string subject = Console.ReadLine();
                    Console.Write("Введите оценку (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out int gradeValue) && gradeValue >= 1 && gradeValue <= 5)
                    {
                        teacher.AddGrade(student, subject, gradeValue);
                    }
                    else
                    {
                        Console.WriteLine("Некорректное значение оценки.");
                    }
                }
                else
                {
                    Console.WriteLine("Студент не найден.");
                }
                Console.ReadKey();
            }
        }

        static Student CreateStudent()
        {
            Console.Clear();
            Console.Write("ФИО: ");
            string fullName = Console.ReadLine();
            Console.Write("Логин: ");
            string login = Console.ReadLine();
            Console.Write("Пароль: ");
            string password = Console.ReadLine();
            Console.Write("Группа: ");
            string group = Console.ReadLine();
            return new Student(fullName, login, password, group);
        }

        static Teacher CreateTeacher()
        {
            Console.Clear();
            Console.Write("ФИО: ");
            string fullName = Console.ReadLine();
            Console.Write("Логин: ");
            string login = Console.ReadLine();
            Console.Write("Пароль: ");
            string password = Console.ReadLine();
            Console.Write("Дисциплины (через запятую): ");
            var subjects = Console.ReadLine().Split(',').Select(s => s.Trim()).ToList();
            Console.Write("Группы (через запятую): ");
            var groups = Console.ReadLine().Split(',').Select(g => g.Trim()).ToList();
            return new Teacher(fullName, login, password, subjects, groups);
        }

        static void SaveData()
        {
            using var writer = new BinaryWriter(File.Open(FilePath, FileMode.Create));
            writer.Write(students.Count);
            foreach (var student in students)
            {
                writer.Write(student.FullName);
                writer.Write(student.Login);
                writer.Write(student.Password);
                writer.Write(student.Group);
                writer.Write(student.Grades.Count);
                foreach (var grade in student.Grades)
                {
                    writer.Write(grade.Subject);
                    writer.Write(grade.Value);
                    writer.Write(grade.Date.ToBinary());
                }
            }
            writer.Write(teachers.Count);
            foreach (var teacher in teachers)
            {
                writer.Write(teacher.FullName);
                writer.Write(teacher.Login);
                writer.Write(teacher.Password);
                writer.Write(string.Join(",", teacher.Subjects));
                writer.Write(string.Join(",", teacher.Groups));
            }
        }

        static void LoadData()
        {
            if (!File.Exists(FilePath)) return;

            using var reader = new BinaryReader(File.Open(FilePath, FileMode.Open));
            int studentCount = reader.ReadInt32();
            for (int i = 0; i < studentCount; i++)
            {
                var student = new Student(reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString());
                int gradeCount = reader.ReadInt32();
                for (int j = 0; j < gradeCount; j++)
                {
                    student.Grades.Add(new Grade(reader.ReadString(), reader.ReadInt32)
                    {
                        Date = DateTime.FromBinary(reader.ReadInt64())
                    });
                }
                students.Add(student);
            }
            int teacherCount = reader.ReadInt32();
            for (int i = 0; i < teacherCount; i++)
            {
                teachers.Add(new Teacher(
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString().Split(',').ToList(),
                    reader.ReadString().Split(',').ToList()));
            }
        }
    }
}