﻿using static System.Collections.Specialized.BitVector32;
using System;
using System.Xml;




namespace Templist
{
    class Program
    {
        // define the path to the XML document
        public static string xmlPath = $"{Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName}/tasks.xml";

        static void Main(string[] args)
        {
            // check if the xml file exists, create one if it does not
            if (File.Exists(xmlPath) == false)
            {
                MakeTaskFile();
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            // initialize task count to the number of existing tasks in XML file
            int taskCount = 0;
            XmlNodeList elemList = xmlDoc.GetElementsByTagName("TASK");
            for (int i=0; i < elemList.Count; i++)
            {
                taskCount++;
            }

            string operations = "1: Add\n2: Complete Task\n3: Remove Task\n4: Show Tasks\n5: Quit\n: ";

            // initialize exit condition for main loop
            bool exit = false;

            while (exit == false)
            {
                // print options to user
                Console.Write(operations);
                int selectInt = 0;
                // prompt user until valid input is provided
                while (true)
                {
                    string? selectStr = Console.ReadLine();
                    // if input is valid, assign integer to selectInt and exit
                    if (int.TryParse(selectStr, out selectInt) && int.Parse(selectStr) > 0 && int.Parse(selectStr) <= 5)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please input an integer between 1 and 5");
                        Console.Write(operations);
                    }
                }

                // choose operation to perform based on user's input
                switch (selectInt)
                {
                    // add task
                    case 1:
                        string userTask = "";
                        // prompt user until task name is not blank
                        while (userTask == "")
                        {
                            Console.Write("Task name: ");
                            userTask = Console.ReadLine();
                            // create new task with user input as the TaskName
                        }
                        AddTask(userTask);
                        taskCount++;
                        break;

                    // mark a task complete (x in [])
                    case 2:
                        // print numbered task list
                        xmlDoc.Load(xmlPath);
                        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                        {
                            //string task = node.InnerText;
                            Console.WriteLine(node.Attributes["id"].Value + " " + node.InnerText);
                        }
                        // initialize integer to store input string after int conversion
                        int taskNum = 0;

                        // prompt user until valid input is provided
                        while (true)
                        {
                            Console.Write("Enter task number: ");
                            string? taskStr = Console.ReadLine();

                            // if input is valid, output integer to taskNum and exit
                            if (int.TryParse(taskStr, out taskNum) && int.Parse(taskStr) > 0 && int.Parse(taskStr) <= taskCount)
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"Please input an integer between 1 and {taskCount}");
                            }
                        }

                        // check off the selected task
                        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                        {
                            //string task = node.InnerText;
                            if (node.Attributes["id"].Value == $"{taskNum}")
                            {
                                Console.WriteLine(node.Attributes["id"].Value);
                                node.Attributes["completion"].Value = "complete";
                            }
                        }
                        xmlDoc.Save(xmlPath);
                        break;
                    case 3:
                        // TODO: Update ID values after task is removed
                        xmlDoc.Load(xmlPath);
                        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                        {
                            //string task = node.InnerText;
                            Console.WriteLine(node.Attributes["id"].Value + " " + node.InnerText);
                        }

                        // prompt user until valid input is provided
                        while (true)
                        {
                            Console.Write("Enter task number: ");
                            string? taskStr = Console.ReadLine();

                            // if input is valid, output integer to taskNum and exit
                            if (int.TryParse(taskStr, out taskNum) && int.Parse(taskStr) > 0 && int.Parse(taskStr) <= taskCount)
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"Please input an integer between 1 and {taskCount}");
                            }
                        }
                        var removedTask = xmlDoc.SelectSingleNode($"//*[@id='{taskNum}']");
                        xmlDoc["TASKS"].RemoveChild(removedTask);

                        // correct task ID numbers
                        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                        {
                            int taskID = Convert.ToInt16(node.Attributes["id"].Value);
                            // select all nodes with IDs greater than removed node and decrement them
                            if (taskID > taskNum) {
                                taskID--;
                                node.Attributes["id"].Value = $"{taskID}";
                            }
                        }
                        xmlDoc.Save(xmlPath);
                        Console.WriteLine("Task removed.");
                        break;
                    case 4:
                        ShowList(xmlDoc);
                        Console.Write("Press enter to continue:");
                        Console.ReadLine();
                        break;

                    // exit the program and print your final list
                    case 5:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Please input one of the characters provided.");
                        break;
                }
            }

            // print the final list
            ShowList(xmlDoc);
            // leave list displayed until key press
            Console.ReadLine();
        }

        static void ShowList(XmlDocument xmlDoc)
        {
            xmlDoc.Load(xmlPath);
            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                if (node.Attributes["completion"].Value == "complete")
                {
                    Console.WriteLine("[x]" + node.InnerText);
                }
                else
                {
                    Console.WriteLine("[ ]" + node.InnerText);
                }
            }
            return;
        }

        static void MakeTaskFile()
        {
            // define a new xml document, taskList
            XmlDocument taskList = new XmlDocument();

            // create the root element in the new file
            XmlElement root = taskList.CreateElement("TASKS");
            taskList.AppendChild(root);

            // save the xml document in the directory specified above
            taskList.Save(xmlPath);
            //Console.WriteLine(taskList.InnerXml);
        }

        static void AddTask(string taskInput)
        {
            XmlDocument taskList = new XmlDocument();

            taskList.Load(xmlPath);
            XmlNode root = taskList.SelectSingleNode("TASKS");
            XmlElement task = taskList.CreateElement("TASK");
            root.AppendChild(task);

            XmlAttribute id = taskList.CreateAttribute("id");
            id.Value = taskList.SelectNodes("TASKS/TASK").Count.ToString();
            task.Attributes.Append(id);

            XmlAttribute completion = taskList.CreateAttribute("completion");
            completion.Value = "incomplete";
            task.Attributes.Append(completion);

            XmlElement taskName = taskList.CreateElement("TASKNAME");
            taskName.InnerText = taskInput;
            task.AppendChild(taskName);
            taskList.Save(xmlPath);
            Console.WriteLine("Task added successfully.");
        }
    }


    public class Tasks
    {
        public string TaskName { get; set; }
        public bool IsComplete = false;

        // allow new tasks to be provided a string for their name
        public Tasks(string taskName)
        {
            TaskName = taskName;
        }
    }
}