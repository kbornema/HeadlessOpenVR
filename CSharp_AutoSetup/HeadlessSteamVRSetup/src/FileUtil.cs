using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HeadlessSteamVRSetup
{
    public static class FileUtil
    {
        public static void MakeBackup(string file)
        {
            if(File.Exists(file))
            {
                int count = 0;
                string backUpFilePath;
                do
                {
                    backUpFilePath = file + "_backup_" + count;
                    count++;

                } while (File.Exists(backUpFilePath));

                File.Copy(file, backUpFilePath);
                Console.WriteLine("Made backup of " + file);
            }
        }
    }
}
