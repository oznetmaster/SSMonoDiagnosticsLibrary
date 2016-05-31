using Crestron.SimplSharp;
using SSMono.IO;

namespace SSMono.Diagnostics
	{
	public class DefaultTraceListener : TraceListener
		{
		public bool AssertUiEnabled { get; set; }

		public string LogFileName { get; set; }

		public DefaultTraceListener ()
			: base ("Default")
			{
			LogFileName = null;
			AssertUiEnabled = false;
			}

		public DefaultTraceListener (string logFileName)
			: this ()
			{
			LogFileName = logFileName;
			}

		public override void Write (string message)
			{
			WriteImpl (message);
			}

		public override void WriteLine (string message)
			{
			string msg = message + CrestronEnvironment.NewLine;
			WriteImpl (msg);

			NeedIndent = true;
			}

		private void WriteImpl (string message)
			{
			if (NeedIndent)
				WriteIndent ();

			Debugger.Write (message);

			WriteLogFile (message, LogFileName);
			}

		private void WriteLogFile (string message, string logFile)
			{
			string fname = logFile;
			if (string.IsNullOrEmpty (fname))
				return;
			FileInfo info = new FileInfo (fname);
			StreamWriter sw;

			// Open the file
			try
				{
				if (info.Exists)
					sw = info.AppendText ();
				else
					sw = info.CreateText ();
				}
			catch
				{
				// We weren't able to open the file for some reason.
				// We can't write to the log file; so give up.
				return;
				}

			using (sw)
				{
				sw.Write (message);
				sw.Flush ();
				}
			}
		}
	}