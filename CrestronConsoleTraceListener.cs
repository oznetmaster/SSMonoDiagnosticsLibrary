using Crestron.SimplSharp;

namespace SSMono.Diagnostics
	{
	public class CrestronConsoleTraceListener : TraceListener
		{
		public CrestronConsoleTraceListener ()
			: base ("CrestronConsole")
			{
			
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

			CrestronConsole.Print (message);
			}
		}
	}