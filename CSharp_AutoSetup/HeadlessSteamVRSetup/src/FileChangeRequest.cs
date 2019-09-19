using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HeadlessSteamVRSetup
{
    public class FileChangeRequest
    {
        private string _filePath;
        private List<FileLineChangeRequest> _lineChanges;

        public FileChangeRequest()
        {
            _lineChanges = new List<FileLineChangeRequest>();
        }

        public void SetTargetFile(string filePath)
        {
            _filePath = filePath;
        }

        public void AddLineChanger(FileLineChangeRequest lineChange)
        {
            _lineChanges.Add(lineChange);
        }

        public void Run(bool makeBackup)
        {
            if(!File.Exists(_filePath))
            {
                Console.WriteLine("File at " + _filePath + " was not found!");
                return;
            }

            int lineChanges = 0;
            string[] lines = File.ReadAllLines(_filePath);

            for (int i = 0; i < _lineChanges.Count; i++)
            {
                if(_lineChanges[i].Apply(_filePath, ref lines, makeBackup))
                {
                    lineChanges++;
                }

            }

            Console.WriteLine("Changed " + lineChanges + " on file " + _filePath);

            if(lineChanges > 0)
                File.WriteAllLines(_filePath, lines);
        }
    }
}
