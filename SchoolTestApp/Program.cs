using System;
using System.Collections.Generic;
using System.IO;

class MathTestApp
{
    static Random random = new Random();
    static HashSet<string> generatedQuestions = new HashSet<string>();
    static string[] arithmeticOperators = { "+", "-", "*", "/" };
    static string studentName;
    static bool hasLearningDifficulty;
    static TimeSpan defaultTestDuration = TimeSpan.FromMinutes(45);
    static TimeSpan extraTimeForDisability = TimeSpan.FromMinutes(15);

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Moeez's Primary School Mathematics Test App!");

        // Prompt student for name
        Console.Write("Enter your name: ");
        studentName = Console.ReadLine();

        // Ask if the student has a learning difficulty
        Console.Write("Do you have a learning difficulty? (yes/no): ");
        string response = Console.ReadLine();
        hasLearningDifficulty = response.ToLower() == "yes";

        // Calculate test duration
        TimeSpan testDuration = CalculateTestDuration();

        // Generate and present questions
        Dictionary<string, double> questionAnswers = new Dictionary<string, double>();
        List<string> questions = GenerateQuestions();
        DateTime startTime = DateTime.Now;
        int correctAnswers = 0;

        Dictionary<string, bool> questionResults = new Dictionary<string, bool>();

        foreach (string question in questions)
        {
            bool isCorrect = PresentQuestion(question, out double userAnswer);
            questionAnswers.Add(question, userAnswer);
            questionResults.Add(question, isCorrect);
            if (isCorrect)
            {
                correctAnswers++;
            }
        }

        // Calculate and display results and time taken
        DateTime endTime = DateTime.Now;
        TimeSpan elapsedTime = endTime - startTime;

        Console.WriteLine($"Test completed in {elapsedTime.TotalMinutes} minutes/seconds.");

        string testResult = DetermineTestResult(correctAnswers);
        Console.WriteLine($"Your test result: {testResult}");

        // Generate tutor report
        GenerateTutorReport(questionAnswers, questionResults, testResult, elapsedTime, testDuration);

        Console.WriteLine("Thank you for taking the test!");
    }

    static List<string> GenerateQuestions()
    {
        List<string> easyQuestions = GenerateEasyQuestions();
        List<string> hardQuestions = GenerateHardQuestions();

        List<string> questions = new List<string>();
        questions.AddRange(easyQuestions);
        questions.AddRange(hardQuestions);

        // Shuffle the questions to randomise them
        Shuffle(questions);

        return questions;
    }

    static List<string> GenerateEasyQuestions()
    {
        List<string> easyQuestions = new List<string>();

        // Generate 10 unique easy questions
        while (easyQuestions.Count < 10)
        {
            int operand1 = random.Next(1, 26); // Limit operands to 26 for easy questions
            int operand2 = random.Next(1, 26);
            string arithmeticOperator = arithmeticOperators[random.Next(0, 2)]; // Only addition and subtraction for easy questions

            string question = $"{operand1} {arithmeticOperator} {operand2} = ?";
            if (!generatedQuestions.Contains(question))
            {
                generatedQuestions.Add(question);
                easyQuestions.Add(question);
            }
        }

        return easyQuestions;
    }

    static List<string> GenerateHardQuestions()
    {
        List<string> hardQuestions = new List<string>();

        // Generate 10 unique hard questions
        while (hardQuestions.Count < 10)
        {
            int operand1 = random.Next(1, 51); // Increase operands range for hard questions
            int operand2 = random.Next(1, 51);
            string arithmeticOperator = arithmeticOperators[random.Next(0, 4)]; // Allow all arithmetic operators for hard questions

            string question = $"{operand1} {arithmeticOperator} {operand2} = ?";
            if (!generatedQuestions.Contains(question))
            {
                generatedQuestions.Add(question);
                hardQuestions.Add(question);
            }
        }

        return hardQuestions;
    }

    static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    static bool PresentQuestion(string question, out double userAnswer)
    {
        Console.WriteLine(question);

        bool validAnswer = false;
        userAnswer = 0;

        while (!validAnswer)
        {
            Console.Write("Enter your answer: ");
            string input = Console.ReadLine();

            // Check if input is a valid integer
            if (double.TryParse(input, out userAnswer))
            {
                validAnswer = true;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }

        // Validate student answer
        double correctAnswer = CalculateAnswer(question);
        bool isCorrect = CheckAnswer(userAnswer, correctAnswer);

        return isCorrect;
    }

    static double CalculateAnswer(string question)
    {
        // Extract operands and operator from the question
        string[] tokens = question.Split(' ');
        double operand1 = Convert.ToDouble(tokens[0]);
        double operand2 = Convert.ToDouble(tokens[2]);
        string arithmeticOperator = tokens[1];

        // Calculate the correct answer
        double answer = 0;
        switch (arithmeticOperator)
        {
            case "+":
                answer = operand1 + operand2;
                break;
            case "-":
                answer = operand1 - operand2;
                break;
            case "*":
                answer = operand1 * operand2;
                break;
            case "/":
                answer = operand1 / operand2;
                break;
        }

        return answer;
    }

    static bool CheckAnswer(double userAnswer, double correctAnswer)
    {
        // Check if the student's answer matches the correct answer within a small margin of error
        double epsilon = 0.0001;
        return Math.Abs(userAnswer - correctAnswer) < epsilon;
    }

    static string DetermineTestResult(int correctAnswers)
    {
        string testResult;
        if (correctAnswers <= 4)
            testResult = "Failed";
        else if (correctAnswers <= 10)
            testResult = "Pass";
        else if (correctAnswers <= 15)
            testResult = "Merit";
        else
            testResult = "Distinction";

        return testResult;
    }

    static TimeSpan CalculateTestDuration()
    {
        TimeSpan testDuration = defaultTestDuration;
        if (hasLearningDifficulty)
        {
            testDuration = testDuration.Add(extraTimeForDisability);
        }
        return testDuration;
    }

    static void GenerateTutorReport(Dictionary<string, double> questionAnswers, Dictionary<string, bool> questionResults, string testResult, TimeSpan elapsedTime, TimeSpan testDuration)
    {
        // Generate tutor report containing student name, question details, results, and test duration time
        string report = $"--- Math Test Report for {studentName} ---\n\n";
        report += $"Learning Difficulty: {(hasLearningDifficulty ? "Yes (extra 15 minutes given)" : "No")}\n\n";
        report += "Questions:\n";

        foreach (var kvp in questionResults)
        {
            string question = kvp.Key;
            bool isCorrect = kvp.Value;
            string result = isCorrect ? "Correct" : "Incorrect";
            double userAnswer = questionAnswers[question];
            report += $"\nQuestion: {question}\nAnswer: {userAnswer}\nResult: {result}\n";
        }

        report += $"\nTest Result: {testResult}\n";
        report += $"Test Duration: {testDuration.TotalMinutes} minutes\n";
        report += $"Elapsed Time: {elapsedTime.TotalMinutes} minutes\n";

        // Write report to file
        string fileName = $"{studentName}_report.txt";
        File.WriteAllText(fileName, report);
        Console.WriteLine($"Tutor report saved as '{fileName}'.");
    }

}