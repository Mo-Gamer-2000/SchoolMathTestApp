using System;
using System.Collections.Generic;
using System.IO;

class MathTestApp
{
    static Random random = new Random(); // Initialize random number generator
    static HashSet<string> generatedQuestions = new HashSet<string>(); // Collection to store generated questions to avoid duplicates
    static string[] arithmeticOperators = { "+", "-", "*", "/" }; // Array of arithmetic operators
    static string studentName; // Variable to store student's name
    static bool hasLearningDifficulty; // Flag indicating if student has learning difficulty
    static TimeSpan defaultTestDuration = TimeSpan.FromMinutes(45); // Default duration of the test
    static TimeSpan extraTimeForDisability = TimeSpan.FromMinutes(15); // Additional time given for students with learning difficulty

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Moeez's Primary School Mathematics Test App!");

        // Ensure user enters their name
        while (string.IsNullOrEmpty(studentName))
        {
            Console.Write("Enter your full name: ");
            studentName = Console.ReadLine();

            if (string.IsNullOrEmpty(studentName))
            {
                Console.WriteLine("Please enter your full name.");
            }
        }

        // Ask if the student has a learning difficulty
        Console.Write("Do you have a learning difficulty? (yes/no): ");
        string response = Console.ReadLine();
        hasLearningDifficulty = response.ToLower() == "yes";

        // Calculate test duration
        TimeSpan testDuration = CalculateTestDuration();

        // Generate and present questions
        Dictionary<string, double> questionAnswers = new Dictionary<string, double>(); // Dictionary to store question-answer pairs
        List<string> questions = GenerateQuestions(); // Generate questions
        DateTime startTime = DateTime.Now; // Record start time of the test
        int correctAnswers = 0; // Counter for correct answers

        Dictionary<string, bool> questionResults = new Dictionary<string, bool>(); // Dictionary to store question-result pairs

        int questionNumber = 1;
        foreach (string question in questions)
        {
            bool isCorrect = PresentQuestion(questionNumber, question, out double userAnswer); // Present question to the user
            questionAnswers.Add($"{questionNumber}. {question}", userAnswer); // Store question-answer pair
            questionResults.Add($"{questionNumber}. {question}", isCorrect); // Store question-result pair
            if (isCorrect)
            {
                correctAnswers++; // Increment correct answer counter
            }
            questionNumber++; // Increment question number
        }

        // Calculate and display results and time taken
        DateTime endTime = DateTime.Now; // Record end time of the test
        TimeSpan elapsedTime = endTime - startTime; // Calculate elapsed time of the test

        Console.WriteLine($"Test completed in {elapsedTime.TotalMinutes} minutes/seconds."); // Display test completion time

        string testResult = DetermineTestResult(correctAnswers); // Determine test result based on correct answers
        Console.WriteLine($"Your test result: {testResult}"); // Display test result

        // Generate tutor report
        GenerateTutorReport(questionAnswers, questionResults, testResult, elapsedTime, testDuration); // Generate and save tutor report

        Console.WriteLine("Thank you for taking the test!"); // Display gratitude message
    }

    // Method to generate a list of questions
    static List<string> GenerateQuestions()
    {
        List<string> easyQuestions = GenerateEasyQuestions(); // Generate easy questions
        List<string> hardQuestions = GenerateHardQuestions(); // Generate hard questions

        List<string> questions = new List<string>();
        questions.AddRange(easyQuestions); // Add easy questions to the list
        questions.AddRange(hardQuestions); // Add hard questions to the list

        // Shuffle the questions to randomize them
        Shuffle(questions);

        return questions;
    }

    // Method to generate easy questions
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

    // Method to generate hard questions
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

    // Method to shuffle a list
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

    // Method to present a question to the user
    static bool PresentQuestion(int questionNumber, string question, out double userAnswer)
    {
        Console.WriteLine($"Question {questionNumber}: {question}"); // Display the question number and the question

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

    // Method to calculate the correct answer for a given question
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

    // Method to check if the user's answer is correct
    static bool CheckAnswer(double userAnswer, double correctAnswer)
    {
        // Check if the student's answer matches the correct answer within a small margin of error
        double epsilon = 0.0001;
        return Math.Abs(userAnswer - correctAnswer) < epsilon;
    }

    // Method to determine the test result based on the number of correct answers
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

    // Method to calculate the test duration considering any extra time for students with learning difficulty
    static TimeSpan CalculateTestDuration()
    {
        TimeSpan testDuration = defaultTestDuration;
        if (hasLearningDifficulty)
        {
            testDuration = testDuration.Add(extraTimeForDisability);
        }
        return testDuration;
    }

    // Method to generate and save a tutor report
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
            report += $"\n{question}\nAnswer: {userAnswer}\nResult: {result}\n";
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
