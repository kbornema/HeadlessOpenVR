using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeadlessSteamVRSetup
{
    public class FileLineChangeRequest
    {
        public string InputLine;
        public string OutputLine;

        public FileLineChangeRequest()
        {

        }

        public FileLineChangeRequest(string input, string output)
        {
            InputLine = input;
            OutputLine = output;
        }

        public void SetInputLine(string input)
        {
            InputLine = input;
        }

        public void SetOutputLine(string output)
        {
            OutputLine = output;
        }

        public bool Apply(string path, ref string[] lines, bool makeBackup)
        {
            if(string.IsNullOrEmpty(InputLine) || string.IsNullOrEmpty(OutputLine))
            {
                Console.WriteLine("A line string was null/empty. Skipping.");
                return false;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                var curLine = lines[i];

                if(curLine.Contains(InputLine))
                {
                    if(makeBackup)
                    {
                        FileUtil.MakeBackup(path);
                        makeBackup = false;
                    }

                    lines[i] = curLine.Replace(InputLine, OutputLine);
                    return true;
                }
            }

            return false;
        }
    }
}
