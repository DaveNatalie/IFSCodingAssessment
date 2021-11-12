using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveNatalie.IFS.CodingAssessment.Question4
{
    internal class WiringStateManager
    {


        List<WiringInstruction> instructions = new List<WiringInstruction>();

        private Dictionary<string, ushort> wireState = new ();

        int maxIterations = 100000;

        public WiringStateManager()
        {

        }



        /// <summary>
        /// Loads the instruction set from a text file
        /// </summary>
        public async Task LoadInstructionsAsync(string filename)
        {
            this.instructions = new List<WiringInstruction>();

            using (StreamReader streamReader = new StreamReader(filename))
            {
                while (!streamReader.EndOfStream)
                {
                    string? instructionText = await streamReader.ReadLineAsync();
                    if (instructionText != null)
                    {
                        //Wiring instructions are parsed from their string format to standard model that can more easily be processed
                        WiringInstruction wi = WiringInstruction.Parse(instructionText);
                        instructions.Add(wi);
                    }
                }
            }
        }



        /// <summary>
        /// This method is useful in debugging. It displays the current value for all loaded wire states
        /// </summary>
        public void DumpCurrentStateDictionary()
        {
            foreach (var item in wireState.OrderBy(x => x.Key))
            {
                Console.WriteLine($"{item.Key} : {item.Value}");
            }
        }

        /// <summary>
        /// If input is a numeric value, the numberic value will be returned. If input is non-numeric, a wire state matching the provided name will be returned.
        /// </summary>
        /// <param name="input">A numeric value, or wirestate name</param>
        /// <returns>If no value is available, will return null</returns>
        public ushort? GetWireStateOrValue(string input)
        {
            //Check to see if the input is numberic
            if (ushort.TryParse(input, out ushort val))
            {
                return val;
            }
            else //if input is not numeric, look to see if we know the current state of the wire
            {
                if (wireState.TryGetValue(input, out ushort state))
                {
                    return state;
                }
            }

            //If the input is not numeric, and we don't yet knwo the wire state
            return null;
        }


        /// <summary>
        /// If input is a numeric value, the numberic value will be provided as an out parameter. If input is non-numeric, a wire state matching the provided name will be provided as an out parameter
        /// </summary>
        /// <param name="input">A numeric value, or wirestate name</param>
        /// <param name="val">The value returned from GetWireStateOrValue</param>
        /// <returns>True if a value is available</returns>
        public bool TryGetWireStateOrValue(string input, out ushort val)
        {
            ushort? res = GetWireStateOrValue(input);
            if (res.HasValue)
            {
                val = res.Value;
                return true;
            }
            val = 0;
            return false;
        }



        /// <summary>
        /// Process the current instruction set. If an instruction set has not be loaded, no processing will occur.
        /// </summary>
        /// <returns>True if the instruction set was able to process successfully</returns>
        public bool Process()
        {
            if (instructions == null)
            {
                return false;
            }

            //The instruction set is processed repeatedly untill all instructions have been handled
            //If an instruction cannot be handled in the current state of the application, it will be skipped and handled in a future iteration

            //Keep track of how many times the instruction set has been iterated.
            int numberOfIterations = 0;

            while (instructions.Count > 0)
            {

                //We can't modify a collection as it is being interated
                //So we queue handled Instructions for removoval after interation
                var handledInstructions = new List<WiringInstruction>();

                foreach (WiringInstruction instruction in instructions)
                {
                    bool handled = ProcessInstruction(instruction);
                    if (handled)
                    {
                        handledInstructions.Add(instruction);
                    }
                }

                //Removed all handled instructions from the instruction set
                foreach (WiringInstruction handledInstruction in handledInstructions)
                {
                    instructions.Remove(handledInstruction);
                }

                //During development, not all instructions may be handled
                //This will cause an infinite loop, so we want to detect that, and exit gracefully
                numberOfIterations++;
                if (numberOfIterations > maxIterations)
                {
                    return false;
                }
            }

            //If we reach this point, it means every instruction has been handled
            return true;
        }


        /// <summary>
        /// Processes an individual instruction
        /// </summary>
        /// <param name="instruction">The instruction to process</param>
        /// <returns>True if the instruction can be handled. If false, the instruction should remain in the instruction pool.</returns>
        private bool ProcessInstruction(WiringInstruction instruction)
        {
            //Determines which operator method to call, and passes the appropriate input from the instruction model
            switch(instruction.Operator)
            {
                case "ASSIGN":
                    return ProcessAssignment(instruction.Input1, instruction.Output);

                case "NOT":
                    return ProcessNotOperator(instruction.Input1, instruction.Output);
                case "AND":
                    if (instruction.Input2 != null)
                    {
                        return ProcessAndOperator(instruction.Input1, instruction.Input2, instruction.Output);
                    }
                    break;
                case "OR":
                    if (instruction.Input2 != null)
                    {
                        return ProcessOrOperator(instruction.Input1, instruction.Input2, instruction.Output);
                    }
                    break;

                case "RSHIFT":
                    if (instruction.Input2 != null)
                    {
                        return ProcessRightShift(instruction.Input1, instruction.Input2, instruction.Output);
                    }
                    break;

                case "LSHIFT":
                    if (instruction.Input2 != null)
                    {
                        return ProcessLeftShift(instruction.Input1, instruction.Input2, instruction.Output);
                    }
                    break;
                default:
                    throw new NotImplementedException($"Operator is unsupported. {instruction.Operator}");
            }

            return false;
        }


        //ASSIGN
        private bool ProcessAssignment(string input, string output)
        {
            if (TryGetWireStateOrValue(input, out var val))
            {
                wireState[output] = val;
                return true;
            }
            return false;
        }

        //NOT
        private bool ProcessNotOperator(string input, string output)
        {
            if (TryGetWireStateOrValue(input, out var val))
            {
                wireState[output] = (ushort)~val; ;
                return true;
            }
            return false;
        }

        //AND
        private bool ProcessAndOperator(string input1, string input2, string output)
        {
            if (TryGetWireStateOrValue(input1, out var val1) && TryGetWireStateOrValue(input2, out var val2))
            {
                wireState[output] = (ushort)(val1 & val2);
                return true;
            }
            return false;
        }

        //OR
        private bool ProcessOrOperator(string input1, string input2, string output)
        {
            if (TryGetWireStateOrValue(input1, out var val1) && TryGetWireStateOrValue(input2, out var val2))
            {
                wireState[output] = (ushort)(val1 | val2);
                return true;
            }
            return false;
        }

        //RSHIFT
        private bool ProcessRightShift(string input, string shiftAmount, string output)
        {

            if (TryGetWireStateOrValue(input, out var val) && int.TryParse(shiftAmount, out int shift))
            {
                wireState[output] = (ushort)(val >> shift);
                return true;
            }
            return false;
        }

        //LSHIFT
        private bool ProcessLeftShift(string input, string shiftAmount, string output)
        {

            if (TryGetWireStateOrValue(input, out var val) && int.TryParse(shiftAmount, out int shift))
            {
                wireState[output] = (ushort)(val << shift);
                return true;
            }
            return false;
        }



    }
}
