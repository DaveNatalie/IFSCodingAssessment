
using DaveNatalie.IFS.CodingAssessment;
using DaveNatalie.IFS.CodingAssessment.Question4;
using DaveNatalie.IFS.CodingAssessment.Question5;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("1. To what floor do the instructions take the worker?");
        Console.WriteLine(Question1());
        Console.WriteLine();

        Console.WriteLine("2. How many houses have their meters read at least once?");
        Console.WriteLine(Question2());
        Console.WriteLine();

        Console.WriteLine("3. How many strings are good?");
        Console.WriteLine(await Question3Async());
        Console.WriteLine();


        Console.WriteLine("4. Using the instructions (provided as your question input), what signal is ultimately provided to wire 'a'?");
        Console.WriteLine(await Question4Async());
        Console.WriteLine();


        Console.WriteLine("5. What is the Manhattan distance from the central port to the _closest_ intersection?");
        Console.WriteLine(await Question5Async());
        Console.WriteLine();

        Console.WriteLine("Done");
    }

    static string Question1()
    {
        int currentFloor = 0;
        using (StreamReader streamReader = new StreamReader("Files\\question01_input.txt"))
        {
            //Read the file one character at a time
            while (!streamReader.EndOfStream)
            {
                char c = (char)streamReader.Read();

                //Use pattern matching to determine the floor delta
                currentFloor += c switch
                {
                    '(' => 1,
                    ')' => -1,
                    _ => 0
                };
            }
        }
        return $"The worker is taken to floor: {currentFloor}";
    }


    static string Question2()
    {
        //Hashet will index it's values to allow for fast lookup using 'Contains'
        HashSet<string> usedAddresses = new HashSet<string>();

        using (StreamReader streamReader = new StreamReader("Files\\question02_input.txt"))
        {
            //Keep track of cartesion position as the dispatcher moves around the town
            int x = 0;
            int y = 0;

            //Read the file one character at a time
            while (!streamReader.EndOfStream)
            {
                char c = (char)streamReader.Read();

                switch (c)
                {
                    case '^':
                        y++;
                        break;
                    case 'v':
                        y--;
                        break;
                    case '>':
                        x++;
                        break;
                    case '<':
                        x--;
                        break;
                }

                //Create a string representation of the address
                string address = $"{x},{y}";

                //If the address is unique, add it to the usedAddress collection
                if (!usedAddresses.Contains(address))
                {
                    usedAddresses.Add(address);
                }

            }
        }


        //It's possible to add all addresses to a collection, and then count distinct values at the end
        //This would prevent multiple calls to Contains()
        //I didn't do any tests to see which approach was faster

        return $"The number of meters read, at least once, is: {usedAddresses.Count}";

    }

    static async Task<string> Question3Async()
    {
        //Setup three patterns to detect the word requirements
        //These patterns assume the input is all lowercase, but the code could be easily modified to be case insensative

        Regex threeVowelPattern = new Regex("(\\w*[aeiou]\\w*){3}");
        Regex doubleLetterPattern = new Regex("(\\w)\\1");
        Regex rejectedTermsPattern = new Regex("(ab|cd|pq|or|xy)");

        int goodStringCount = 0;

        using (StreamReader streamReader = new StreamReader("Files\\question03_input.txt"))
        {
            while (!streamReader.EndOfStream)
            {
                //Read one line at a time from the input file
                //The question doesn't state that each word would be on it's own line, but this solution assumes that they are

                string? word = await streamReader.ReadLineAsync();
                if (word != null)
                {
                    //Check againts each of the patterns
                    //I think if I spent more time on this, I may be able to do the detection with a single pattern
                    if (threeVowelPattern.IsMatch(word))
                    {
                        if (doubleLetterPattern.IsMatch(word))
                        {
                            if (!rejectedTermsPattern.IsMatch(word))
                            {
                                //If the string passes all three tests, incrememnt our counter
                                goodStringCount++;
                            }
                        }
                    }
                }
            }

        }
        return $"The number of good strings is: {goodStringCount}";
    }


    static async Task<string> Question4Async()
    {
        WiringStateManager wiring = new WiringStateManager();

        await wiring.LoadInstructionsAsync("Files\\question04_input.txt");

        bool foundSolution = wiring.Process();

        if (foundSolution && wiring.TryGetWireStateOrValue("a", out ushort a))
        {
            return $"The signal provided to 'a' is: {a}";
        }
        else
        {
#if DEBUG
            wiring.DumpCurrentStateDictionary();
#endif
            return $"The signal provided to 'a' could not be determined";
        }
    }


    static async Task<string> Question5Async()
    {

        int? minDistance = null;


        using (StreamReader streamReader = new StreamReader("Files\\question05_input.txt"))
        {

            //Load wire paths from input file
            string pathString1 = await streamReader.ReadLineAsync() ?? String.Empty;
            string pathString2 = await streamReader.ReadLineAsync() ?? String.Empty;

            WirePath path1 = WirePath.FromString(pathString1);
            WirePath path2 = WirePath.FromString(pathString2);


            //Check if path1 intersects with path2
            //If an intersection is found, the callback will contain the point of intersection
            path1.IntersectsAll(path2, (point) =>
            {
                var distance = point.ManhattanDistanceTo(Vector.Origin);

                //If the new distance found is closer than the current minimum, update the current minimum
                if (distance < minDistance.GetValueOrDefault(int.MaxValue))
                {
                    minDistance = distance;
                }
            });
        }
        return $"The Manhattan distance to the closest intersection is: { minDistance }";

    }

}