using Crestron.SimplSharp;
using System;

namespace SSMono.Diagnostics
	{
	public class ErrorlogTraceListener : TraceListener
		{
		public enum TraceLevel
			{
			Error,
			Warning,
			Information
			}

		private TraceLevel _traceLevel;

		public ErrorlogTraceListener ()
			: base ("Errorlog")
			{
			_traceLevel = TraceLevel.Information;
			}

		public ErrorlogTraceListener (TraceLevel traceLevel)
			: this ()
			{
			_traceLevel = traceLevel;
			}

		public ErrorlogTraceListener (string initializeData)
			: this ()
			{
			if (string.IsNullOrEmpty (initializeData))
				return;

			try
				{
				_traceLevel = (TraceLevel)Enum.Parse (typeof (TraceLevel), initializeData, true);
				}
			catch (Exception)
				{
				}
			}

		public override void Write (string message)
			{
			if (message.StartsWith ("Error : 0 : "))
				ErrorLog.Error (message);
			else if (message.StartsWith ("Warning : 0 : "))
				ErrorLog.Warn (message);
			else if (message.StartsWith ("Information : 0 : "))
				ErrorLog.Info (message);
			else
				switch (_traceLevel)
					{
					case TraceLevel.Error:
						ErrorLog.Error (message);
						break;
					case TraceLevel.Warning:
						ErrorLog.Warn (message);
						break;
					case TraceLevel.Information:
						ErrorLog.Info (message);
						break;
					}
			}

		public override void WriteLine (string message)
			{
			Write (message + CrestronEnvironment.NewLine);
			}
		}
	}