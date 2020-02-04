//http://www.cheynewallace.com/get-active-ports-and-associated-process-names-in-c/
public static class PortsHelper
{
    // ===============================================
// The Method That Parses The NetStat Output
// And Returns A List Of Port Objects
// ===============================================
    public static IEnumerable<Port> GetNetStatPorts()
    {
        var ports = new List<Port>();

        try
        {
            using (var p = new Process())
            {
                var ps = new ProcessStartInfo
                {
                    Arguments = "-a -n -o",
                    FileName = "netstat.exe",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                p.StartInfo = ps;
                p.Start();

                StreamReader stdOutput = p.StandardOutput;
                StreamReader stdError = p.StandardError;

                string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                var exitStatus = p.ExitCode.ToString();

                if (exitStatus != "0")
                {
                    // Command Errored. Handle Here If Need Be
                }

                //Get The Rows
                string[] rows = Regex.Split(content, "\r\n");
                foreach (string row in rows)
                {
                    //Split it baby
                    string[] tokens = Regex.Split(row, "\\s+");
                    if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                    {
                        string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                        var item = new Port();
                        item.Protocol = localAddress.Contains("1.1.1.1") ? $"{tokens[1]}v6" : $"{tokens[1]}v4";

                        item.PortNumber = localAddress.Split(':')[1];
                        if (tokens[1] == "UDP")
                        {
                            (string processName, string processFullPath) = LookupProcess(Convert.ToInt16(tokens[4]));
                            item.ProcessName = processName;
                            item.ProcessFullPath = processFullPath;
                        }
                        else
                        {
                            (string processName, string processFullPath) = LookupProcess(Convert.ToInt16(tokens[5]));
                            item.ProcessName = processName;
                            item.ProcessFullPath = processFullPath;
                        }

                        ports.Add(item);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        return ports;
    }

    public static (string processName, string processFullPath) LookupProcess(int pid)
    {
        string procName;
        string processFullPath;
        try
        {
            var process = Process.GetProcessById(pid);
            procName = process.ProcessName;
            processFullPath = process.MainModule?.FileName;
        }
        catch (Exception)
        {
            procName = "-";
            processFullPath = "-";
        }

        return (procName, processFullPath);
    }

// ===============================================
// The Port Class We're Going To Create A List Of
// ===============================================
    public class Port
    {
        //public string Name => $"{ProcessName} ({Protocol} port {PortNumber})";

        public string PortNumber { get; set; }
        public string ProcessName { get; set; }
        public string Protocol { get; set; }

        public string ProcessFullPath { get; set; }
    }
}
