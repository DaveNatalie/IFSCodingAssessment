using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveNatalie.IFS.CodingAssessment.Question4
{
    internal class WiringInstruction
    {
        public string Input1 { get; set; }

        public string? Input2 { get; }


        public string Operator { get; set; }

        public string Output { get; set; }

        public string? OriginalString { get; set; }


        public WiringInstruction(string op, string output, string input1, string? input2 = null)
        {
            this.Operator = op;
            this.Output = output;
            this.Input1 = input1;
            this.Input2 = input2;
        }


        public static WiringInstruction Parse(string s)
        {
           
            string[] instructionPieces = s.Split(' ');

            switch(instructionPieces.Length)
            {
                case 3: //Assignment
                    return new WiringInstruction("ASSIGN", instructionPieces[2], instructionPieces[0])
                    {
                        OriginalString = s
                    };
                case 4: //Single Input
                    return new WiringInstruction(instructionPieces[0], instructionPieces[3], instructionPieces[1])
                    {
                        OriginalString = s
                    };
                case 5: //Operation
                    return new WiringInstruction(instructionPieces[1], instructionPieces[4], instructionPieces[0], instructionPieces[2])
                    {
                        OriginalString = s
                    };

                default:
                    break;
            }

            return new WiringInstruction("NOP", "0", "0")
            {
                OriginalString = s
            };

        }




    }


}
