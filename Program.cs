using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable
 
// Data Access Layer (DAL)
public class TimeTrackingDbContext
{
    public List<User> Users { get; set; } = new List<User>();
    public List<Project> Projects { get; set; } = new List<Project>();
    public List<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}

public class User
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class Project
{
    public string Name { get; set; } = "";
}

public class TimeEntry
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Description { get; set; } = "";
    public User? User { get; set; }
    public Project? Project { get; set; }
}

// Business Logic Layer (BLL)
public class TimeTrackingManager
{
    private readonly TimeTrackingDbContext dbContext;

    public TimeTrackingManager(TimeTrackingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public bool RegisterUser(string username, string password)
    {
        if (dbContext.Users.Any(u => u.Username == username))
        {
            Console.WriteLine("Username already exists. Choose a different one.");
            return false;
        }

        dbContext.Users.Add(new User { Username = username, Password = password });
        Console.WriteLine("User registered successfully.");
        return true;
    }

    public User? Login(string username, string password)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user == null)
        {
            Console.WriteLine("Invalid username or password.");
        }
        return user;
    }

    public void CreateProject(string projectName)
    {
        dbContext.Projects.Add(new Project { Name = projectName });
        Console.WriteLine($"Project '{projectName}' created successfully.");
    }

    public void StartTrackingTime(User? user, Project? project, string description)
    {
        if (user == null || project == null)
        {
            Console.WriteLine("User or project not found.");
            return;
        }

        var startTime = DateTime.Now;
        dbContext.TimeEntries.Add(new TimeEntry { User = user, Project = project, StartTime = startTime, Description = description });
        Console.WriteLine($"Time tracking started at {startTime} for project '{project.Name}'.");
    }

    public void StopTrackingTime(User? user)
    {
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        var activeEntry = dbContext.TimeEntries.FirstOrDefault(te => te.User == user && te.EndTime == null);
        if (activeEntry != null)
        {
            activeEntry.EndTime = DateTime.Now;
            Console.WriteLine($"Time tracking stopped at {activeEntry.EndTime} for project '{activeEntry.Project?.Name}'.");
        }
        else
        {
            Console.WriteLine("No active time tracking found.");
        }
    }

    public void GenerateReport(User? user)
    {
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        var userEntries = dbContext.TimeEntries.Where(te => te.User == user).ToList();
        Console.WriteLine($"Report for {user.Username}:");
        foreach (var entry in userEntries)
        {
            Console.WriteLine($"Project: {entry.Project?.Name}, Description: {entry.Description}, Duration: {(entry.EndTime - entry.StartTime).Value}");
        }
    }
}


// Presentation Layer
class Program
{
    static void Main()
    {
        var dbContext = new TimeTrackingDbContext();
        var timeTrackingManager = new TimeTrackingManager(dbContext);

        Console.WriteLine("Welcome to the Time Tracking App!");
        

        while (true)
        {
            Console.WriteLine("\n1. Register\n2. Login\n3. Create Project\n4. Start Time Tracking\n5. Stop Time Tracking\n6. Generate Report\n0. Exit");
            Console.Write("Select an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter username: ");
                    var regUsername = Console.ReadLine();
                    Console.Write("Enter password: ");
                    var regPassword = Console.ReadLine();
                    timeTrackingManager.RegisterUser(regUsername, regPassword);
                    break;

                case "2":
                    Console.Write("Enter username: ");
                    var loginUsername = Console.ReadLine();
                    Console.Write("Enter password: ");
                    var loginPassword = Console.ReadLine();
                    var loggedInUser = timeTrackingManager.Login(loginUsername, loginPassword);

                    if (loggedInUser != null)
                    {
                        Console.WriteLine($"Welcome, {loggedInUser.Username}!");
                        Console.WriteLine("Logged in successfully.");

                        while (true)
                        {
                            Console.WriteLine("\n1. Create Project\n2. Start Time Tracking\n3. Stop Time Tracking\n4. Generate Report\n0. Logout");
                            Console.Write("Select an option: ");
                            var userChoice = Console.ReadLine();

                            switch (userChoice)
                            {
                                case "1":
                                    Console.Write("Enter project name: ");
                                    var newprojectName = Console.ReadLine();
                                    timeTrackingManager.
                                    CreateProject(newprojectName);
                                    break;

                                case "2":
                                    Console.Write("Enter project name: ");
                                    var startProjectName = Console.ReadLine();
                                    Console.Write("Enter time entry description: ");
                                    var description = Console.ReadLine();
                                    var projectToStart = dbContext.Projects.FirstOrDefault(p => p.Name == startProjectName);
                                    if (projectToStart != null)
                                    {
                                        timeTrackingManager.StartTrackingTime(loggedInUser, projectToStart, description);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Project not found.");
                                    }
                                    break;

                                case "3":
                                    timeTrackingManager.StopTrackingTime(loggedInUser);
                                    break;

                                case "4":
                                    timeTrackingManager.GenerateReport(loggedInUser);
                                    break;

                                case "0":
                                    Console.WriteLine("Logged out successfully.");
                                    break;

                                default:
                                    Console.WriteLine("Invalid option.");
                                    break;
                            }

                            if (userChoice == "0")
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Login failed. Please try again.");
                    }
                    break;

                case "3":
                    Console.Write("Enter project name: ");
                    var projectName = Console.ReadLine();
                    timeTrackingManager.CreateProject(projectName);
                    break;

                case "4":
                    Console.WriteLine("Please login to start time tracking.");
                    break;

                case "5":
                    Console.WriteLine("Please login to stop time tracking.");
                    break;

                case "6":
                    Console.WriteLine("Please login to generate a report.");
                    break;

                case "0":
                    Console.WriteLine("Exiting application.");
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}