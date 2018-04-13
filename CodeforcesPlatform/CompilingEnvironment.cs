﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeforcesPlatform
{
    public static class CompilingEnvironment
    {
        public abstract class CompilingLanguage
        {
            protected const string BASE_DIRECTORY = "./temp/";
            protected Process process = null;               // 程序执行进程
            protected TimeSpan excuteTotalTime;             // 程序执行时间
            protected string sourceCodeText;                // 程序源代码
            protected string standardInputText;             // 程序输入数据
            protected string standardOutputText;            // 程序输出数据
            protected string standardErrorText;             // 程序错误流数据
            private static volatile uint nextFileId;                     // 文件命名 ID

            public TimeSpan ExcuteTotalTime => excuteTotalTime;

            public string SourceCodeText => sourceCodeText;

            public string StandardInputText => standardInputText;

            public string StandardOutputText => standardOutputText;

            public string StandardErrorText => standardErrorText;
            public abstract string Execute(string sourceCodeText);
            public abstract string Execute(string sourceCodeText, string inputText);
            protected virtual void Initialization(string sourceCodeText, string inputText)
            {
                this.sourceCodeText = sourceCodeText;
                this.standardInputText = inputText;
            }

            protected uint GetNextFileId()
            {
                return nextFileId++;
            }
        }

        public class GNUCompiler : CompilingLanguage
        {
            private string filePath;    // 源程序路径

            /// <summary>
            /// 将源程序写入文件，初始化进程参数
            /// </summary>
            /// <param name="sourceCodeText"></param>
            /// <param name="inputText"></param>
            protected override void Initialization(string sourceCodeText, string inputText)
            {
                base.Initialization(sourceCodeText, inputText);

                if (!Directory.Exists(BASE_DIRECTORY)) Directory.CreateDirectory(BASE_DIRECTORY);   // 检测是否含有 temp 文件夹

                filePath = BASE_DIRECTORY + "cpp_" + GetNextFileId().ToString() + ".cpp";   // 将源代码写入文件
                var streamWriter = File.CreateText(filePath);
                streamWriter.WriteLine(sourceCodeText);
                streamWriter.Close();

                try
                {
                    process = new Process();
                    process.StartInfo.UseShellExecute = false;          // 是否使用 Shell
                    process.StartInfo.CreateNoWindow = true;            // 是否在新窗口中启动该进程的值
                    process.StartInfo.RedirectStandardInput = true;     // 重定向输入流
                    process.StartInfo.RedirectStandardOutput = true;    // 重定向输出流
                    process.StartInfo.RedirectStandardError = true;     // 重定向错误流
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 编译源代码，生成可执行程序
            /// </summary>
            private void ProgramCompilation()
            {
                process.StartInfo.FileName = "g++";
                process.StartInfo.Arguments = string.Format("{0} -pipe -O2 -DONLINE_JUDGE -std=c++14 -o {0}.exe", filePath);
                process.Start();
                process.WaitForExit();
            }

            public override string Execute(string sourceCodeText, string inputText)
            {
                try
                {
                    Initialization(sourceCodeText, inputText);      // 初始化参数
                    ProgramCompilation();                           // 编译

                    process.StartInfo.FileName = filePath + ".exe"; // 执行
                    process.StartInfo.Arguments = "";
                    process.Start();

                    process.StandardInput.Flush();
                    process.StandardInput.WriteLine(standardInputText);     // 写入输入数据

                    standardOutputText = process.StandardOutput.ReadToEnd();
                    standardErrorText = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    excuteTotalTime = (process.ExitTime - process.StartTime);

                    //ClearCache(filePath);                           // 清理临时文件
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return standardOutputText;
            }

            public override string Execute(string sourceCodeText)
            {
                return Execute(sourceCodeText, "");
            }

            private void ClearCache(string path)
            {
                if (File.Exists(path)) File.Delete(path);
                if (File.Exists(path + ".exe")) File.Delete(path + ".exe");
            }
        }
        //public class JavaCompiler : CompilingLanguage
        //{
        //    private CompilingLanguage instance = null;
        //    public CompilingLanguage GetInstance()
        //    {
        //        if (instance == null)
        //            instance = new JavaCompiler();
        //        return instance;
        //    }
        //}
    }
}
