using Crestron.SimplSharp;


namespace SSMono.Diagnostics
	{
	public class VSDebuggerTraceListener : TraceListener
		{

		public VSDebuggerTraceListener ()
			: base ("VSDebugger")
			{
			}

		public override void Write (string message)
			{
			Debugger.Write (message);
			}

		public override void WriteLine (string message)
			{
			Debugger.WriteLine (message);
			}

		}
	}