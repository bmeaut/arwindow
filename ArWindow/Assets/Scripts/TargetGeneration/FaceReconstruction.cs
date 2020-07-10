using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.TargetGeneration
{
    public class FaceReconstruction
    {
        //TODO: From configuration
        public string PythonPath { get; set; } = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python37_64\python.exe";
        public string ScriptPath { get; set; } = @"d:\docszoli\arwindow\Deep3DFaceReconstruction\";

        protected Process process;
        protected StreamWriter sw;
        protected StreamReader sr;

        public void StartProcess()
        {
            /*if(process != null && process is running)
             {
                process.Close/Kill();
                process.WaitForExit();
             }*/
            process = Process.Start(new ProcessStartInfo
            {
                FileName = PythonPath,
                Arguments = "demo.py",
                CreateNoWindow = false,
                UseShellExecute = false,
                WorkingDirectory = ScriptPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            });

            sw = process.StandardInput;
            sr = process.StandardOutput;
        }

        public void StopProcess()
        {
            sw.Close();
            sr.Close();
            process.Close();
        }

        public void Reconstruct()
        {
            //memory map input image

            //find 5 landmarks with opencv

            //send pipe message to python process for reconstruction

            //wait pipe result with vertices (+texture?)
        }
    }
}
