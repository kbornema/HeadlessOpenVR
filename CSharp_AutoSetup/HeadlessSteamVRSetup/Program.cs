using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HeadlessSteamVRSetup
{
    class Program
    {
        private const bool MAKE_BACKUPS = true;

        private static FileChangeRequest CreateNullDriverSettingsRequest()
        {
            string path = "C:/Program Files (x86)/Steam/steamapps/common/SteamVR/drivers/null/resources/settings/default.vrsettings";
            FileChangeRequest request = new FileChangeRequest();
            request.SetTargetFile(path);

            var lineRequest = new FileLineChangeRequest();
            lineRequest.SetInputLine("\"enable\": false,");
            lineRequest.SetOutputLine("\"enable\": true,");
            request.AddLineChanger(lineRequest);

            return request;
        }

        private static FileChangeRequest CreateResourcesSettingsRequest()
        {
            string path = "C:/Program Files (x86)/Steam/steamapps/common/SteamVR/resources/settings/default.vrsettings";
            FileChangeRequest request = new FileChangeRequest();
            request.SetTargetFile(path);

            var requireHmdRequest = new FileLineChangeRequest();
            requireHmdRequest.SetInputLine("\"requireHmd\": true,");
            requireHmdRequest.SetOutputLine("\"requireHmd\": false,");
            request.AddLineChanger(requireHmdRequest);

            var forcedDriverRequest = new FileLineChangeRequest();
            forcedDriverRequest.SetInputLine("\"forcedDriver\": \"\",");
            forcedDriverRequest.SetOutputLine("\"forcedDriver\": \"null\",");
            request.AddLineChanger(forcedDriverRequest);

            var multipleDriversRequest = new FileLineChangeRequest();
            multipleDriversRequest.SetInputLine("\"activateMultipleDrivers\": false,");
            multipleDriversRequest.SetOutputLine("\"activateMultipleDrivers\": true,");
            request.AddLineChanger(multipleDriversRequest);

            return request;
        }

        static void Main(string[] args)
        {
            List<FileChangeRequest> requests = new List<FileChangeRequest>
            {
                //1
                CreateResourcesSettingsRequest(),

                //4
                CreateNullDriverSettingsRequest()

            };

            for (int i = 0; i < requests.Count; i++)
                requests[i].Run(MAKE_BACKUPS);
        }
    }
}
